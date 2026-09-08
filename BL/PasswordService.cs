using Microsoft.AspNetCore.Identity;

namespace CKM_ManagementSystem.BL
{
    public class PasswordService
    {
        private readonly object _hashedUser = new();
        private readonly PasswordHasher<object> _hasher = new();

        public string HashPassword(string password)
        {
            return _hasher.HashPassword(_hashedUser, password);
        }
        public bool VerifyPassword(string hashedPassword, string providedPassword)
        {
            var result = _hasher.VerifyHashedPassword(_hashedUser, hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
