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
                            new NameValueCollection()
							{
								{ "stazione", data.Station },
								{ "lang", "IT" }
							});

					Response = System.Text.Encoding.UTF8.GetString(response);
				}
                catch
                {
                }
			}
		}

        public void SendDataToBot(string data)
        {
            throw new NotImplementedException();
        }
	}
}

