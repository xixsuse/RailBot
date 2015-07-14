﻿using System;
using System.Net;

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

		public void GetBotData ()
		{			
			Question = _wc.DownloadString (Addresses.geturl);
			if (Question == PreviousQuestion)
				Question = null;
			else
				PreviousQuestion = Question;
		}
	}
}

