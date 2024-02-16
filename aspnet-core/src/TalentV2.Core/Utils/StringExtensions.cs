using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TalentV2.Constants.Enum;

namespace TalentV2.Utils
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input) =>
        input switch
        {
            null => throw new ArgumentNullException(nameof(input)),
            "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
            _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
        };

        public static string GetNamePerson(string fullName)
        {
            var stringNames = fullName.Split(" ");
            if (stringNames.Length == 1)
                return stringNames[0];
            return stringNames[stringNames.Length - 1];
        }

        public static string GetSurnamePerson(string fullName)
        {
            var stringNames = fullName.Split(" ");
            if (stringNames.Length == 1)
                return stringNames[0];
            var surname = fullName.Substring(0, fullName.Length - stringNames[stringNames.Length - 1].Length);
            return surname.Trim();
        }

        public static string GetAccountUserLMS(string fullName, string userType, string subposition, string branch, StatusCreateAccount statusCreateAccount)
        {
            var textNotUnicode = RemoveSign4VietnameseString(fullName);
            var surname = GetSurnamePerson(textNotUnicode);

            var name = GetNamePerson(textNotUnicode);
            if (statusCreateAccount == StatusCreateAccount.CREATE_LMS_ACCOUT)
            {
                var usName = name + "." + ReplaceWhitespace(surname) + "."
                + ReplaceWhitespace(subposition) + "."
                + userType + "."
                + GetBranchName(branch) + "."
                + UsernameUntils.GenerateUsername(3, true);
                return ReplaceSpecialCharacters(usName);
            };
            var usNameContest = name + "_" + GetInitials(surname) + "_"
                + ReplaceWhitespace(subposition.Substring(0, 2)) + "_"
                + GetBranchName(branch) + "_"
                + UsernameUntils.GenerateUsername(3, true);

            return ReplaceSpecialCharacters(usNameContest);
        }

        public static string ReplaceSpecialCharacters(string input) => Regex.Replace(input, "/", "_");

        private static string GetBranchName(string branch)
        {
            branch = ReplaceWhitespace(branch);
            branch = RemoveSign4VietnameseString(branch);
            return branch;
        }

        public static string ReplaceWhitespace(string input)
            => Regex.Replace(input, @"\s+", "");

        public static string GetInitials(string input)
        {
            string[] words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            StringBuilder initials = new StringBuilder();

            foreach (string word in words)
            {
                initials.Append(word[0]);
            }

            return initials.ToString();
        }

        private static string RemoveSign4VietnameseString(string str)
        {
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            return str;
        }

        private static readonly string[] VietnameseSigns = new string[]
        {
            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "ỗóòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"
        };
    }
}