using System;

namespace Krakkl.Authorship.Models
{
    internal sealed class AuthorModel
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
