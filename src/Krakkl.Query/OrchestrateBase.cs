using Microsoft.Framework.ConfigurationModel;

namespace Krakkl.Query
{
    public class OrchestrateBase
    {
        internal Orchestrate.Net.Orchestrate Orchestrate;
        internal readonly string _storageConnectionString;

        public OrchestrateBase()
        {
            var configuration = new Configuration().AddJsonFile("config.json");

            Orchestrate = new Orchestrate.Net.Orchestrate(configuration["Data:Orchestrate:ApiKey"]);
            _storageConnectionString = configuration["Data:AzureStorage:ConnectionString"];
        }
    }
}
