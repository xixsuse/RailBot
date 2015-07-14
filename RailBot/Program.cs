using System;
using System.Net;
using System.Threading;

namespace RailBot
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var wc = new WebClient ();
			var carrier = new Carrier (wc);

			while (true) {
				if(carrier.GetBotData ())
					Console.WriteLine (carrier.Question);
				Thread.Sleep (5000);
			}
		}
	}
}
