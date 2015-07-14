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
				carrier.GetBotData ();
				if(carrier.Question != null)
					Console.WriteLine (carrier.Question);
				Thread.Sleep (5000);
			}
		}
	}
}
