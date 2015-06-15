using System;
using System.Threading;
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
            var retryCount = 0;
            var maxTries = 5;

            while (true)
            {
                try
                {
                    _orchestrate.Put(Definitions.BookEventsCollection, message.Id, message.AsString);
                    break;
                }
                catch (Exception)
                {
                    if (retryCount < maxTries)
                    {
                        retryCount++;
                        Thread.Sleep(retryCount * 1000);
                    }
                    else
                        throw;
                }
            }
        }
    }
}