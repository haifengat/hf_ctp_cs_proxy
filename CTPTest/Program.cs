using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaiFeng
{
	class Program
	{
		static void Main(string[] args)
		{
			var tdx = new TestTDX();
			tdx.Run(args);

			Console.ReadKey(true);
		}
	}
}
