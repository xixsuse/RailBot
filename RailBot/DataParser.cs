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
                data.IgnoreQuestion = true;
				return;
			}

			var split = command.Split (' ');

            var s = split[0].Replace("\\/", "");
            if (!Commands.CommandsList.ContainsValue(s.ToUpper()))
            {
                data.ErrorMessage = "Comando non trovato."
                    + Environment.NewLine +
                    "/help per avere aiuto.";
                return;
            }
            if (data.IsStartOrHelp(s))
                return;

            string argument = null;
            for (int i = 1; i < split.Length; i++)
            {
                argument += split[i] + " ";
            }

            if (argument == null)
            {
                data.ErrorMessage = "Argomento non valido. " +
                    Environment.NewLine + 
                    "Tutti i comandi devono avere un argomento. " +
                    Environment.NewLine +
                    "Scrivi /help per avere aiuto.";
                return;
            }
            argument = argument.TrimEnd();

            string station = null;
			int? trainNumber = null;
            var trainType = TrainTypeEnum.Both;

            if (s.ToUpper() == Commands.Station.ToUpper())
                station = argument;
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
            else if (s.ToUpper() == Commands.TrainNumber.ToUpper())
            {
                int number = 0;
                if (int.TryParse(argument, out number))
                {
                    trainNumber = number;
                }
                else
                {
                    data.ErrorMessage = "Numero non valido. Riprovare.";
                    return;
                }
            }
            else
            {
                data.ErrorMessage = "Errore generico. Per favore riportare " +
                    "il problema al tema di sviluppo RailBot.";
                return;
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
                data.ErrorMessage = "Selezionare una stazione esistente";
                return data;
            }
            else if (response.ToUpper()
                .Contains("localita&#039; non trovata".ToUpper()))
            {
                data.ErrorMessage = "Località non trovata";
                return data;
            }
            else if(response.ToUpper()
                .Contains("numero treno non valido".ToUpper()))
            {
                data.ErrorMessage = "Numero treno non valido";
                return data;
            }
            else
            {
                data.Message = response;
            }

            data.Message = ParsePage(qdata, response, qdata.TrainType);

            return data;
        }

        private static string ParsePage(QuestionData qdata, string response, TrainTypeEnum trainType)
        {
            var genericErrorMessage = "Errore generico. Per favore, avvisare " +
                "gli sviluppatori di Rail Bot di ciò che è successo.";

            var builder = new StringBuilder();
            var h1 = new Regex(@"<h1>.*<\/h1>");
            var stazione = h1.Match(response);

            builder.AppendLine(stazione.Value
                .Replace("<h1>", "")
                .Replace("</h1>", ""));
            builder.AppendLine();
            
            if (stazione.Value.ToUpper()
                .Contains("cerca treno".ToUpper()))
                return ParseChooseStationPage(builder, response);

            try
            {
            if (qdata.Station != null)
                return ParseTimeTablePage(response, builder, trainType);
            else if (qdata.TrainNumber.HasValue)
                return ParseTrainNumberPage(builder, response);
            
            else
                return genericErrorMessage;
            }
            catch
            {
                return genericErrorMessage;
            }
        }

        private static string ParseTrainNumberPage(StringBuilder builder, 
            string response)
        {
            var origineSection = SelectSection(response, "<!-- ORIGINE -->");
            var ultimaFermataSection = SelectSection(response, "<!-- ULTIMA FERMATA -->");
            var destinazioneSection = SelectSection(response, "<!-- DESTINAZIONE -->");
            var situazioneSection = SelectSection(response, "<!-- SITUAZIONE -->");

            if (!string.IsNullOrWhiteSpace(origineSection) || !string.IsNullOrWhiteSpace(destinazioneSection))
            {
                string origine=null;
                string orarioOrigine=null;
                string destinazione=null;
                string orarioDestinazione=null;
                if (!string.IsNullOrWhiteSpace(origineSection))
                {
                    origine = new Regex(@"<h2>.*<\/h2>")
                        .Match(origineSection).Value
                        .Replace("<h2>", "").Replace("</h2>", "");
                    
                    var orari = new Regex(@"<strong>[\s\S]*?<\/strong>")
                        .Matches(origineSection);
                    orarioOrigine = Regex.Replace(orari[0].Value, @"\s+", "")
                        .Replace("<strong>", "")
                        .Replace("</strong>", "");
                }
                if (!string.IsNullOrWhiteSpace(destinazioneSection))
                {
                    destinazione = new Regex(@"<h2>.*<\/h2>")
                        .Match(destinazioneSection).Value
                        .Replace("<h2>", "").Replace("</h2>", "");

                    var orari = new Regex(@"<strong>[\s\S]*?<\/strong>")
                        .Matches(destinazioneSection);
                    orarioDestinazione = Regex.Replace(orari[0].Value, @"\s+", "")
                        .Replace("<strong>", "")
                        .Replace("</strong>", "");
                }

                if (origine != null && orarioOrigine != null && destinazione != null && orarioDestinazione != null)
                {
                    builder.AppendLine(
                        string.Format(
                            "{0} ({1}) {4} {2} ({3})",
                            origine, orarioOrigine,
                            destinazione, orarioDestinazione,
                            '\u27A1'
                        )
                    );
                    builder.AppendLine();
                }
            }


            if (!string.IsNullOrWhiteSpace(situazioneSection) || !string.IsNullOrWhiteSpace(ultimaFermataSection))
            {
                builder.AppendLine("SITUAZIONE ù");
                var replacerForù = ' ';
                if (!string.IsNullOrWhiteSpace(situazioneSection))
                {
                    if (situazioneSection.ToUpper().Contains("ritardo".ToUpper()))
                        replacerForù = '\u2757';  
                    else if (situazioneSection.ToUpper().Contains("anticipo".ToUpper()))
                        replacerForù = '\u2728';   
                    else if (situazioneSection.ToUpper().Contains("orario".ToUpper()))
                        replacerForù = '\u2705';    
                    else if (situazioneSection.ToUpper().Contains("partito".ToUpper()))
                        replacerForù = '\u270B';                       

                    builder.AppendLine(
                        Regex.Replace(
                            Regex.Replace(
                                situazioneSection
                    .Replace("<div  class=\"evidenziato\"><strong>", "")
                    .Replace("<br>", ""), @"\r\n?|\n", ""),
                            @"\s+", " ").Replace("&#039;", "'")
                    .TrimStart()
                    );
                }
                if (!string.IsNullOrWhiteSpace(ultimaFermataSection))
                {
                    var fermata = new Regex(@"<h2>.*<\/h2>")
                    .Match(ultimaFermataSection).Value
                    .Replace("<h2>", "").Replace("</h2>", "");
                    builder.AppendLine("Ultima fermata:\t" + fermata);
                    var orari = new Regex(@"<strong>[\s\S]*?<\/strong>")
                    .Matches(ultimaFermataSection);
                    builder.AppendLine("Arrivo programmato:\t" +
                        Regex.Replace(orari[0].Value, @"\s+", "")
                    .Replace("<strong>", "")
                    .Replace("</strong>", ""));
                    builder.AppendLine("Arrivo effettivo:\t" +
                        Regex.Replace(orari[1].Value, @"\s+", "")
                    .Replace("<strong>", "")
                    .Replace("</strong>", ""));
                }
                builder.Replace('ù', replacerForù);
            }



             return builder.ToString();
        }

        private static string SelectSection(string response, string sectionIdentifier)
        {
            try
            {
            var a = response.IndexOf(sectionIdentifier);
            var b = response.Substring(a).Replace(sectionIdentifier, "");
            var c = b.Substring(0, b.IndexOf("<!--"));
                return c;
            }
            catch{return null;}
        }

        private static string ParseChooseStationPage(StringBuilder builder, 
            string response)
        {
            var options = new Regex(@">.*</o").Matches(response);

            builder.AppendLine("Scegliere una stazione dalla seguente lista");
            builder.AppendLine();

            foreach (Match m in options)
            {
                builder.AppendLine(m.Value
                    .Replace('>',' ')
                    .Replace("</o",""));
            }

            return builder.ToString();
        }

        private static string ParseTimeTablePage(string response, 
            StringBuilder builder, TrainTypeEnum trainType)
        {
            var numeroTreno = new Regex(@"<h2>.*<\/h2>");
            var numeriTreni = numeroTreno.Matches(response);
            var strong = new Regex(@"<strong>.*<\/strong>|[^!]--[^>]|\d+[\s]|in orario|ritardo.*\d+");
            var strongs = strong.Matches(response);
            int strongsCounter = 0;

            var departuresList = new List<TrainEntry>();
            var arrivalsList = new List<TrainEntry>();
            var tes = new List<TrainEntry>();

            TrainEntryTypeEnum trainEntryType = TrainEntryTypeEnum.Number;

            foreach (Match m in numeriTreni)
            {

                if ((strongs[strongsCounter] as Match).Value.ToUpper() == "<strong>Partenze</strong>".ToUpper())
                {
                    trainEntryType = TrainEntryTypeEnum.Departure;
                    tes = departuresList;
                    ++strongsCounter;
                }
                if ((strongs[strongsCounter] as Match).Value.ToUpper() == "<strong>arrivi</strong>".ToUpper())
                {
                    trainEntryType = TrainEntryTypeEnum.Arrival;
                    tes = arrivalsList;
                    ++strongsCounter;
                }

                string treno = m.Value.Replace("<h2>", "").Replace("</h2>", "");
                string stazione = string.Empty;
                string ore = string.Empty;
                string binarioPrev = string.Empty;
                string binarioReal = string.Empty;
                string situa = string.Empty;

                var actualStrongCounter = strongsCounter + 5;
                var j = 0;
                for (int i = strongsCounter; i < actualStrongCounter; i++)
                {
                    switch (j)
                    {
                        case 0:
                            stazione = (strongs[i] as Match).Value
                                .Replace("<strong>", "")
                                .Replace("</strong>", "");
                            break;
                        case 1:
                            ore = (strongs[i] as Match).Value
                                .Replace("<strong>", "")
                                .Replace("</strong>", "");
                            break;
                        case 2:
                            binarioPrev = (strongs[i] as Match).Value
                                .Replace("<strong>", "")
                                .Replace("</strong>", "");
                            break;
                        case 3:
                            binarioReal = (strongs[i] as Match).Value
                                .Replace("<strong>", "")
                                .Replace("</strong>", "");
                            break;
                        case 4:
                            situa = (strongs[i] as Match).Value
                                .Replace("<strong>", "")
                                .Replace("</strong>", "");
                            break;
                        default:
                            break;
                    }

                    ++strongsCounter;
                    ++j;
                }
                var te = new TrainEntry(treno, stazione, ore, binarioPrev,
                    binarioReal, situa, trainEntryType);
                tes.Add(te);
            }


            int maxEntriesNum = 30;
            int addedEntries = 0;

            int maxDepartures = trainType == TrainTypeEnum.Departures ?
                maxEntriesNum : trainType == TrainTypeEnum.Arrivals ?
                0 : maxEntriesNum / 2;

            int maxArrivals = trainType == TrainTypeEnum.Arrivals ?
                maxEntriesNum : trainType == TrainTypeEnum.Departures ?
                0 : maxEntriesNum / 2;

            if (trainType == TrainTypeEnum.Departures || 
                trainType == TrainTypeEnum.Both)
            {
                builder.AppendLine("PARTENZE");
                builder.AppendLine();
                foreach (var te in departuresList)
                {
                    if (addedEntries != maxDepartures)
                    {
                        ++addedEntries;
                        builder.Append(te.ToString());
                    }
                    else
                        break;
                }

            }

            if (trainType == TrainTypeEnum.Arrivals || 
                trainType == TrainTypeEnum.Both)
            {
                addedEntries = 0;
                builder.AppendLine("ARRIVI");
                builder.AppendLine();
                foreach (var te in arrivalsList)
                {
                    if (addedEntries != maxArrivals)
                    {
                        ++addedEntries;
                        builder.Append(te.ToString());
                    }
                    else
                        break;
                }
            }
            return builder.ToString();
        }

        #endregion
	}
}
