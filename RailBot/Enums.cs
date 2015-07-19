using System;

namespace RailBot
{
	[Flags]
	public enum TrainTypeEnum
	{
		Arrivals = 1,
		Departures = 2,
		Both = Arrivals | Departures,
	}

	public enum QuestionTypeEnum
	{
        None,
		Station,
		TrainNumber,
	}
}

