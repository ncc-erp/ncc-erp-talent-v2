﻿using System;
using System.Collections.Generic;
using System.Globalization;
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

        public static string GetAccountUserLMS(string fullName, string userType, string subposition, string branch)
        {
            var textNotUnicode = RemoveSign4VietnameseString(fullName);
            var surname = GetSurnamePerson(textNotUnicode);

            var name = GetNamePerson(textNotUnicode);
          
                var usName = name + "." + ReplaceWhitespace(surname) + "."
                + ReplaceWhitespace(subposition) + "."
                + userType + "."
                + GetBranchName(branch) + "."
                + UsernameUntils.GenerateUsername(3, true);
                return ReplaceSpecialCharacters(usName);
        }

        public static string ReplaceSpecialCharacters(string input) => Regex.Replace(input, "/", "_");

        private static string GetBranchName(string branch)
        {
            branch = ReplaceWhitespace(branch);
            branch = RemoveSign4VietnameseString(branch);
            return branch;
        }

        public static string FormatPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return phoneNumber;
            }

            string formattedNumber = Regex.Replace(phoneNumber, @"\D", "");
            formattedNumber = Regex.Replace(formattedNumber, "^84", "0");
            return formattedNumber;
        }

        public static string FormatName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            string normalized = Regex.Replace(name, @"[-_.]", " ");

            normalized = Regex.Replace(normalized, @"\s+", " ");

            TextInfo textInfo = new CultureInfo("vi-VN", false).TextInfo;
            normalized = textInfo.ToTitleCase(normalized.ToLower());

            return normalized.Trim();
        }

        public static string ReplaceWhitespace(string input)
            => Regex.Replace(input, @"\s+", "");

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