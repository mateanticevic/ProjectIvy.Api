using System;

namespace ProjectIvy.BL.Exceptions
{
    public class ResourceExistsException : Exception
    {
        public ResourceExistsException(string resourceName)
        {
            ResourceName = resourceName;
        }

        public string ResourceName { get; set; }
    }
}
