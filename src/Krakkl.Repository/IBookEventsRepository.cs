using System;

namespace Krakkl.Repository
{
    public interface IBookEventsRepository
    {
        T FindByKey<T>(Guid key);
        void Save(object state);
    }
}