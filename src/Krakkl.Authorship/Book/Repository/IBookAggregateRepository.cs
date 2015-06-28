using System;
using System.IO;
using Krakkl.Authorship.Book.Aggregate;

namespace Krakkl.Authorship.Book.Repository
{
    public interface IBookAggregateRepository
    {
        BookAggregate FindByKey<T>(Guid key);
        void Save(BookAggregate aggregate);
        void SaveCoverArt(BookAggregate aggregate, Guid coverArtKey, Stream coverArt);
    }
}