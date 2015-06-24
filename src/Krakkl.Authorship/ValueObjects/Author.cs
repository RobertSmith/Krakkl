using System;

namespace Krakkl.Authorship.ValueObjects
{
    public sealed class Author
    {
        public Guid Key { get; set; }
        public string Name { get; set; }

        public Author(Guid key, string name)
        {
            Key = key;
            Name = name;
        }
    }
}
