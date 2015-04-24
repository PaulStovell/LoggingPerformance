using System;

namespace LoggingPerformance.Octopus.Persistance
{
    public class StringTooLongException : Exception
    {
        public StringTooLongException(string message) : base(message)
        {
            
        }
    }
}