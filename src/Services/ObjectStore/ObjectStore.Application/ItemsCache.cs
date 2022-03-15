using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DrifterApps.Holefeeder.ObjectStore.Application;

[Serializable]
public class ItemsCache : Dictionary<string, object>
{
    public ItemsCache()
    {
    }

    protected ItemsCache(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
