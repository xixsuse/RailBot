using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace RailBot
{

	public static class DataParser
	{
		static class Constants
		{
			public static readonly string UpdateID = "\"update_id\":";
			public static readonly string ChatID = "\"chat\":{\"id\":";
			public static readonly string Text = "\"text\":\"";
		}

		public static List<QuestionData> ParseQuestion(string question)
		{
			var dataList = new List<QuestionData> ();

			var n = new Regex(Regex.Escape(Constants.UpdateID))
                .Matches(question).Count;

			var s = question;
            var lastUpdateID = 0;
			for (int i = 0; i < n; i++) {

				s = s.Substring (s.IndexOf (Constants.UpdateID) + 
                    Constants.UpdateID.Length);
				var updateID = int.Parse(s.Remove(s.IndexOf(',')));
                lastUpdateID = updateID;
				s = s.Substring (s.IndexOf (Constants.ChatID) + 
                    Constants.ChatID.Length);
				var chatID = int.Parse(s.Remove(s.IndexOf(',')));

				s = s.Substring (s.IndexOf (Constants.Text) + 
                    Constants.Text.Length);
				var text = s.Remove(s.IndexOf('\"'));

				var data = new QuestionData (updateID, chatID);
				dataList.Add (data);

				ParseBotCommand (text, data);

            }
            if (lastUpdateID != 0)
                Configurator.WriteConfiguration(lastUpdateID);

			return dataList;
            }
            
		private static void ParseBotCommand(string command, QuestionData data)
		{
			if (command.Remove(2) != "\\/")
			{
                data.SetError("Comando non valido. "+
                    "I comandi cominciano con /. "+
                    "Scrivi /help per avere un aiuto");
				return;
			}

			var split = command.Split (' ');

            var s = split[0].Replace("\\/", "");
            if (data.IsStartOrHelp(s))
                return;

            string argument = null;
            for (int i = 1; i < split.Length; i++)
            {
                argument += split[i] + " ";
            }
                
            if (argument == null)
            {
                data.SetError("Argomento non valido. "+
                    "Tutti i comandi devono avere un argomento. "+
                    "Scrivi /help per avere aiuto.");
                return;
            }

            string station = null;
			int? trainNumber = null;
			int volatileTrainNumber = 0;
            var trainType = TrainTypeEnum.Both;

            if (s.ToUpper() == Commands.Station.ToUpper())
                station = argument;
            else if (s.ToUpper() == Commands.TrainNumber.ToUpper() &&
                     int.TryParse(argument, out volatileTrainNumber))
                trainNumber = volatileTrainNumber;
            else if (s.ToUpper() == Commands.Arrivals.ToUpper())
            {
                station = argument;
                trainType = TrainTypeEnum.Arrivals;
            }
            else if (s.ToUpper() == Commands.Departures.ToUpper())
            {
                station = argument;
                trainType = TrainTypeEnum.Departures;
            }

			data.SetQuestionInfo(true, station, trainNumber, trainType);
		}

        public static string ParseResponse(string response)
        {
            throw new NotImplementedException();
        }
	}
}
