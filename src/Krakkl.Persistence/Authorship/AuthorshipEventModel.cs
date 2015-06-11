using System;
using System.Collections.Generic;

namespace Krakkl.Persistence.Authorship
{
    internal sealed class AuthorshipEventModel
    {
        public Guid BookKey { get; set; }
        public string EventSource { get; set; }
        public string EventType { get; set; }
        public string LanguageKey { get; set; }
        public string LanguageName { get; set; }
        public string Title { get; set; }
        public AuthorModel AddedAuthor { get; set; }
        public AuthorModel RemovedAuthor { get; set; }
        public List<AuthorModel> ValidAuthors { get; set; }
        public string OldTitle { get; set; }
        public string NewTitle { get; set; }
        public string OldSubTitle { get; set; }
        public string NewSubTitle { get; set; }
        public string OldSeriesTitle { get; set; }
        public string NewSeriesTitle { get; set; }
        public string OldSeriesVolume { get; set; }
        public string NewSeriesVolume { get; set; }
        public GenreModel OldGenre { get; set; }
        public GenreModel NewGenre { get; set; }
        public LanguageModel OldLanguage { get; set; }
        public LanguageModel NewLanguage { get; set; }
        public string OldSynopsis { get; set; }
        public string NewSynopsis { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UpdatedBy { get; set; }
    }

    internal sealed class AuthorModel
    {
        public Guid Key { get; set; }
        public string Name { get; set; }
    }

    internal sealed class GenreModel
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public bool IsFiction { get; set; }
    }

    internal sealed class LanguageModel
    {
        public string Key { get; set; }
        public string Name { get; set; }
    }

    internal sealed class ModerationIssueModel
    {
        public string Field { get; set; }
        public string Reason { get; set; }
    }
}