namespace AnticevicApi.Common.Helpers
{
    public class PasswordHelper
    {
        public static bool IsValid(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        public static string GetHash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
