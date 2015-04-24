using System;
using System.Data;

namespace LoggingPerformance.Octopus.Persistance
{
    public interface IProjectionMapper
    {
        TResult Map<TResult>(string prefix);
        void Read(Action<IDataReader> callback);
    }
}