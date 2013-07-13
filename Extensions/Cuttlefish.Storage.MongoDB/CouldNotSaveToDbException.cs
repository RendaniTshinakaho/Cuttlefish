using System;

namespace Cuttlefish.Storage.MongoDB
{
    [Serializable]
    public class CouldNotSaveToDbException : Exception
    {
        public CouldNotSaveToDbException(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; private set; }
    }
}