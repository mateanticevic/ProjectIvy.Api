using System;
namespace ProjectIvy.Common.Helpers;

public static class IdHelper
{
    public static string Generate() => Guid.NewGuid().ToString().Substring(0, 6);
}
