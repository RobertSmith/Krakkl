using System;
using System.IO;

namespace Krakkl.Authorship.Services
{
    public class BookCommandBase
    {
        public Guid BookKey { get; set; }
        public Guid AuthorKey { get; set; }
    }

    public class StartANewBookCommand
    {
        public Guid AuthorKey { get; set; }
        public string AuthorName { get; set; }
        public string LanguageKey { get; set; }
        public string LanguageName { get; set; }
    }

    public class AddAuthorToBookCommand : BookCommandBase
    {
        public Guid NewAuthorKey { get; set; }
        public string NewAuthorName { get; set; }
    }

    public class RemoveAuthorFromBookCommand : BookCommandBase
    {
        public Guid RemoveAuthorKey { get; set; }
        public string RemoveAuthorName { get; set; }
    }

    public class RetitleBookCommand : BookCommandBase
    {
        public string Title { get; set; }
    }

    public class ChangeBookSubtitleCommand : BookCommandBase
    {
        public string SubTitle { get; set; }
    }

    public class ChangeBookSeriesTitleCommand : BookCommandBase
    {
        public string SeriesTitle { get; set; }
    }

    public class ChangeBookSeriesVolumeCommand : BookCommandBase
    {
        public string SeriesVolume { get; set; }
    }

    public class ChangeBookGenreCommand : BookCommandBase
    {
        public string GenreKey { get; set; }
        public string GenreName { get; set; }
        public bool IsFiction { get; set; }
    }

    public class ChangeBookEditorLanguageCommand : BookCommandBase
    {
        public string LanguageKey { get; set; }
        public string LanguageName { get; set; }
    }

    public class ChangeBookSynopsisCommand : BookCommandBase
    {
        public string Synopsis { get; set; }
    }

    public class CompleteBookCommand : BookCommandBase
    {
    }

    public class AbandonBookCommand : BookCommandBase
    {
    }

    public class ReviveBookCommand : BookCommandBase
    {
    }

    public class PublishBookCommand : BookCommandBase
    {
    }

    public class SetBookCoverArtCommand : BookCommandBase
    {
        public Stream CoverArt { get; set; }
    }
}