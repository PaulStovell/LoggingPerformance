using LoggingPerformance.Octopus;

namespace LoggingPerformance.TestHarness
{
    public abstract class Approach
    {
        protected Approach(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public abstract IServerLogStorage GetStorage();
        public abstract long GetDiskImpact(string id);
        public abstract void CountAnyAdditionalWork(string id);
        public abstract void TransitionToDurableStorage(string id);

        public override string ToString()
        {
            return Name;
        }
    }
}
