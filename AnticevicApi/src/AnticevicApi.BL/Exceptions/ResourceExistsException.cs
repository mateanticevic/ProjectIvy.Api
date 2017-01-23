using System;

namespace AnticevicApi.BL.Exceptions
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
