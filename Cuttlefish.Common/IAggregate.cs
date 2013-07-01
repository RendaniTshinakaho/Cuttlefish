using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Cuttlefish.Common
{
    public interface IAggregate
    {
        [BsonId]
        Guid AggregateIdentity { get; set; }

        string TypeName { get; }
    }
}