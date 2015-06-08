using System;
using System.Threading.Tasks;
using Krakkl.Authorship.Core;

namespace Krakkl.Authorship.Repository
{
    internal interface IBookAggregateRepository
    {
        BookState FindByKey(Guid key);
        Task<BookState> FindByKeyAsync(Guid key);
    }
}
