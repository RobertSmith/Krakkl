using Microsoft.Framework.ConfigurationModel;

namespace Krakkl.Query
{
    public class OrchestrateBase
    {
        internal Orchestrate.Net.Orchestrate Orchestrate;

        public OrchestrateBase()
        {
            var configuration = new Configuration().AddJsonFile("config.json");

            Orchestrate = new Orchestrate.Net.Orchestrate(configuration["Data:Orchestrate:ApiKey"]);
        }
    }
}
