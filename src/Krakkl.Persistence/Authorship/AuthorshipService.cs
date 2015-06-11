using System;
using System.Threading;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Krakkl.Persistence.Authorship
{
    public class AuthorshipService
    {
        private readonly CloudQueue _queue;
        private int _idleCount;
        private int _sleepLength;

        public AuthorshipService()
        {
            var configuration = new Configuration().AddJsonFile("config.json");
            var storageConnectionString = configuration["Data:AzureStorage:ConnectionString"];

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            _queue = queueClient.GetQueueReference("authorship");

            Run();
        }

        private async void Run()
        {
            await _queue.CreateIfNotExistsAsync();
            var processor = new ProcessAuthorshipMessage();

            while (true)
            {
                var message = await _queue.GetMessageAsync();

                if (message != null)
                {
                    try
                    {
                        processor.ProcessMessage(message);
                        await _queue.DeleteMessageAsync(message);
                    }
                    catch (Exception)
                    {
                    }

                    _idleCount = 0;
                }
                else
                    _idleCount++;

                CalcSleepLength();
                Thread.Sleep(_sleepLength);
            }
        }

        private void CalcSleepLength()
        {
            _sleepLength = (_idleCount + 1) * 1000;

            if (_sleepLength > 30000)
                _sleepLength = 30000;
        }
    }
}