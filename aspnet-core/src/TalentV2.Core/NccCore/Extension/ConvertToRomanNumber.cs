using Abp.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalentV2.NccCore.Extension
{
    public static class ConvertToRomanNumber
    {
        /// <summary>
        /// Conver number to roman-numerals
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        private static string ToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + ToRoman(number - 1000);
            if (number >= 900) return "CM" + ToRoman(number - 900);
            if (number >= 500) return "D" + ToRoman(number - 500);
            if (number >= 400) return "CD" + ToRoman(number - 400);
            if (number >= 100) return "C" + ToRoman(number - 100);
            if (number >= 90) return "XC" + ToRoman(number - 90);
            if (number >= 50) return "L" + ToRoman(number - 50);
            if (number >= 40) return "XL" + ToRoman(number - 40);
            if (number >= 10) return "X" + ToRoman(number - 10);
            if (number >= 9) return "IX" + ToRoman(number - 9);
            if (number >= 5) return "V" + ToRoman(number - 5);
            if (number >= 4) return "IV" + ToRoman(number - 4);
            if (number >= 1) return "I" + ToRoman(number - 1);
            throw new ArgumentOutOfRangeException("something bad happened");
        }

        /// <summary>
        /// return list roman-numerals from 1 to number
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static List<string> ConvertToRoman(this int number)
        {
            var list = new List<string>();
            for (int i = 1; i <= number; i++)
            {
                var x = ToRoman(i);
                list.Add(x);
            }
            return list;

        }

        /// <summary>
        /// return list roman-numerals from 1 to number
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ConvertIntToRoman(this int number)
        {
            return ToRoman(number);
        }
        /// <summary>
        /// follow Ascii Alphabet characters
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string ConvertNumberToAlphabet(this int number)
        {
            Char c = (Char)((65) + (number - 1));
            return c.ToString();
        }
        public static int CheckFromYear(this int fromYear)
        {
            if (fromYear <= 0)
            {
                fromYear = 1994;
            }
            return fromYear;
        }
        public static int CheckToYear(this int toYear)
        {
            if (toYear <= 0)
            {
                toYear = ClockProviders.Local.Now.Year;
            }
            return toYear;
        }
    }
}
