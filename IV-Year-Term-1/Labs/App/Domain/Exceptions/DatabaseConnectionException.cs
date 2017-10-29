using System;

namespace App.Domain.Exceptions
{
    public class DatabaseConnectionException : Exception
    {
        public DatabaseConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}