using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.Utils
{
    public static class UsernameUntils
    {
        public static string GenerateUsername(int length, bool isComplex)
        {
            if (isComplex)
            {
                const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                StringBuilder sb = new StringBuilder();
                Random random = new Random();

                for (int i = 0; i < length; i++)
                {
                    sb.Append(chars[random.Next(chars.Length)]);
                }

                return sb.ToString();
            }
            return "PasswordUserabc";
        }
    }
}