namespace Krakkl.Authorship.Models
{
    internal sealed class GenreModel
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public bool IsFiction { get; set; }

        public GenreModel(string key, string name, bool isFiction)
        {
            Key = key;
            Name = name;
            IsFiction = isFiction;
        }
    }
}
