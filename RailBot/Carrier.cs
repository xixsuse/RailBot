using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace RailBot
{
	public class Carrier
	{

        class Constants
        {
            public static readonly string HelpMessage = 
                "I comandi possibili sono:" + Environment.NewLine +
                "/stazione" + Environment.NewLine +
                "/partenze" + Environment.NewLine +
                "/arrivi" + Environment.NewLine +
                "seguiti dal nome della stazione di cui si vuole " +
                "conoscere la situazione dei treni." + Environment.NewLine +
                "Il bot risponderà con la situazione dei treni in tempo " +
                "reale, binari, orario ed eventuale ritardo." +
                Environment.NewLine + "In ogni momento puoi usare il comando" +
                Environment.NewLine + "/help" + Environment.NewLine +
                "Per rileggere questo aiuto.";

            public static readonly string StartMessage = 
                "BEVENUTO IN RAILBOT! " + Environment.NewLine +
                "IL BOT CHE RENDE LA VITA DEL VIAGGIATORE PIU' SEMPLICE" +
                Environment.NewLine + HelpMessage;
        }

        public string Response {
			get;
			private set;
		}

		public string Question {
			get;
			private set;
		}

		WebClient _wc;

		public Carrier (WebClient wc)
		{
			_wc = wc;
		}

		public void GetBotData ()
        {
            Question = _wc.DownloadString (Addresses.GetURL(
                Configurator.GetOffsetFromConfiguration()));
		}

        public void AskToWeb(QuestionData data)
		{
            if (data.IsError)
                return;
			using (WebClient client = new WebClient())
			{
				try
				{
                    byte[] response = null;
                    if (data.QuestionType == QuestionTypeEnum.Station)
						response = client.UploadValues(Addresses.ViaggiaURL, 
                            "POST",
                            new NameValueCollection()
							{
                                { "stazione", data.Station },
								{ "lang", "IT" }
							});
                    if(data.QuestionType == QuestionTypeEnum.TrainNumber)
                        response = client.UploadValues(Addresses.ViaggiaURLNumbero, 
                            "POST",
                            new NameValueCollection()
                            {
                                { "numeroTreno", data.TrainNumber.ToString()},
                                { "lang", "IT" }
                            });
                    else
                    {
                        data.ErrorMessage = "Bravo! Sei riuscito ad arrivare " +
                            "in un punto non previsto. Per favore, segnala " +
                            "questo problema al team di sviluppo di RailBot";
                        return;
                    }
                        

                    Response = System.Text.Encoding.UTF8.GetString(response);
				}
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    data.ErrorMessage = "Errore nella ricezione dei dati. "+
                        "Per favore riportare questo errore al team " +
                        "di sviluppo.";
                }
			}
		}

        public void SendDataToBot(ResponseData response)
        {
            if(!response.IsError)
                _wc.DownloadString (Addresses.SendURL(response.ChatID, 
                    response.Message));
            else
                _wc.DownloadString (Addresses.SendURL(response.ChatID, 
                    response.ErrorMessage));
        }

        public void SendStartOrHelpMessage(QuestionData data)
        {
            ResponseData r = new ResponseData(data.UpdateID, data.ChatID);
            if (data.AmIHelp)
            {
                r.Message = Constants.HelpMessage;
            }
            if (data.AmIStart)
            {
                r.Message = Constants.StartMessage;
            }
            SendDataToBot(r);
        }
	}
}

