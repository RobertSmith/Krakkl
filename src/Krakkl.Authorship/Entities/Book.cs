using System;
using System.Collections.Generic;
using Krakkl.Authorship.ValueObjects;

namespace Krakkl.Authorship.Entities
{
    public sealed class Book
    {
        public Guid Key { get; set; }
        public List<ValueObjects.Author> Authors { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string SeriesTitle { get; set; }
        public string SeriesVolume { get; set; }
        public Genre Genre { get; set; }
        public Language Language { get; set; }
        public string Synopsis { get; set; }
        public bool Published { get; set; }
        public bool Completed { get; set; }
        public bool Abandoned { get; set; }
        public long LastEventRefTime { get; set; }

        public Book()
        {
            Authors = new List<ValueObjects.Author>();
        }
    }
}
