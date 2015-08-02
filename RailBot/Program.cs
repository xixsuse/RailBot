using System;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Net.Security;

namespace RailBot
{
	class MainClass
	{
		public static void Main (string[] args)
        {
            ServicePointManager.ServerCertificateValidationCallback = 
                new RemoteCertificateValidationCallback(
                    delegate { return true; });
            
			using (var wc = new WebClient ()) {

                while (true) {
                    try
                    {
                        var carrier = new Carrier (wc);
                        carrier.GetBotData();
                        if (carrier.Question != null)
                        {
                            var questions = 
                                DataParser.ParseQuestion (carrier.Question);
                            foreach (var data in questions)
                            {
                                if (!data.IgnoreQuestion)
                                {
                                    if(data.AmIStartOrHelp)
                                    {
                                        carrier.SendStartOrHelpMessage(data);
                                        continue;
                                    }
                                    carrier.AskToWeb(data);
                                    var responseData =
                                        DataParser.ParseResponse(carrier.Response, 
                                            data);
                                    try
                                    {
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
				    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message + "\n"+e.StackTrace);
                    }
                    finally
                    {
                        Thread.Sleep(5000);
                    }
			    }
		    }
	    }
    }
}
