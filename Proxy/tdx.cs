using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XAPI;
using XAPI.Callback;

namespace HaiFeng
{
	public class TdxTrade : Trade
	{
		XApi xapi = null;
		private ReqQueryField queryField;
		private Thread _trdQry = null;
		private List<string> _sub_insts = new List<string>();// new[] { "600020" });

		public ConcurrentDictionary<string, MarketData> DicTick = new ConcurrentDictionary<string, MarketData>();

		public override string TradingDay { get { return DateTime.Today.ToString("yyyyMMdd"); } protected set { } }
		public override bool IsLogin { get; protected set; }

		public TdxTrade()
		{
			Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
			Debug.AutoFlush = true;
			Debug.Indent();

			xapi = new XApi(@"Tdx\Tdx_Trade_x86.dll");

			xapi.OnConnectionStatus = OnConnect;
			xapi.OnRspQryTradingAccount = OnRspAccount;
			xapi.OnRspQryInstrument = OnRspInstrument;
			xapi.OnRspQryInvestorPosition = OnRspPosition;
			xapi.OnRtnOrder = OnRtnOrder;
			xapi.OnRtnTrade = OnRtnTrade;
			xapi.OnRspQryTrade = OnRspTrade;

			xapi.OnRtnDepthMarketData = OnTick;
		}

		private void OnRspInstrument(object sender, ref XAPI.InstrumentField instrument, int size1, bool bIsLast)
		{
			DicInstrumentField[instrument.InstrumentID] = new InstrumentField
			{
				InstrumentID = instrument.InstrumentID,
			};
		}

		private void Log(string msg)
		{
			//Console.WriteLine($"tdx|{DateTime.Now.ToString("HH:mm:ss")}|{msg}");
			Debug.WriteLine($"tdx|{DateTime.Now.ToString("HH:mm:ss")}|{msg}");
		}
		
		private void OnRspTrade(object sender, ref XAPI.TradeField trade, int size1, bool bIsLast)
		{
			OnRtnTrade(sender, ref trade);
		}

		private void OnRtnTrade(object sender, ref XAPI.TradeField trade)
		{
			if (DicTradeField.TryAdd(trade.ID, new TradeField
			{
				Direction = trade.Side == OrderSide.Buy ? DirectionType.Buy : DirectionType.Sell,
				//ExchangeID = trade.ExchangeID,
				Hedge = HedgeType.Speculation,
				InstrumentID = trade.InstrumentID,
				Offset = trade.OpenClose == OpenCloseType.Open ? OffsetType.Open : OffsetType.Close,
				OrderID = trade.ID,
				Price = trade.Price,
				TradeID = trade.TradeID,
				TradeTime = TimeSpan.FromTicks(trade.Time).ToString(),
				Volume = (int)trade.Qty,
				TradingDay = DateTime.Today.ToString("yyyyMMdd"),
			}))
			{
				//成交价计算（待测）
				var of = DicOrderField[trade.ID];
				of.AvgPrice = ((of.AvgPrice * (of.Volume - of.VolumeLeft)) + (int)trade.Qty * trade.Price) / (of.Volume - of.VolumeLeft + (int)trade.Qty);
				of.VolumeLeft -= (int)trade.Qty;
				of.Status = of.VolumeLeft == 0 ? OrderStatus.Filled : OrderStatus.Partial;

				if (!IsLogin) return;

				if (of.IsLocal)
					_OnRtnOrder?.Invoke(this, new OrderArgs { Value = of });

				Log(trade.ToString());
				_OnRtnTrade?.Invoke(this, new TradeArgs { Value = DicTradeField[trade.ID] });
			}
		}

