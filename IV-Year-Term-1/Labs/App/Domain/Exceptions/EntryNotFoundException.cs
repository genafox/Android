using System;

namespace App.Domain.Exceptions
{
    public class EntryNotFoundException : Exception
    {
        public EntryNotFoundException(string message) : base(message)
        {
        }
    }
}