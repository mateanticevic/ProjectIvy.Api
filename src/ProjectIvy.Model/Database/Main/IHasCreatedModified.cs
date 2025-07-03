using System;

namespace ProjectIvy.Model.Database.Main;

public interface IHasCreatedModified : IHasCreated
{
    DateTime Modified { get; set; }
}