		private void OnRtnOrder(object sender, ref XAPI.OrderField order)
		{
			//new单会返回两次
			//Log(order.ToFormattedString());
			var of = DicOrderField.GetOrAdd(order.ID, new OrderField
			{
				Direction = order.Side == OrderSide.Buy ? DirectionType.Buy : DirectionType.Sell,
				InsertTime = TimeSpan.FromTicks(order.Time).ToString(),
				InstrumentID = order.InstrumentID,
				IsLocal = false,
				LimitPrice = order.Price,
				Offset = order.OpenClose == OpenCloseType.Open ? OffsetType.Open : OffsetType.Close,
				OrderID = order.ID,
				Status = OrderStatus.Normal,
				StatusMsg = "已报单",
				SysID = order.LocalID,
				Volume = (int)order.Qty,
				VolumeLeft = (int)order.Qty,
			});
			of.StatusMsg = Encoding.Default.GetString(order.Text).Trim('\0');

			switch (order.Status)
			{
				case XAPI.OrderStatus.New:
					if (!of.IsLocal && IsLogin)
					{
						of.IsLocal = true;
						//this.ReqOrderCancel(of.OrderID);
						_OnRtnOrder?.Invoke(this, new OrderArgs { Value = of });
					}
					break;
				case XAPI.OrderStatus.Cancelled:
					of.Status = OrderStatus.Canceled;
					if (of.IsLocal)
						_OnRtnCancel?.Invoke(this, new OrderArgs { Value = of });
					break;
				case XAPI.OrderStatus.Filled:
					//of.Status = OrderStatus.Filled;
					//if (of.IsLocal)
					//	_OnRtnOrder?.Invoke(this, new OrderArgs { Value = of });
					break;
				case XAPI.OrderStatus.NotSent:
					break;
				case XAPI.OrderStatus.PartiallyFilled:
					of.Status = OrderStatus.Partial;
					break;
				default:
					of.Status = OrderStatus.Error;
					if (of.IsLocal)
						_OnRtnErrOrder?.Invoke(this, new ErrOrderArgs { ErrorID = order.XErrorID, ErrorMsg = of.StatusMsg, Value = of });
					break;
			}
			Log(of.ToString());
		}


		internal void ReqSubscribeMarketData(params string[] insts)
		{
			foreach (var inst in insts)
			{
				if (_sub_insts.IndexOf(inst) < 0)
					_sub_insts.Add(inst);
			}
		}

		internal void ReqUnSubscribeMarketData(params string[] insts)
		{
			foreach (var inst in insts)
			{
				if (_sub_insts.IndexOf(inst) > 0)
				{
					_sub_insts.Remove(inst);
					MarketData tmp;
					DicTick.TryRemove(inst, out tmp);
				}
			}
		}


		private void OnConnect(object sender, ConnectionStatus status, ref RspUserLoginField userLogin, int size1)
		{
			Log($"{(size1 > 0 ? userLogin.ToFormattedStringLong() : status.ToString())}");
			if (size1 != 0)
				Log(Encoding.Default.GetString(userLogin.Text)); //错误信息
			if (!IsLogin && status == ConnectionStatus.Done)//Done登录成功,否则会触发Disconnected
			{
				if (_trdQry == null)
				{
					_trdQry = new Thread(new ThreadStart(Qry));
					_trdQry.Start();
				}
			}
			else if (status == ConnectionStatus.Disconnected)
			{
				if (IsLogin)
					_OnRspUserLogout?.Invoke(this, new IntEventArgs { Value = size1 });
				else
					_OnRspUserLogin?.Invoke(this, new IntEventArgs { Value = size1 });
			}

		}

		private void Qry()
		{
			Thread.Sleep(500);

			xapi.ReqQuery(QueryType.ReqQryTradingAccount, ref queryField);
			xapi.ReqQuery(QueryType.ReqQryInvestorPosition, ref queryField);
			xapi.ReqQuery(QueryType.ReqQryOrder, ref queryField);
			xapi.ReqQuery(QueryType.ReqQryTrade, ref queryField);

			Thread.Sleep(500);
			IsLogin = true;
			_OnRspUserLogin?.Invoke(this, new IntEventArgs { Value = 0 });
			while (IsLogin)
			{
				//xapi.ReqQuery(QueryType.ReqQryInstrument, ref queryField);
				xapi.ReqQuery(QueryType.ReqQryTradingAccount, ref queryField);
				xapi.ReqQuery(QueryType.ReqQryInvestorPosition, ref queryField);
				//xapi.ReqQuery(QueryType.ReqQryOrder, ref queryField);
				xapi.ReqQuery(QueryType.ReqQryTrade, ref queryField);
				//查行情
				foreach (var inst in _sub_insts)
					xapi.Subscribe(inst, "");
				Thread.Sleep(1000);
			}
		}


