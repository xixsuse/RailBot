using System;

namespace RailBot
{
	public class QuestionData
	{
		public int UpdateID { get; private set;}
		public int ChatID { get; private set;}
		public string Station { get; private set;}
		public int? TrainNumber { get; private set;}
		public TrainTypeEnum TrainType { get; private set;}

		public QuestionTypeEnum QuestionType { 
			get 
			{ 
				return (Station != null ? QuestionTypeEnum.Station : QuestionTypeEnum.TrainNumber);
			}
		}

		public bool IsError {
			get{ return (ErrorMessage == null ? false : true); }
		}

		string _errorMessage = null;
		public string ErrorMessage {
			get{ return _errorMessage; }
			private set { _errorMessage = value; }
		}

		public QuestionData (int updateID, int chatID)
		{
			UpdateID = updateID;
			ChatID = chatID;
		}

		public void SetQuestionInfo (bool commandFound, string station = null, int? trainNumber = null, TrainTypeEnum trainType = TrainTypeEnum.Both)
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
	}
}

