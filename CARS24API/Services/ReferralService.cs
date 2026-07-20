using System.Security.Cryptography;

namespace Cars24API.Services
{
    public class ReferralService
    {
        public string GenerateReferralCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var code = new char[8];

            for (int i = 0; i < code.Length; i++)
            {
                code[i] = chars[RandomNumberGenerator.GetInt32(chars.Length)];
            }

            return $"CARS24-{new string(code)}";
        }
    }
}
