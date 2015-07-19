using System;

namespace RailBot
{
	public static class Commands
	{
        public static readonly string Start = "start";
        public static readonly string Help = "help";
        public static readonly string Station = "stazione";
        public static readonly string Arrivals = "arrivi";
        public static readonly string Departures = "partenze";
		public static readonly string TrainNumber = "numero";

        public static bool IsStartOrHelp(string command)
        {
            if (command.ToUpper() == Start.ToUpper() || 
                command.ToUpper() == Help.ToUpper())
                return true;
            return false;
        }
	}
}

