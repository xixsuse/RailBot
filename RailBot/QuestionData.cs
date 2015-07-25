using System;

namespace RailBot
{
    public class QuestionData : Data
	{
		public string Station { get; private set;}
		public int? TrainNumber { get; private set;}
		public TrainTypeEnum TrainType { get; private set;}
        public bool AmIStart
        {
            get;
            private set;
        }
        public bool AmIHelp
        {
            get;
            private set;
        }

        bool _ignoreQuestion = false;
        public bool IgnoreQuestion
        {
            get { return _ignoreQuestion; }
            set { _ignoreQuestion = value; }
        }

		public QuestionTypeEnum QuestionType { 
			get 
			{ 
                return (Station != null ? QuestionTypeEnum.Station : 
                    (TrainNumber != null ? QuestionTypeEnum.TrainNumber : 
                        QuestionTypeEnum.None));
			}
		}

        public QuestionData (int updateID, int chatID) : 
            base(updateID, chatID)
		{ }

		public void SetQuestionInfo (bool commandFound, string station = null, 
            int? trainNumber = null, 
            TrainTypeEnum trainType = TrainTypeEnum.Both)
		{			
			if (!commandFound)
				ErrorMessage = "Comando non trovato, /help per avere aiuto.";

			if (station == null && trainNumber == null)
            {
                ErrorMessage = "Comando non trovato, /help per avere aiuto.";
			}
			Station = station;
			TrainNumber = trainNumber;
			TrainType = trainType;
		}

        public bool IsStartOrHelp(string command)
        {
            var b = Commands.IsStartOrHelp(command);
            if (Commands.IsStartOrHelp(command))
            {
                if (command.ToUpper() == Commands.Start.ToUpper())
                    AmIStart = true;
                if (command.ToUpper() == Commands.Help.ToUpper())
                    AmIHelp = true;
            }
            return b;
        }
	}
}

