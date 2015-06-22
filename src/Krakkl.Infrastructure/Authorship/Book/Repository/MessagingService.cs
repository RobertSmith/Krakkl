using System;
using System.Threading;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Krakkl.Infrastructure.Authorship.Book.Repository
{
    internal class MessagingService
    {
        private readonly CloudQueue _queue;

        public MessagingService()
        {
            var configuration = new Configuration().AddJsonFile("config.json");
            var storageConnectionString = configuration["Data:AzureStorage:ConnectionString"];

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            _queue = queueClient.GetQueueReference("authorship");
            _queue.CreateIfNotExistsAsync();
        }

        public async void SendBookEventMessage(object e)
        {
            var retryCount = 0;
            var maxTries = 5;

            while (true)
            {
                try
                {
                    CloudQueueMessage message = new CloudQueueMessage(e.ToString());
                    await _queue.AddMessageAsync(message);
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