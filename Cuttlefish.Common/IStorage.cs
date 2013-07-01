using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Cuttlefish.Common
{
    public interface IStorage
    {
        void SaveOrUpdate<T>(T item) where T : IAggregate;
        T FetchById<T>(Guid id) where T : IAggregate;
        IEnumerable<T> SearchFor<T>(Expression<Func<T, bool>> predicate) where T : IAggregate;
    }
}