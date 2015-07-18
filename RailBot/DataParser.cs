using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace RailBot
{

	public static class DataParser
	{
		class Constants
		{
			public static readonly string UpdateID = "\"update_id\":";
			public static readonly string ChatID = "\"chat\":{\"id\":";
			public static readonly string Text = "\"text\":\"";
		}

		public static List<QuestionData> ParseQuestion(string question)
		{
			var dataList = new List<QuestionData> ();

			var n = new Regex(Regex.Escape(Constants.UpdateID)).Matches(question).Count;

			var s = question;
			for (int i = 0; i < n; i++) {

				s = s.Substring (s.IndexOf (Constants.UpdateID) + Constants.UpdateID.Length);
				var updateID = int.Parse(s.Remove(s.IndexOf(',')));

				s = s.Substring (s.IndexOf (Constants.ChatID) + Constants.ChatID.Length);
				var chatID = int.Parse(s.Remove(s.IndexOf(',')));

				s = s.Substring (s.IndexOf (Constants.Text) + Constants.Text.Length);
				var text = s.Remove(s.IndexOf('\"'));

				var data = new QuestionData (updateID, chatID);
				dataList.Add (data);

				ParseBotCommand (text, data);
			}

			return dataList;
		}


		private static void ParseBotCommand(string command, QuestionData data)
		{
			if (command.Remove(2) != "\\/")
			{
				data.SetQuestionInfo(false);
				return;
			}

			var split = command.Split (' ');

			var s = split [0].Replace ("\\/", "");

			string station = null;
			int? trainNumber = null;
			int volatileTrainNumber = 0;

			if (split.Length > 1) {
				if (s.ToUpper () == Commands.Station.ToUpper ())
					station = split [1];
				else if (s.ToUpper () == Commands.TrainNumber.ToUpper () && 
					int.TryParse (split [1], out volatileTrainNumber))
					trainNumber = volatileTrainNumber;
			}

			var t = TrainTypeEnum.Both;

			if (split.Length > 2) {
				if (split [2].ToUpper ().Contains ("arriv".ToUpper ()))
					t = TrainTypeEnum.Arrivals;
				else if (split [2].ToUpper ().Contains ("part".ToUpper ()) ||
				        split [2].ToUpper ().Contains ("depart".ToUpper ()))
					t = TrainTypeEnum.Departures;
			}

			data.SetQuestionInfo(true, station, trainNumber, t);
	}
}
}
