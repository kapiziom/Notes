using System.Security.Cryptography;
using System.Text;
using Notes.Services.Identities.Exceptions;
using Notes.Services.Identities.Services;

namespace Notes.WebAPI.Modules.Identities
{
    public class PasswordService : IPasswordService
    {
        public readonly bool IsDigitRequired;
        public readonly bool IsUppercaseLetterRequired;
        public readonly bool IsLowercaseLetterRequired;
        public readonly bool IsSymbolRequired;
        public readonly int MinimumPasswordLength;
        public readonly int MaximumPasswordLength;
        
        public PasswordService(
            bool isDigitRequired = true,
            bool isUppercaseLetterRequired = false,
            bool isLowercaseLetterRequired = true,
            bool isSymbolRequired = false,
            int minimumPasswordLength = 8,
            int maximumPasswordLength = 64)
        {
            this.IsDigitRequired = isDigitRequired;
            this.IsUppercaseLetterRequired = isUppercaseLetterRequired;
            this.IsLowercaseLetterRequired = isLowercaseLetterRequired;
            this.IsSymbolRequired = isSymbolRequired;
            this.MinimumPasswordLength = minimumPasswordLength;
            this.MaximumPasswordLength = maximumPasswordLength;
        }
        
        public void HashPassword(string password, out string passwordHash, out string passwordSalt)
        {
            if (password.Any(char.IsWhiteSpace))
                throw new InvalidPasswordFormat();
            if (string.IsNullOrEmpty(password) || password.Length < MinimumPasswordLength)
                throw new InvalidPasswordLength(this.MinimumPasswordLength, this.MaximumPasswordLength);
            if (this.IsDigitRequired && !password.Any(char.IsDigit))
                throw new InvalidPasswordFormat();
            if (this.IsUppercaseLetterRequired && !password.Any(char.IsUpper))
                throw new InvalidPasswordFormat();
            if (this.IsLowercaseLetterRequired && !password.Any(char.IsLower))
                throw new InvalidPasswordFormat();
            if (this.IsSymbolRequired && !password.Any(char.IsSymbol))
                throw new InvalidPasswordFormat();
            
            using var hmac = new HMACSHA512();
            
            passwordSalt = Convert.ToBase64String(hmac.Key);

            passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }

        public bool VerifyPassword(string password, string passwordHash, string passwordSalt)
        {
            using var hmac = new HMACSHA512(Convert.FromBase64String(passwordSalt));

            return passwordHash == Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }
    }
}