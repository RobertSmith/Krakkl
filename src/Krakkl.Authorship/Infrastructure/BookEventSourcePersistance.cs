using Krakkl.Authorship.Core;
using Krakkl.Authorship.Models;
using Microsoft.Framework.ConfigurationModel;

namespace Krakkl.Authorship.Infrastructure
{
    internal class BookEventSourcePersistance
    {
        private readonly Orchestrate.Net.Orchestrate _orchestrate;

        public BookEventSourcePersistance()
        {
            var configuration = new Configuration().AddJsonFile("config.json");

            _orchestrate = new Orchestrate.Net.Orchestrate(configuration["Data:Orchestrate:ApiKey"]);
        }

        public void OnBookCreated(object sender, BookCreatedEventArgs e)
        {
            var author = new AuthorModel(e.AuthorKey, e.AuthorName);
            var language = new LanguageModel(e.LanguageKey, e.LanguageName);

            var state = new BookState
            {
                Key = e.BookKey,
                Language = language,
                CreatedAt = e.CreatedAt,
                CreatedBy = e.AuthorKey,
                Published = false,
                Abandoned = false,
                Completed = false,
                DMCA = false,
                Title = e.Title
            };

            state.Authors.Add(author);

            _orchestrate.Put("Books", e.BookKey.ToString(), state);
        }

        public void OnBookRetitled(object sender, BookRetitledEventArgs e)
        {
            var x = e.NewTitle;
        }
    }
}
