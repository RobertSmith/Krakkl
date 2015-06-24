using System;
using System.Threading.Tasks;
using Krakkl.Authorship.Book.Aggregate;

namespace Krakkl.Authorship.Book.Repository
{
    public interface IBookAggregateRepository
    {
        Task<BookAggregate> FindByKey<T>(Guid key);
        void Save(BookAggregate aggregate);
    }
}