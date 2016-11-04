using System;

namespace AnticevicApi.Model.Database.Main
{
    public interface IHasTimestamp
    {
        DateTime Timestamp { get; set; }
    }
}
