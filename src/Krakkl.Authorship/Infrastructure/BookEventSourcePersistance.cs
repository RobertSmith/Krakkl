using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Krakkl.Authorship.Core;

namespace Krakkl.Authorship.Infrastructure
{
    internal class BookEventSourcePersistance
    {
        public void OnBookCreated(object sender, BookCreatedEventArgs e)
        {
            var x = e.BookKey;
        }
    }
}
