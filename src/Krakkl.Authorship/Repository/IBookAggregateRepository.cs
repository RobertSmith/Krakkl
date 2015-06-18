using System;
using Krakkl.Authorship.Aggregates;

namespace Krakkl.Authorship.Repository
{
    public interface IBookAggregateRepository
    {
        BookAggregate FindByKey<T>(Guid key);
        void Save(BookAggregate aggregate);
    }
}