using System;

namespace RailBot
{
    public abstract class Data
    {
        public int UpdateID { get; protected set;}
        public int ChatID { get; protected set;}

        public Data (int updateID, int chatID)
        {
            UpdateID = updateID;
            ChatID = chatID;
        }

        #region Error

        public virtual bool IsError {
            get{ return (ErrorMessage == null ? false : true); }
        }

        protected string _errorMessage = null;

        public string ErrorMessage {
            get{ return _errorMessage; }
            set { _errorMessage = value; }
        }

        #endregion
    }
}

