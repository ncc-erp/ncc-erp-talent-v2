using System;

namespace TalentV2.Utils
{
    public static class EmailUtils
    {
        public static string GetNccUserName(string email)
        {
            ArgumentNullException.ThrowIfNull(email);

            const string domain = "@ncc.plus";
            if (email.EndsWith(domain, StringComparison.OrdinalIgnoreCase))
            {
                return email.Substring(0, email.Length - domain.Length);
            }

            return email;
        }
    }
}
