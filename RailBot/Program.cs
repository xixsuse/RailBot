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

                while (true) {
                    var carrier = new Carrier (wc);
                    carrier.GetBotData();
                    if (carrier.Question != null){
                        var questions = 
                            DataParser.ParseQuestion (carrier.Question);
                        foreach (var data in questions)
                        {
                            carrier.AskToWeb(data);
                            if (carrier.Response != null)
                            {
                                var response = carrier.Response;
//                                    DataParser.ParseResponse(carrier.Response);
                                carrier.SendDataToBot(response, data);
                            }
                        }
                    }
					Thread.Sleep (5000);
				}
			}
		}
	}
}
