using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HaiFeng
{
	public class TestTDX
	{
		TdxTrade _tdx = null;
		TdxQuote _q = null;
		private string _ext;
		private string _pwd;
		private string _ivnestor;

		public TestTDX()
		{
			_tdx = new TdxTrade();
			_tdx.OnFrontConnected += _tdx_OnFrontConnected;
			_tdx.OnRspUserLogin += _tdx_OnRspUserLogin;
			_tdx.OnRspUserLogout += _tdx_OnRspUserLogout;
			_tdx.OnRtnOrder += _tdx_OnRtnOrder;
			_tdx.OnRtnCancel += _tdx_OnRtnCancel;
			_tdx.OnRtnTrade += _tdx_OnRtnTrade;
		}

		private void _tdx_OnRspUserLogout(object sender, IntEventArgs e)
		{
			Log($"logout:{e.Value}");
		}

		private void _tdx_OnRtnTrade(object sender, TradeArgs e)
		{
			if (!_tdx.DicOrderField[e.Value.OrderID].IsLocal) return;

			Log($"trade: {e.Value}");
		}

		private void _tdx_OnRtnCancel(object sender, OrderArgs e)
		{
			if (!e.Value.IsLocal) return;
			Log($"cancel: {e.Value}");
		}

		private void Log(string msg)
		{
			Console.WriteLine(msg);
		}

		private void _tdx_OnRtnOrder(object sender, OrderArgs e)
		{
			if (!e.Value.IsLocal) return;
			Log($"order:{e.Value}");

			if (e.Value.Status == OrderStatus.Normal)
				_tdx.ReqOrderAction(e.Value.OrderID);
		}

		private void _tdx_OnRspUserLogin(object sender, IntEventArgs e)
		{
			Log($"Logged:{(e.Value == 0 ? "success" : ("error:" + e.Value))}");
			if (e.Value == 0)//不能直接执行,需要sleep;
			{
				Log($"{_tdx.DicPositionField.Count}");
				new Thread(() =>
				  {
					  Thread.Sleep(1000);
					  //if (_q == null)
					  //{
						 // _q = new TdxQuote(_tdx);
						 // _q.OnRtnTick += _q_OnRtnTick;
						 // _q.ReqSubscribeMarketData("000001");
					  //}

					  _tdx.ReqOrderInsert("600149", DirectionType.Buy, OffsetType.Open, 20.0, 100, 1001);
				  }).Start();
			}
		}

		private void _q_OnRtnTick(object sender, TickEventArgs e)
		{
			//Log($"{e.Tick.AskPrice},{e.Tick.AskVolume},{e.Tick.BidPrice},{e.Tick.BidVolume}");
		}

		private void _tdx_OnFrontConnected(object sender, EventArgs e)
		{
			Log("connected.");

			_tdx.ReqUserLogin(_ivnestor, _pwd, _ext);
		}

		public void Run(params string[] args)
		{
			_ivnestor = args[0];
			_pwd = args[1];
			_ext = args.Length > 2 ? args[2] : "";
			//_tdx.ReqConnect(@"C:\TdxW_HuaTai\login.lua");
			_tdx.ReqConnect("C:\\new_tdx_zcgl\\login.lua");
		}
	}
}
