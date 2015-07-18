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

		public string PreviousQuestion {
			get;
			private set;
		}

		WebClient _wc;

		public Carrier (WebClient wc)
		{
			_wc = wc;
		}

		public bool GetBotData ()
		{
			Question = _wc.DownloadString (Addresses.GetURL());
			if (Question == PreviousQuestion)
				return false;
			else {
				PreviousQuestion = Question;
				return true;
			}
		}

		public bool AskToWeb(QuestionData parameters)
		{
			using (WebClient client = new WebClient())
			{
				try
				{
					byte[] response =
						client.UploadValues(Addresses.ViaggiaURL, new NameValueCollection()
							{
								{ "stazione", parameters.Station },
								{ "lang", "IT" }
							});

					Response = System.Text.Encoding.UTF8.GetString(response);
						return true;
				}
				catch
				{
					return false;
				}
			}
		}
	}
}

