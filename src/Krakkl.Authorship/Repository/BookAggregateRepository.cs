using System;
using System.Linq;
using Krakkl.Authorship.Aggregates;
using Microsoft.Framework.ConfigurationModel;
using Newtonsoft.Json;

namespace Krakkl.Authorship.Repository
{
    public class BookAggregateRepository : IBookAggregateRepository
    {
        private readonly Orchestrate.Net.Orchestrate _orchestrate;
        private const string BookEventsCollection = "BookEvents";

        public BookAggregateRepository()
        {
            var configuration = new Configuration().AddJsonFile("config.json");

            _orchestrate = new Orchestrate.Net.Orchestrate(configuration["Data:Orchestrate:ApiKey"]);
        }

        public BookAggregate FindByKey<T>(Guid key)
        {
            var aggregate = (BookAggregate)BookAggregateCache.Get(key);

            if (aggregate == null)
            {
                var searchResults = _orchestrate.Search(BookEventsCollection, "BookKey = " + key, 50, 0, "TimeStamp:asc").Results.ToList();

                if (!searchResults.Any())
                    throw new Exception("Book not found");

                var events = searchResults.Select(result => JsonConvert.DeserializeObject(result.Value.ToString())).ToList();
                var newAggregate = new BookAggregate(events);

                return newAggregate;
            }

            return aggregate;
        }

        public void Save(BookAggregate aggregate)
        {
            BookAggregateCache.UpdateItem(aggregate.Key, aggregate);
        }
    }
}