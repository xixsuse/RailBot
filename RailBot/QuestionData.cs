using System;

namespace RailBot
{
    public class QuestionData : Data
	{
		public string Station { get; private set;}
		public int? TrainNumber { get; private set;}
		public TrainTypeEnum TrainType { get; private set;}
        public bool AmIStartOrHelp
        {
            get;
            private set;
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
				ErrorMessage = "Possible commands: /station , /number";

			if (station == null && trainNumber == null)
			{
				ErrorMessage = "Possible commands: /station , /number";
			}
			Station = station;
			TrainNumber = trainNumber;
			TrainType = trainType;
		}

        public bool IsStartOrHelp(string command)
        {
            return AmIStartOrHelp = Commands.IsStartOrHelp(command);
        }

	}
}

