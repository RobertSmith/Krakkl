namespace Krakkl.Authorship.Book.Models
{
    public sealed class LanguageModel
    {
        public string Key { get; set; }
        public string Name { get; set; }

        public LanguageModel(string key, string name)
        {
            Key = key;
            Name = name;
        }
    }
}
