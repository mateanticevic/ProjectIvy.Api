namespace ProjectIvy.Api.Constants;

public static class IvyClient
{
    public static class Roles
    {
        public const string TrackingUser = "tracking_user";
    }

    public static class Resources
    {
        public static class Tracking
        {
            public const string Name = "tracking";
            
            public static class Scopes
            {
                public const string TrackingCreate = "tracking:create";
                public const string TrackingViewLast = "tracking:view_last";
            }
        }
    }
}