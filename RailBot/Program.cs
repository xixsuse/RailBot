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
                                var responseData =
                                    DataParser.ParseResponse(carrier.Response, 
                                        data);
                                try{
                                    carrier.SendDataToBot(responseData);
                                }
                                catch
                                {
                                    responseData.ErrorMessage = 
                                        "Probabilmente" +
                                        " la risposta è troppo " +
                                        "lunga per le nostre caapcità. " +
                                        "Provare con i comandi /arrivi e /partenze.";
                                    carrier.SendDataToBot(responseData);
                                }
                            }
                        }
                    }
					Thread.Sleep (5000);
				}
			}
		}
	}
}
