using System;

namespace RailBot
{
    public class TrainEntry
    {

        string _numeroTreno;
        string _stazione;
        string _ore;
        string _binarioPrevisto;
        string _binarioReale;
        string _situazione;
        TrainEntryTypeEnum _trainType;

        public TrainEntry(string numeroTreno, string stazione, string ore, string binarioPrevisto,
            string binarioReale, string situazione, TrainEntryTypeEnum trainType)
        {
            _numeroTreno = numeroTreno;
            _stazione = stazione;
            _ore = ore;
            _binarioPrevisto = binarioPrevisto;
            _binarioReale = binarioReale;
            _situazione = situazione;
            _trainType = trainType;
        }

        public override string ToString()
        {
            return string.Format("Treno n.: {0}\n"+
                (_trainType == TrainEntryTypeEnum.Arrival ? "Da: " : 
                    (_trainType == TrainEntryTypeEnum.Departure ? "Per: " : 
                        "")) + "{1}\n" +
                        ""+
                "Delle ore: {2}\n" +
                "Binario previsto: {3}\n" +
                "Binario reale: {4}\n" +
                "Situazione: {5}\n\n",
                _numeroTreno,
                _stazione,
                _ore,
                _binarioPrevisto,
                _binarioReale,
                _situazione);
        }
    }
}

