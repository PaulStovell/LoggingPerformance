namespace LoggingPerformance.TestHarness
{
    public class TestDeployment
    {
        private readonly Log log;

        public TestDeployment(Log log)
        {
            this.log = log;
        }

        public void Deploy()
        {
            log.Info("Begin deployment XYZ");

            for (var i = 0; i < 2; i++)
            {
                DeployToMachine(i);
            }
        }

        private void DeployToMachine(int machineId)
        {
            using (var child = log.Indent())
            {
                child.Info("Deploying to machine " + machineId);

                child.Info("Uploading package <xyz> to <abc>");
                for (var j = 0; j < 100; j++)
                {
                    child.Progress(j, "Uploading...");
                }

                child.Info("Running script \"foo\"");

                for (var i = 0; i < 100000; i++)
                {
                    child.Info("Script output line " + i);
                }

                child.Info("Do more work");
                for (var j = 0; j < 70; j++)
                {
                    child.Progress(j, "Uploading again");
                }

                child.Error("Oops!");
            }
        }
    }
}