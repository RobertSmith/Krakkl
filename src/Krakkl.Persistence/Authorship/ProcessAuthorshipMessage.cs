using Microsoft.Framework.ConfigurationModel;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Krakkl.Persistence.Authorship
{
    internal class ProcessAuthorshipMessage
    {
        private readonly Orchestrate.Net.Orchestrate _orchestrate;

        public ProcessAuthorshipMessage()
        {
            var configuration = new Configuration().AddJsonFile("config.json");
            _orchestrate = new Orchestrate.Net.Orchestrate(configuration["Data:Orchestrate:ApiKey"]);
        }

        public void ProcessEvent(CloudQueueMessage message)
        {
            _orchestrate.Put(Definitions.BookEventsCollection, message.Id, message.AsString);
        }
    }
}