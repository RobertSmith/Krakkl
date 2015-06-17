using System;

namespace Krakkl.Authorship.Repository
{
    public interface IBookAggregateRepository
    {
        T FindByKey<T>(Guid key);
        void Save(object state);
    }
}