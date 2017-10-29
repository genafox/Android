using System;

namespace App.Domain.Exceptions
{
    public class DatabaseInitializationException : Exception
    {
        public DatabaseInitializationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}