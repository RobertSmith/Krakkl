using System;
using Microsoft.Framework.ConfigurationModel;
using Newtonsoft.Json;

namespace Krakkl.Authorship.Repository
{
    //TODO: THIS NEEDS TO GO THROUGH THE QUERY SIDE
    public class BookAggregateRepositoryOrchestrate : IBookAggregateRepository
    {
        private readonly Orchestrate.Net.Orchestrate _orchestrate;

        public BookAggregateRepositoryOrchestrate()
        {
            var configuration = new Configuration().AddJsonFile("config.json");

            _orchestrate = new Orchestrate.Net.Orchestrate(configuration["Data:Orchestrate:ApiKey"]);
        }

        public T FindByKey<T>(Guid key)
        {
            var result = _orchestrate.Get(Definitions.BookCollection, key.ToString());
            return JsonConvert.DeserializeObject<T>(result.Value.ToString());
        }

        public void Save(object state)
        { }
    }
}
