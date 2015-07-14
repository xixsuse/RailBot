using System;
using System.Net;
using System.Threading;

namespace RailBot
{
	class MainClass
	{

		const string tokenurl = "https://api.telegram.org/bot112284476:AAExtPGiiA1cmPf8hZLLiBRI29Sv5rr528g/";
		const string geturl = tokenurl + "getUpdates?offset=:offset?offset=:offset:&timeout=60:&timeout=60";

		public static void Main (string[] args)
		{
			WebClient wc = new WebClient ();
			while (true) {
				var page = wc.DownloadString (geturl);
				Console.WriteLine (page);
				Thread.Sleep (2000);
			}
		}
	}
}
