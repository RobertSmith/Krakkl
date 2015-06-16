using System;
using System.Threading.Tasks;

namespace Krakkl.Authorship.Repository
{
    internal interface IBookAggregateRepository
    {
        BookState FindByKey(Guid key);
        Task<BookState> FindByKeyAsync(Guid key);
    }
}
