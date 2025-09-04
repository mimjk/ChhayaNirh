using BCryptNet = BCrypt.Net.BCrypt;

namespace ChhayaNirh.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            return BCryptNet.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            return BCryptNet.Verify(password, hash);
        }
    }
}