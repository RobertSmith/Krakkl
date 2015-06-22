using System;

namespace Krakkl.Authorship.Book.Models
{
    public sealed class AuthorModel
    {
        public Guid Key { get; set; }
        public string Name { get; set; }

        public AuthorModel(Guid key, string name)
        {
            Key = key;
            Name = name;
        }
    }
}
