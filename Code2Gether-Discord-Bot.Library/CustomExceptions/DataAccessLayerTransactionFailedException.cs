using System;

namespace Code2Gether_Discord_Bot.Library.CustomExceptions
{
    public class DataAccessLayerTransactionFailedException : Exception
    {
        public DataAccessLayerTransactionFailedException(string message) : base(message)
        {
        }
    }
}
