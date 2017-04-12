using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaiFeng
{
	public class TestTDX
	{
		public TestTDX()
		{
			_tdx = new TDX();
		}

		TDX _tdx = null;

		public void Run(string user, string pwd, string ext="")
		{
			_tdx.ReqUserLogin(user, pwd, ext);
		}

		public void Order()
		{
			_tdx.ReqOrderInsert("000001", 8.3, 100);
		}

		public void Cancel(string id)
		{
			_tdx.ReqOrderCancel(id);
		}
	}
}
