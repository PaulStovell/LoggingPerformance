namespace LoggingPerformance.Octopus.Persistance
{
    public interface IPropertyReaderWriter<TCast>
    {
        TCast Read(object target);
        void Write(object target, TCast value);
    }
}