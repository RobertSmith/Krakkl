using System;
using Krakkl.Authorship.Book.Aggregates;

namespace Krakkl.Authorship.Book.Repository
{
    public interface IBookAggregateRepository
    {
        BookAggregate FindByKey<T>(Guid key);
        void Save(BookAggregate aggregate);
    }
}