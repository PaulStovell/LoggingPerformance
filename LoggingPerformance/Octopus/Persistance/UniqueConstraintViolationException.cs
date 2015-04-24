using System;

namespace LoggingPerformance.Octopus.Persistance
{
    public class UniqueConstraintViolationException : Exception
    {
        public UniqueConstraintViolationException(string message)
            : base(message)
        {

        }
    }
}