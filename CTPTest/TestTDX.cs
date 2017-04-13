using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaiFeng
{
	public class TestTDX
	{
		public TestTDX()
		{
			_tdx = new TdxTrade();
			_tdx.OnFrontConnected += _tdx_OnFrontConnected;
			_tdx.OnRspUserLogin += _tdx_OnRspUserLogin;
			_tdx.OnRtnOrder += _tdx_OnRtnOrder;
			_tdx.OnRtnCancel += _tdx_OnRtnCancel;
			_tdx.OnRtnTrade += _tdx_OnRtnTrade;
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
			Debug.WriteLine(msg);
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
			//if (e.Value == 0)
				_tdx.ReqOrderInsert("000001", DirectionType.Buy, OffsetType.Open, 8.3, 100, 1001);
		}

		private void _tdx_OnFrontConnected(object sender, EventArgs e)
		{
			Log("connected.");

			_tdx.ReqUserLogin(_ivnestor, _pwd, _ext);
		}

		TdxTrade _tdx = null;
		private string _ext;
		private string _pwd;
		private string _ivnestor;

		public void Run(string user, string pwd, string ext = "")
		{
			_ivnestor = user;
			_pwd = pwd;
			_ext = ext;
			_tdx.ReqConnect();
		}
	}
}
