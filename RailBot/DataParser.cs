using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Text;

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

        #region Question Parsing

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
            
            if (command.Length <= 2 || command.Remove(2) != "\\/")
			{
                data.ErrorMessage = "Comando non valido. "+
                    "I comandi cominciano con /. "+
                    "Scrivi /help per avere un aiuto";
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
                data.ErrorMessage = "Argomento non valido. "+
                    "Tutti i comandi devono avere un argomento. "+
                    "Scrivi /help per avere aiuto.";
                return;
            }
            argument = argument.TrimEnd();

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

        #endregion

        #region Response Parsing

        public static ResponseData ParseResponse(string response, 
            QuestionData qdata)
        {
            var data = new ResponseData(qdata.UpdateID, qdata.ChatID);

            if (qdata.IsError)
            {
                data.ErrorMessage = qdata.ErrorMessage;
                return data;
            }
                
            if (response.ToUpper()
                .Contains("Inserire almeno 3 caratteri".ToUpper()))
            {
                data.ErrorMessage = "Selezionare una stazione esistenze";
                return data;
            }
            else
            {
                data.Message = response;
            }


            var builder = new StringBuilder();


            var h1 = new Regex(@"<h1>.*<\/h1>");
            var stazione = h1.Match(response);

            builder.AppendLine(stazione.Value
                .Replace("<h1>","")
                .Replace("</h1>",""));
            builder.AppendLine();

            var numeroTreno = new Regex(@"<h2>.*<\/h2>");
            var numeriTreni = numeroTreno.Matches(response);


            var strong = new Regex(@"<strong>.*<\/strong>");
            var strongs = strong.Matches(response);


            int strongsCounter = 0;
            foreach (Match m in numeriTreni)
            {

                builder.AppendLine();
                if ((strongs[strongsCounter] as Match).Value.ToUpper()
                    == "<strong>Partenze</strong>".ToUpper())
                {
                    builder.AppendLine("PARTENZE");
                    builder.AppendLine();
                    ++strongsCounter;
                }

                if ((strongs[strongsCounter] as Match).Value.ToUpper()
                    == "<strong>arrivi</strong>".ToUpper())
                {
                    builder.AppendLine("ARRIVI");
                    builder.AppendLine();
                    ++strongsCounter;
                }
                    

                builder.AppendLine(m.Value
                    .Replace("<h2>","")
                    .Replace("</h2>",""));

                var actualStrongCounter = strongsCounter + 2;
                for (int i = strongsCounter; i < actualStrongCounter; i++)
                {
                    builder.AppendLine((strongs[i] as Match).Value
                        .Replace("<strong>","")
                        .Replace("</strong>",""));

                    ++strongsCounter;
                }
            }


            data.Message = builder.ToString();
            data.Message = response;

            return data;
        }

        #endregion
	}
}
