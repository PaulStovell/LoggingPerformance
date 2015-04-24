using System.Data;

namespace LoggingPerformance.Octopus.Persistance
{
    public interface IRelationalStore
    {
        string ConnectionString { get; }
        IRelationalTransaction BeginTransaction();
        IRelationalTransaction BeginTransaction(IsolationLevel isolationLevel);
    }
}