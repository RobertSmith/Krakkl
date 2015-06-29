using System.Collections.Generic;
using Krakkl.Query.Models;
using Newtonsoft.Json;

namespace Krakkl.Query
{
    public class GenreQueries : OrchestrateBase
    {
        public List<GenreModel> Genres { get; }

        public GenreQueries()
        {
            Genres = new List<GenreModel>();
            var query = Orchestrate.Search("Genres", "*", 100, 0, "IsFiction:desc,Name:asc");

            foreach (var genre in query.Results)
            {
                var val = JsonConvert.DeserializeObject<GenreModel>(genre.Value.ToString());
                val.Key = genre.Path.Key;
                Genres.Add(val);
            }
        }
    }
}
