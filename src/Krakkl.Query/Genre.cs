using System.Collections.Generic;
using Newtonsoft.Json;

namespace Krakkl.Query
{
    public class Genre : OrchestrateBase
    {
        public List<GenreObject> Genres { get; }

        public Genre()
        {
            Genres = new List<GenreObject>();
            var query = Orchestrate.Search("Genres", "*", 100, 0, "IsFiction:desc,Name:asc");

            foreach (var genre in query.Results)
            {
                GenreObject val = JsonConvert.DeserializeObject<GenreObject>(genre.Value.ToString());
                val.Key = genre.Path.Key;
                Genres.Add(val);
            }
        }
    }

    public class GenreObject
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public bool IsFiction { get; set; }
    }
}
