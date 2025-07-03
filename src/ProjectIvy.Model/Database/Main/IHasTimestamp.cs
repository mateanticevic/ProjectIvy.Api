using System;

namespace ProjectIvy.Model.Database.Main;

public interface IHasTimestamp
{
    DateTime Timestamp { get; set; }
}
