namespace LoggingPerformance.Octopus.Persistance
{
    public interface IKeyAllocator
    {
        int NextId(string tableName);
    }
}