using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XAPI;
using XAPI.Callback;

namespace HaiFeng
{
	public class TDX
	{
		tdx_q _q = new tdx_q();
		tdx_t _t = new tdx_t();
		XApi xapi = null;
		private ReqQueryField queryField;
		private Thread _trdQry = null;
		private List<string> _sub_insts = new List<string>();// new[] { "600020" });

		public TDX(string tdx_login_lua = @"C:\TdxW_HuaTai\Login.lua")
		{
			Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));
			Debug.AutoFlush = true;
			Debug.Indent();

			xapi = new XApi(@"Tdx\Tdx_Trade_x86.dll");
			xapi.OnConnectionStatus = OnConnect;
			xapi.OnRspQryTradingAccount = OnRspAccount;
			xapi.OnRspQryInvestorPosition = OnRspPosition;
			xapi.OnRtnOrder = OnRtnOrder;
			xapi.OnRtnTrade = OnRtnTrade;

			xapi.OnRtnDepthMarketData = OnTick;

			xapi.Server.Address = tdx_login_lua;
			xapi.Server.ExtInfoChar128 = tdx_login_lua.Substring(0, tdx_login_lua.LastIndexOf('\\') + 1);// @"C:\TdxW_HuaTai\";
		}
		private void Log(string msg)
		{
			//Console.WriteLine(msg);
			Debug.WriteLine(msg);
		}

		private void OnRtnTrade(object sender, ref XAPI.TradeField trade)
		{
			Log(trade.ToFormattedString());
		}

		private void OnRtnOrder(object sender, ref XAPI.OrderField order)
		{
			//new单会返回两次
			//Log(order.ToFormattedString());
			var of = _t.DicOrderField.GetOrAdd(order.ID, new OrderField
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

			switch (order.Status)
			{
				case XAPI.OrderStatus.New:
					if (!of.IsLocal)
					{
						of.IsLocal = true;
						this.ReqOrderCancel(of.OrderID);
					}
					break;
				case XAPI.OrderStatus.Cancelled:
					of.Status = OrderStatus.Canceled;
					break;
				case XAPI.OrderStatus.Filled:
					of.Status = OrderStatus.Filled;
					break;
				case XAPI.OrderStatus.NotSent:
					break;
				case XAPI.OrderStatus.PartiallyFilled:
					of.Status = OrderStatus.Partial;
					break;
				default:
					of.Status = OrderStatus.Error;
					break;
			}
			of.StatusMsg = Encoding.Default.GetString(order.Text).Trim('\0');
			Log(of.ToString());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="investor">帐号</param>
		/// <param name="pwd">密码</param>
		/// <param name="ext">附加码</param>
		public void ReqUserLogin(string investor, string pwd, string ext)
		{
			xapi.User.UserID = investor;
			xapi.User.Password = pwd;
			xapi.User.ExtInfoChar64 = ext;
			xapi.Connect();
		}

		public void ReqSubscribeMarketData(params string[] insts)
		{
			foreach (var inst in insts)
			{
				if (_sub_insts.IndexOf(inst) < 0)
					_sub_insts.Add(inst);
			}
		}


		private void OnConnect(object sender, ConnectionStatus status, ref RspUserLoginField userLogin, int size1)
		{
			Log($"{(size1 > 0 ? userLogin.ToFormattedStringLong() : status.ToString())}");
			if (size1 == 0)
				if (_trdQry == null)
				{
					_trdQry = new Thread(new ThreadStart(Qry));
					_trdQry.Start();
				}
		}

		private void Qry()
		{
			xapi.ReqQuery(QueryType.ReqQryTradingAccount, ref queryField);
			while (true)
			{
				xapi.ReqQuery(QueryType.ReqQryInvestorPosition, ref queryField);
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
			_t.TradingAccount.Available = account.Available;
			_t.TradingAccount.CloseProfit = account.CloseProfit;
			_t.TradingAccount.Commission = account.Commission;
			_t.TradingAccount.CurrMargin = account.CurrMargin;
			_t.TradingAccount.FrozenCash = account.FrozenCash;
			//_t.TradingAccount.Fund = account.PreBalance
			_t.TradingAccount.PositionProfit = account.PositionProfit;
			_t.TradingAccount.PreBalance = account.PreBalance;
			//_t.TradingAccount.Risk = account.
			Log(_t.TradingAccount.ToString());
		}

		private void OnRspPosition(object sender, ref XAPI.PositionField position, int size1, bool bIsLast)
		{
			var field = _t.DicPositionField.GetOrAdd(position.InstrumentID, new PositionField());
			//field.CloseProfit = 0
			//field.Commission = position.
			field.Direction = DirectionType.Buy;
			field.InstrumentID = position.InstrumentID;
			//field.Margin = position
			field.Position = (int)position.Position;
			//field.PositionProfit = position.Position
			//field.Price = position.
			//field.
			Log(field.ToString());
		}

		public void ReqOrderInsert(string inst, double price, int vol)
		{
			XAPI.OrderField order = new XAPI.OrderField
			{
				InstrumentID = inst,
				Type = XAPI.OrderType.Limit,
				Side = OrderSide.Buy,
				Qty = vol,
				Price = price,
			};
			var ret = xapi.SendOrder(ref order);
			Log(ret);
		}

		public void ReqOrderCancel(string id)
		{
			var ret = xapi.CancelOrder(id);
			Log(ret);
		}

		//行情响应
		private void OnTick(object sender, ref DepthMarketDataNClass marketData)
		{
			if (marketData.Asks.Length < 0) return;

			var tick = _q.DicTick.GetOrAdd(marketData.InstrumentID, new MarketData());
			tick.InstrumentID = marketData.InstrumentID;
			tick.AskPrice = marketData.Asks[0].Price;
			tick.AskVolume = marketData.Asks[0].Size;
			tick.BidPrice = marketData.Bids[0].Price;
			tick.BidVolume = marketData.Bids[0].Size;
			tick.LowerLimitPrice = marketData.LowerLimitPrice;
			tick.UpperLimitPrice = marketData.UpperLimitPrice;
			//tick.UpdateTime = marketData.UpdateTime;
			tick.UpdateMillisec = marketData.UpdateMillisec;
			Log(marketData.ToFormattedStringExchangeDateTime());
		}

	}

	class tdx_q : Quote
	{
		public override bool IsLogin { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

		public override int ReqConnect(string pFront)
		{
			throw new NotImplementedException();
		}

		public override int ReqSubscribeMarketData(params string[] pInstrument)
		{
			throw new NotImplementedException();
		}

		public override int ReqUnSubscribeMarketData(params string[] pInstrument)
		{
			throw new NotImplementedException();
		}

		public override int ReqUserLogin(string pInvestor, string pPassword, string pBroker)
		{
			throw new NotImplementedException();
		}

		public override void ReqUserLogout()
		{
			throw new NotImplementedException();
		}
	}


	class tdx_t : Trade
	{
		public override string TradingDay { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }
		public override bool IsLogin { get => throw new NotImplementedException(); protected set => throw new NotImplementedException(); }

		public override TimeSpan GetExchangeTime()
		{
			throw new NotImplementedException();
		}

		public override ExchangeStatusType GetInstrumentStatus(string pExc)
		{
			throw new NotImplementedException();
		}

		public override int ReqAuth(string pProductInfo, string pAuthCode)
		{
			throw new NotImplementedException();
		}

		public override int ReqConnect(string pFront)
		{
			throw new NotImplementedException();
		}

		public override int ReqOrderAction(string pOrderId)
		{
			throw new NotImplementedException();
		}

		public override int ReqOrderInsert(string pInstrument, DirectionType pDirection, OffsetType pOffset, double pPrice, int pVolume, int pCustom, OrderType pType = OrderType.Limit, HedgeType pHedge = HedgeType.Speculation)
		{
			throw new NotImplementedException();
		}

		public override int ReqUserLogin(string pInvestor, string pPassword, string pBroker)
		{
			throw new NotImplementedException();
		}

		public override void ReqUserLogout()
		{
			throw new NotImplementedException();
		}

		public override int ReqUserPasswordUpdate(string pOldPassword, string pNewPassword)
		{
			throw new NotImplementedException();
		}
	}
}
