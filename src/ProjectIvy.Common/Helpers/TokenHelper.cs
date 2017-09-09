using System;

namespace ProjectIvy.Common.Helpers
{
    public class TokenHelper
    {
        public static string Generate()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
    }
}
