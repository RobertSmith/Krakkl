using System.Collections.Generic;
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
                LanguageObject val = JsonConvert.DeserializeObject<LanguageObject>(lang.Value.ToString());
                Languages.Add(lang.Path.Key, val.Name);
            }
        }
    }

    internal class LanguageObject
    {
        public string Name { get; set; }
    }
}
