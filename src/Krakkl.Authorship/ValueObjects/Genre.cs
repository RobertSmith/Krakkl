namespace Krakkl.Authorship.ValueObjects
{
    public sealed class Genre
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public bool IsFiction { get; set; }

        public Genre(string key, string name, bool isFiction)
        {
            Key = key;
            Name = name;
            IsFiction = isFiction;
        }
    }
}
