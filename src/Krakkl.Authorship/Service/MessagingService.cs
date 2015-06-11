using Microsoft.Framework.ConfigurationModel;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Krakkl.Authorship.Service
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

        public async void OnBookEventSendMessage(object sender, object e)
        {
            CloudQueueMessage message = new CloudQueueMessage(e.ToString());
            await _queue.AddMessageAsync(message);
        }
    }
}