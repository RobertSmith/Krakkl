using System.Collections.Generic;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

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

        internal void ProcessMessage(CloudQueueMessage message)
        {
            var eventModel = JsonConvert.DeserializeObject<AuthorshipEventModel>(message.AsString);

            switch (eventModel.EventType)
            {
                case "BookCreated":
                    BookCreatedHandler(eventModel);
                    break;

                case "AuthorAddedToBook":
                    AuthorAddedHandler(eventModel);
                    break;
            }
        }

        private void BookCreatedHandler(AuthorshipEventModel eventModel)
        {
            var language = new LanguageModel { Key = eventModel.LanguageKey, Name = eventModel.LanguageName };

            var state = new BookState
            {
                Key = eventModel.BookKey,
                Authors = eventModel.ValidAuthors,
                Language = language,
                CreatedAt = eventModel.CreatedAt,
                CreatedBy = eventModel.AddedAuthor.Key,
                Title = eventModel.Title
            };

            _orchestrate.Put(Definitions.BookCollection, eventModel.BookKey.ToString(), state);
            _orchestrate.PutEvent(Definitions.BookCollection, eventModel.BookKey.ToString(), "update", eventModel.CreatedAt, eventModel);
        }

        private void AuthorAddedHandler(AuthorshipEventModel eventModel)
        {
            var patchItems = new List<object>
            {
                new PatchItemObject { Op = "replace", Path = "/Authors", Value = eventModel.ValidAuthors },
                new PatchItemString { Op = "add", Path = "/UpdatedBy", Value = eventModel.UpdatedBy.ToString() },
                new PatchItemDate { Op = "add", Path = "/UpdatedAt", Value = eventModel.UpdatedAt }
            };

            PatchBook(patchItems, eventModel.BookKey.ToString());
            _orchestrate.PutEvent(Definitions.BookCollection, eventModel.BookKey.ToString(), "update", eventModel.UpdatedAt, eventModel);
        }

        private void PatchBook(List<object> patchItems, string bookKey)
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var json = JsonConvert.SerializeObject(patchItems, settings);
        
            _orchestrate.Patch(Definitions.BookCollection, bookKey, json);
        }
    }
}