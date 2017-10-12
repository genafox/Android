using System;

namespace App.Domain.Exceptions
{
    public class EntryAlreadyExistsException : Exception
    {
        public EntryAlreadyExistsException(string message) : base(message)
        {
        }
    }
}