		private void OnRspAccount(object sender, ref AccountField account, int size1, bool bIsLast)
		{
			//Log(account.ToFormattedString());
			//_account = account;
			TradingAccount.Available = account.Available;
			TradingAccount.CloseProfit = account.CloseProfit;
			TradingAccount.Commission = account.Commission;
			TradingAccount.CurrMargin = account.CurrMargin;
			TradingAccount.FrozenCash = account.FrozenCash;
			//_t.TradingAccount.Fund = account.PreBalance
			TradingAccount.PositionProfit = account.PositionProfit;
			TradingAccount.PreBalance = account.PreBalance;
			//_t.TradingAccount.Risk = account.
			//Log(TradingAccount.ToString());
		}

		private void OnRspPosition(object sender, ref XAPI.PositionField position, int size1, bool bIsLast)
		{
			var field = DicPositionField.GetOrAdd(position.InstrumentID, new PositionField());
			//field.CloseProfit = 0
			//field.Commission = position.
			field.Direction = DirectionType.Buy;
			field.InstrumentID = position.InstrumentID;
			//field.Margin = position
			field.Position = (int)position.Position;
			field.YdPosition = (int)position.HistoryPosition;
			//field.PositionProfit = position.Position
			//field.Price = position.
			//field.
			//Log(field.ToString());
		}

		internal Quote.RtnTick _OnRtnTick = null;

