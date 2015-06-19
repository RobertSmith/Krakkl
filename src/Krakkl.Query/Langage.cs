using System.Collections.Generic;
using Krakkl.Query.Models;
using Newtonsoft.Json;

namespace Krakkl.Query
{
    public class Language : OrchestrateBase
    {
        public Dictionary<string, string> Languages { get; }

        public Language()
        {
            Languages = new Dictionary<string, string>();
            var query = Orchestrate.Search("Languages", "*", 100, 0, "Name:asc");

            foreach (var lang in query.Results)
            {
                var val = JsonConvert.DeserializeObject<LanguageModel>(lang.Value.ToString());
                Languages.Add(lang.Path.Key, val.Name);
            }
        }
    }
}
