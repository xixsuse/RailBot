using System;
using System.Net;
using System.Threading;
using System.Collections.Generic;

namespace RailBot
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			using (var wc = new WebClient ()) {
				var carrier = new Carrier (wc);

				while (true) {
					if (carrier.GetBotData ())
						DataParser.ParseQuestion (carrier.Question);
//					if (carrier.AskToWeb (new QuestionData("chieri")))
//						Console.WriteLine (carrier.Response);
					Thread.Sleep (5000);
				}
			}
		}
	}
}
