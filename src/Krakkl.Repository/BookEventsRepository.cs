using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Framework.ConfigurationModel;
using Newtonsoft.Json;

namespace Krakkl.Repository
{
    public class BookEventsRepository : IBookEventsRepository
    {
        private readonly Orchestrate.Net.Orchestrate _orchestrate;
        private const string BookEventsCollection = "BookEvents";

        public BookEventsRepository()
        {
            var configuration = new Configuration().AddJsonFile("config.json");

            _orchestrate = new Orchestrate.Net.Orchestrate(configuration["Data:Orchestrate:ApiKey"]);
        }

        public T FindByKey<T>(Guid key)
        {
            var bookEvents = _orchestrate.Search(BookEventsCollection, "BookKey = " + key, 100, 0, "TimeStamp:asc");
            return JsonConvert.DeserializeObject<T>(bookEvents.Results.ToString());
        }

        public void Save(object e)
        {
        }
    }
}