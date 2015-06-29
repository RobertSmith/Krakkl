using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Krakkl.Authorship.Services;
using Krakkl.Cache;
using Krakkl.Models;
using Krakkl.Query;
using Krakkl.Query.Models;

namespace Krakkl.Services
{
    public class DeskService
    {
        private readonly BookService _service = new BookService(new Authorship.Infrastructure.BookAggregateRepository());

        public async Task<BookModel> GetBookByKey(string key)
        {
            var retryCount = 0;
            var maxTries = 5;

            var query = new BookQueries();

            while (true)
            {
                try
                {
                    var book = await query.GetBookAsync(key);
                    return book;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("404") && retryCount < maxTries)
                    {
                        retryCount++;
                        Thread.Sleep(1000);
                    }
                    else
                        throw;
                }
            }
        }

        public Guid StartANewBook(ApplicationUser user)
        {
            var languages = LanguagesCache.GetAll() as Dictionary<string, string>;

            var bookKey = _service.Start(new StartANewBookCommand
            {
                AuthorKey = Guid.Parse(user.Id),
                AuthorName = user.Pseudonym,
                LanguageKey = languages?.First(x => x.Key == user.EditorLanguage).Key,
                LanguageName = languages?.First(x => x.Key == user.EditorLanguage).Value
            });

            return bookKey;
        }

        public bool UpdateBookTitle(ApplicationUser user, string bookKey, string newTitle)
        {
            try
            {
                _service.Apply(new RetitleBookCommand
                {
                    AuthorKey = Guid.Parse(user.Id),
                    BookKey = Guid.Parse(bookKey),
                    Title = newTitle
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateBookSubTitle(ApplicationUser user, string bookKey, string newSubTitle)
        {
            try
            {
                _service.Apply(new ChangeBookSubtitleCommand
                {
                    AuthorKey = Guid.Parse(user.Id),
                    BookKey = Guid.Parse(bookKey),
                    SubTitle = newSubTitle
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateBookSeriesTitle(ApplicationUser user, string bookKey, string newSeriesTitle)
        {
            try
            {
                _service.Apply(new ChangeBookSeriesTitleCommand
                {
                    AuthorKey = Guid.Parse(user.Id),
                    BookKey = Guid.Parse(bookKey),
                    SeriesTitle = newSeriesTitle
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateBookSeriesVolume(ApplicationUser user, string bookKey, string newSeriesVolume)
        {
            try
            {
                _service.Apply(new ChangeBookSeriesVolumeCommand
                {
                    AuthorKey = Guid.Parse(user.Id),
                    BookKey = Guid.Parse(bookKey),
                    SeriesVolume = newSeriesVolume
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateBookGenre(ApplicationUser user, string bookKey, string genreKey)
        {
            try
            {
                var genres = GenreCache.GetAll() as List<GenreModel>;

                if (genres == null)
                    throw  new Exception("No genres? WTF?");

                _service.Apply(new ChangeBookGenreCommand
                {
                    AuthorKey = Guid.Parse(user.Id),
                    BookKey = Guid.Parse(bookKey),
                    GenreKey = genreKey,
                    GenreName = genres.Single(x => x.Key == genreKey).Name,
                    IsFiction = genres.Single(x => x.Key == genreKey).IsFiction
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateBookLanguage(ApplicationUser user, string bookKey, string languageKey)
        {
            try
            {
                var languages = LanguagesCache.GetAll() as Dictionary<string, string>;

                if (languages == null)
                    throw new Exception("No languages? WTF?");

                _service.Apply(new ChangeBookEditorLanguageCommand
                {
                    AuthorKey = Guid.Parse(user.Id),
                    BookKey = Guid.Parse(bookKey),
                    LanguageKey = languageKey,
                    LanguageName = languages.Single(x => x.Key == languageKey).Value
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateSynopsis(ApplicationUser user, string bookKey, string synopsis)
        {
            try
            {
                var languages = LanguagesCache.GetAll() as Dictionary<string, string>;

                if (languages == null)
                    throw new Exception("No languages? WTF?");

                _service.Apply(new ChangeBookSynopsisCommand
                {
                    AuthorKey = Guid.Parse(user.Id),
                    BookKey = Guid.Parse(bookKey),
                    Synopsis = synopsis
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateBookPublished(ApplicationUser user, string bookKey)
        {
            try
            {
                var languages = LanguagesCache.GetAll() as Dictionary<string, string>;

                if (languages == null)
                    throw new Exception("No languages? WTF?");

                _service.Apply(new PublishBookCommand
                {
                    AuthorKey = Guid.Parse(user.Id),
                    BookKey = Guid.Parse(bookKey)
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateBookCompleted(ApplicationUser user, string bookKey, bool completed)
        {
            try
            {
                var languages = LanguagesCache.GetAll() as Dictionary<string, string>;

                if (languages == null)
                    throw new Exception("No languages? WTF?");

                if (completed)
                    _service.Apply(new CompleteBookCommand
                    {
                        AuthorKey = Guid.Parse(user.Id),
                        BookKey = Guid.Parse(bookKey)
                    });
                else
                    _service.Apply(new SetBookInProgressCommand
                    {
                        AuthorKey = Guid.Parse(user.Id),
                        BookKey = Guid.Parse(bookKey)
                    });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateBookAbandoned(ApplicationUser user, string bookKey, bool abandoned)
        {
            try
            {
                var languages = LanguagesCache.GetAll() as Dictionary<string, string>;

                if (languages == null)
                    throw new Exception("No languages? WTF?");

                if (abandoned)
                    _service.Apply(new AbandonBookCommand
                    {
                        AuthorKey = Guid.Parse(user.Id),
                        BookKey = Guid.Parse(bookKey)
                    });
                else
                    _service.Apply(new ReviveBookCommand
                    {
                        AuthorKey = Guid.Parse(user.Id),
                        BookKey = Guid.Parse(bookKey)
                    });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateBookCoverArt(ApplicationUser user, string bookKey, Stream coverArt)
        {
            try
            {
                _service.Apply(new SetBookCoverArtCommand
                {
                    AuthorKey = Guid.Parse(user.Id),
                    BookKey = Guid.Parse(bookKey),
                    CoverArt = coverArt
                });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}