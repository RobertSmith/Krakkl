using System;
using System.Collections.Generic;
using Krakkl.Authorship.Book.Models;

namespace Krakkl.Authorship.Book.Aggregates
{
    public sealed class BookState
    {
        public Guid Key { get; set; }
        public List<AuthorModel> Authors { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string SeriesTitle { get; set; }
        public string SeriesVolume { get; set; }
        public GenreModel Genre { get; set; }
        public LanguageModel Language { get; set; }
//        public string CoverArt { get; set; }
        public string Synopsis { get; set; }
        public bool Published { get; set; }
        public bool Completed { get; set; }
        public bool Abandoned { get; set; }
//        public bool DMCA { get; set; }
//        public List<ModerationIssueModel> ModerationIssues { get; set; }
//        public DateTime CreatedAt { get; set; }
//        public Guid CreatedBy { get; set; }
//        public DateTime? UpdatedAt { get; set; }
//        public Guid? UpdatedBy { get; set; }
        public long LastEventRefTime { get; set; }

        public BookState()
        {
            Authors = new List<AuthorModel>();
//            ModerationIssues = new List<ModerationIssueModel>();
        }
    }
}
