# hf_ctp_cs_proxy
上期技术期货交易api之C#封装，实现接口调用。支持winform数据绑定即时更新。

## 有问题反馈
在使用中有任何问题，欢迎反馈给我，可以用以下联系方式跟我交流

* 邮件(hubert28@qq.com)
* QQ: 24918700
* Q群:65164336
* 代码同步
    * https://git.oschina.net/haifengat
    * https://github.com/haifengat

## 接口使用代码
```c
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HaiFeng
{
	public class TestQuote
	{
		CTPQuote _q = null;
		string _investor = "008105", _broker = "9999";
		public TestQuote()
		{
			_q = new CTPQuote();

			_q.OnFrontConnected += _q_OnFrontConnected;
			_q.OnRspUserLogin += _q_OnRspUserLogin;
			_q.OnRspUserLogout += _q_OnRspUserLogout;
			_q.OnRtnTick += _q_OnRtnTick;
			_q.OnRtnError += _q_OnRtnError;
		}

		public void Release()
		{
			_q.ReqUserLogout();
		}

		public void Run()
		{
			_q.ReqConnect("tcp://180.168.146.187:10010");
		}

		void Log(string pMsg)
		{
			Console.WriteLine(DateTime.Now.TimeOfDay + "\t" + pMsg);
		}

		private void _q_OnFrontConnected(object sender, EventArgs e)
		{
			Log("connected");
			_q.ReqUserLogin(_investor, "12", _broker);
		}

		private void _q_OnRspUserLogin(object sender, IntEventArgs e)
		{
			if (e.Value == 0)
			{
				Log($"登录成功:{_investor}");
				_q.ReqSubscribeMarketData("rb1705", "cu1705");
			}
			else
			{
				Log($"登录错误：{e.Value}");
				_q.ReqUserLogout();
			}
		}

		private void _q_OnRtnTick(object sender, TickEventArgs e)
		{
			Log($"{e.Tick.InstrumentID}\t{e.Tick.LastPrice}");
		}

		private void _q_OnRspUserLogout(object sender, IntEventArgs e)
		{
			Log($"quote logout: {e.Value}");
		}

		private void _q_OnRtnError(object sender, ErrorEventArgs e)
		{
			Log(e.ErrorMsg);
		}
	}

	class TestTrade
	{
		CTPTrade _t = null;
		string _broker = "9999", _ivnestor = "008105";

		public TestTrade()
		{
			_t = new CTPTrade();
		}

		public void Release()
		{
			_t.ReqUserLogout();
		}

		void Log(string pMsg)
		{
			Console.WriteLine(DateTime.Now.TimeOfDay + "\t" + pMsg);
		}

		public void Run()
		{
			_t.OnFrontConnected += _t_OnFrontConnected;
			_t.OnRspUserLogin += _t_OnRspUserLogin;
			_t.OnRtnOrder += _t_OnRtnOrder;
			_t.OnRtnTrade += _t_OnRtnTrade;
			_t.OnRtnCancel += _t_OnRtnCancel;
			_t.ReqConnect("tcp://180.168.146.187:10000");
		}

		private void _t_OnRtnCancel(object sender, OrderArgs e)
		{
			Log($"{e.Value.StatusMsg}\t{e.Value.InstrumentID}\t{e.Value.Direction}\t{e.Value.Offset}\t{e.Value.LimitPrice}\t{e.Value.Volume}");
		}

		private void _t_OnRtnTrade(object sender, TradeArgs e)
		{
			Log($"{e.Value.InstrumentID}\t{e.Value.Direction}\t{e.Value.Offset}\t{e.Value.Price}\t{e.Value.Volume}");
		}

		private void _t_OnRtnOrder(object sender, OrderArgs e)
		{
			Log($"{e.Value.InstrumentID}\t{e.Value.Direction}\t{e.Value.Offset}\t{e.Value.LimitPrice}\t{e.Value.Volume}");

			if (e.Value.IsLocal)
				_t.ReqOrderAction(e.Value.OrderID);
		}

		private void _t_OnFrontConnected(object sender, EventArgs e)
		{
			_t.ReqUserLogin(_ivnestor, "1", _broker);
		}

		private void _t_OnRspUserLogin(object sender, IntEventArgs e)
		{
			if (e.Value == 0)
			{
				Log("登录成功");
				_t.ReqOrderInsert("rb1705", DirectionType.Sell, OffsetType.Open, 3200, 1, 1000);
			}
			else
			{
				Log($"登录错误：{e.Value}");
			}
		}

		private void OnRtnInstrumentStatus(ref CThostFtdcInstrumentStatusField pInstrumentStatus)
		{
			Log($"{pInstrumentStatus.InstrumentID}:{pInstrumentStatus.InstrumentStatus}");
		}

		private void OnFrontDisconnected(int nReason)
		{
			Log("连接已断开");
		}
	}

}

```
