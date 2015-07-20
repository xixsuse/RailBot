using System;

namespace RailBot
{
    public class ResponseData : Data
    {

        public string Message
        {
            get;
            set;
        }

        public ResponseData(int updateID, int chatID):
            base(updateID, chatID)
        {
            
        }
    }
}

