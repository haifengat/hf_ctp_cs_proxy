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
			if (args.Length < 3)
			{
				Console.WriteLine("params: ivnestor, pwd, ext");
				Console.ReadKey(true);
				return;
			}
			//test tdx
			var tdx = new TestTDX();
			tdx.Run(args[0], args[1], args[2]);
						
			Console.ReadKey(true);
		}
	}
}
