namespace Krakkl.Models
{
    public class EditBookMetaModel
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string SeriesTitle { get; set; }
        public string SeriesVolume { get; set; }
        public string GenreKey { get; set; }
        public string LanguageKey { get; set; }
        public string CoverArt { get; set; }
        public string Synopsis { get; set; }
        public bool Public { get; set; }
        public bool Complete { get; set; }
        public bool Abandoned { get; set; }
    }
}
