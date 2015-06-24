using System;

namespace Krakkl.Authorship.Services
{
    public class StartANewBookCommand
    {
        public Guid AuthorKey { get; set; }
        public string AuthorName { get; set; }
        public string LanguageKey { get; set; }
        public string LanguageName { get; set; }
    }

    public class AddAuthorToBookCommand
    {
        public Guid BookKey { get; set; }
        public Guid AuthorKey { get; set; }
        public Guid NewAuthorKey { get; set; }
        public string NewAuthorName { get; set; }
    }

    public class RemoveAuthorFromBookCommand
    {
        public Guid BookKey { get; set; }
        public Guid AuthorKey { get; set; }
        public Guid RemoveAuthorKey { get; set; }
        public string RemoveAuthorName { get; set; }
    }

    public class RetitleBookCommand
    {
        public Guid BookKey { get; set; }
        public Guid AuthorKey { get; set; }
        public string Title { get; set; }
    }

    public class ChangeBookSubtitleCommand
    {
        public Guid BookKey { get; set; }
        public Guid AuthorKey { get; set; }
        public string SubTitle { get; set; }
    }

    public class ChangeBookSeriesTitleCommand
    {
        public Guid BookKey { get; set; }
        public Guid AuthorKey { get; set; }
        public string SeriesTitle { get; set; }
    }

    public class ChangeBookSeriesVolumeCommand
    {
        public Guid BookKey { get; set; }
        public Guid AuthorKey { get; set; }
        public string SeriesVolume { get; set; }
    }

    public class ChangeBookGenreCommand
    {
        public Guid BookKey { get; set; }
        public Guid AuthorKey { get; set; }
        public string GenreKey { get; set; }
        public string GenreName { get; set; }
        public bool IsFiction { get; set; }
    }

    public class ChangeBookEditorLanguageCommand
    {
        public Guid BookKey { get; set; }
        public Guid AuthorKey { get; set; }
        public string LanguageKey { get; set; }
        public string LanguageName { get; set; }
    }

    public class ChangeBookSynopsisCommand
    {
        public Guid BookKey { get; set; }
        public Guid AuthorKey { get; set; }
        public string Synopsis { get; set; }
    }

    public class CompleteBookCommand
    {
        public Guid BookKey { get; set; }
        public Guid AuthorKey { get; set; }
    }

    public class SetBookInProgressCommand
    {
        public Guid BookKey { get; set; }
        public Guid AuthorKey { get; set; }
    }

    public class AbandonBookCommand
    {
        public Guid BookKey { get; set; }
        public Guid AuthorKey { get; set; }
    }

    public class ReviveBookCommand
    {
        public Guid BookKey { get; set; }
        public Guid AuthorKey { get; set; }
    }

    public class PublishBookCommand
    {
        public Guid BookKey { get; set; }
        public Guid AuthorKey { get; set; }
    }
}