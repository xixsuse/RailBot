using System;

namespace RailBot
{
	public static class Addresses
	{
		public static string TokenURL {
            get{ return "https://api.telegram.org/bot"+
                Configurator.GetAuthTokenFromConfiguration() + "/"; }
		}

		private static readonly string _getURL = TokenURL + 
			"getUpdates?" +
            "offset=:offset:&timeout=60:&timeout=60";

		public static string GetURL(string offset = null)
		{
			if(offset!=null)
				return _getURL.Replace(":offset:", offset);
            return _getURL;
		}

        private static readonly string _sendURL = TokenURL +
                                                  "sendMessage?" +
                                                  "chat_id=:chat_id:&text=:text:";

        public static string SendURL(int chatID, string message)
        {
            var idString = chatID.ToString();
            if (idString != null && message != null)
                return _sendURL.Replace(":chat_id:", idString)
                    .Replace(":text:", message);
            else
                return null;
        }

		public static string ViaggiaURL  {
            get{ return "http://www.viaggiatreno.it/" +
                "vt_pax_internet/mobile/stazione?lang=IT"; }
		}
	}
}

