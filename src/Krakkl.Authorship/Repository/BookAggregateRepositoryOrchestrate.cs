using System;
using System.Threading.Tasks;
using Krakkl.Authorship.Core;
using Krakkl.Authorship.Infrastructure;
using Microsoft.Framework.ConfigurationModel;
using Newtonsoft.Json;

namespace Krakkl.Authorship.Repository
{
    internal class BookAggregateRepositoryOrchestrate : IBookAggregateRepository
    {
        private readonly Orchestrate.Net.Orchestrate _orchestrate;

        public BookAggregateRepositoryOrchestrate()
        {
            var configuration = new Configuration().AddJsonFile("config.json");

            _orchestrate = new Orchestrate.Net.Orchestrate(configuration["Data:Orchestrate:ApiKey"]);
        }

        public BookState FindByKey(Guid key)
        {
            var result = _orchestrate.Get(Definitions.BookCollection, key.ToString());
            return JsonConvert.DeserializeObject<BookState>(result.Value.ToString());
        }

        public async Task<BookState> FindByKeyAsync(Guid key)
        {
            var result = await _orchestrate.GetAsync(Definitions.BookCollection, key.ToString());
            return JsonConvert.DeserializeObject<BookState>(result.Value.ToString());
        }
    }
}
