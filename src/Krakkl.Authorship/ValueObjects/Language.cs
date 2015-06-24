namespace Krakkl.Authorship.ValueObjects
{
    public sealed class Language
    {
        public string Key { get; set; }
        public string Name { get; set; }

        public Language(string key, string name)
        {
            Key = key;
            Name = name;
        }
    }
}
