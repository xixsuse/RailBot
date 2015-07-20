using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace RailBot
{
	public class Carrier
	{
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
			using (WebClient client = new WebClient())
			{
				try
				{
					byte[] response =
						client.UploadValues(Addresses.ViaggiaURL, 
                            "POST",
                            new NameValueCollection()
							{
                                { "stazione", data.Station },
								{ "lang", "IT" }
							});

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
	}
}

