using System;

namespace ProjectIvy.Model.Database.Main
{
    public interface IHasCreatedModified
    {
        DateTime Created { get; set; }

        DateTime Modified { get; set; }
    }
}