		//行情响应
		private void OnTick(object sender, ref DepthMarketDataNClass marketData)
		{
			if (marketData.Asks.Length == 0 || marketData.Bids.Length == 0) return;
			DicExcStatus[marketData.InstrumentID] = ExchangeStatusType.NoTrading;
			switch (marketData.TradingPhase)
			{
				case TradingPhaseType.BeforeTrading:
					DicExcStatus[marketData.InstrumentID] = ExchangeStatusType.BeforeTrading;
					break;
				case TradingPhaseType.AuctionMatch:
					DicExcStatus[marketData.InstrumentID] = ExchangeStatusType.BeforeTrading;
					break;
				case TradingPhaseType.Closed:
					DicExcStatus[marketData.InstrumentID] = ExchangeStatusType.Closed;
					break;
				case TradingPhaseType.Continuous:
					DicExcStatus[marketData.InstrumentID] = ExchangeStatusType.Trading;
					break;
					//case TradingPhaseType.NoTrading:
					//	DicExcStatus[marketData.InstrumentID] = ExchangeStatusType.NoTrading;
					//	break;
			}
			var tick = DicTick.GetOrAdd(marketData.InstrumentID, new MarketData());
			tick.InstrumentID = marketData.InstrumentID;
			tick.LastPrice = marketData.LastPrice;
			tick.AskPrice = marketData.Asks[0].Price;
			tick.AskVolume = marketData.Asks[0].Size;
			tick.BidPrice = marketData.Bids[0].Price;
			tick.BidVolume = marketData.Bids[0].Size;
			tick.LowerLimitPrice = marketData.PreClosePrice * 0.9;// marketData.LowerLimitPrice;
			tick.UpperLimitPrice = marketData.PreClosePrice * 1.1;// marketData.UpperLimitPrice;
																  //tick.UpdateTime = marketData.UpdateTime;
			tick.UpdateMillisec = marketData.UpdateMillisec;
			var tm = marketData.UpdateTime.ToString();
			tick.UpdateTime = DateTime.ParseExact((tm.Length == 5 ? "0" : "") + tm, "HHmmss", CultureInfo.InvariantCulture).ToString("HH:mm:ss");
			//Log(marketData.ToFormattedStringExchangeDateTime());

			_OnRtnTick?.Invoke(this, new TickEventArgs { Tick = tick });
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="pFront"></param>
		/// <returns></returns>
		public override int ReqConnect(string pFront = @"C:\TdxW_HuaTai\Login.lua")
		{
			xapi.Server.Address = pFront;
			xapi.Server.ExtInfoChar128 = pFront.Substring(0, pFront.LastIndexOf('\\') + 1);

			if (_OnFrontConnected != null)//sleep以便应用是在req后收到的on响应
				new Thread(() => { Thread.Sleep(500); _OnFrontConnected(this, new EventArgs()); }).Start();
			return 0;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pInvestor"></param>
		/// <param name="pPassword"></param>
		/// <param name="pBroker">通讯密码</param>
		/// <returns></returns>
		public override int ReqUserLogin(string pInvestor, string pPassword, string pBroker)
		{
			xapi.User.UserID = pInvestor;
			xapi.User.Password = pPassword;
			xapi.User.ExtInfoChar64 = pBroker;
			xapi.Connect();
			return 0;
		}

		public override void ReqUserLogout()
		{
			IsLogin = false;
			Thread.Sleep(1000);
			xapi.Disconnect();
			xapi.Dispose();
			xapi = null;
		}

		public override int ReqAuth(string pProductInfo, string pAuthCode)
		{
			throw new NotImplementedException();
		}

		public override int ReqOrderInsert(string pInstrument, DirectionType pDirection, OffsetType pOffset, double pPrice, int pVolume, int pCustom, OrderType pType = OrderType.Limit, HedgeType pHedge = HedgeType.Speculation)
		{
			XAPI.OrderField order = new XAPI.OrderField
			{
				InstrumentID = pInstrument,
				Type = XAPI.OrderType.Limit,
				Side = pDirection == DirectionType.Buy ? OrderSide.Buy : OrderSide.Sell,
				OpenClose = pOffset == OffsetType.Close ? OpenCloseType.Close : OpenCloseType.Open,
				Price = pPrice,
				Qty = pVolume,
				ReserveInt32 = pCustom,
			};
			switch (pType)
			{
				case OrderType.Market:
					order.Type = XAPI.OrderType.Market;
					break;
			}
			var ret = xapi.SendOrder(ref order);
			Log(ret);
			return 0;// int.Parse(ret);
		}

		public override int ReqOrderAction(string pOrderId)
		{
			var ret = xapi.CancelOrder(pOrderId);
			Log(ret);
			return 0;// int.Parse(ret);
		}

		public override int ReqUserPasswordUpdate(string pOldPassword, string pNewPassword)
		{
			throw new NotImplementedException();
		}

		public override TimeSpan GetExchangeTime()
		{
			return DateTime.Now.TimeOfDay;
		}

		public override ExchangeStatusType GetInstrumentStatus(string pExc)
		{
			return DicExcStatus.Count == 0 ? ExchangeStatusType.NoTrading : DicExcStatus.First().Value;
		}
	}

	public class TdxQuote : Quote
	{
		TdxTrade _t = null;
		public TdxQuote(TdxTrade trade = null)
		{
			_t = trade;
			_t._OnRtnTick += _t_OnRtnTick;
		}

		private void _t_OnRtnTick(object sender, TickEventArgs e)
		{
			_OnRtnTick?.Invoke(this, new TickEventArgs { Tick = e.Tick });
		}

		public override bool IsLogin { get; protected set; } = false;

		public override int ReqConnect(string pFront)
		{
			_OnFrontConnected?.Invoke(this, new EventArgs());
			return 0;
		}

		public override int ReqSubscribeMarketData(params string[] pInstrument)
		{
			_t.ReqSubscribeMarketData(pInstrument);
			return 0;
		}

		public override int ReqUnSubscribeMarketData(params string[] pInstrument)
		{
			_t.ReqUnSubscribeMarketData(pInstrument);
			return 0;
		}

		public override int ReqUserLogin(string pInvestor, string pPassword, string pBroker)
		{
			IsLogin = true;
			_OnRspUserLogin?.Invoke(this, new IntEventArgs { Value = 0 });
			return 0;
		}

		public override void ReqUserLogout()
		{
			_t = null;
		}
	}
}
