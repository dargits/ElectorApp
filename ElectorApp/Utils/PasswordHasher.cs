// File: Utils/PasswordHasher.cs

using BCrypt.Net; // Cần phải có using này

namespace ElectorApp.Utils // Namespace của lớp bạn
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            return BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Verify(password, hashedPassword);
        }
    }
}