using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core_Discord.CoreExtensions
{
    public static class StringExtension
    {
        /// <summary>
        /// Maps json object to c# object based on json property
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T MapJson<T>(this string str)
            => JsonConvert.DeserializeObject<T>(str);
        /// <summary>
        /// Strips html from string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StripHTML(this string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
        /// <summary>
        /// Trims string based on length
        /// Refactored from NadekoBot/src/NadekoBot/Common/Extension/Stringextension.cs
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxLength"></param>
        /// <param name="hideDots"></param>
        /// <returns></returns>
        public static string TrimTo(this string str, int maxLength, bool hideDots = false)
        {
            if(maxLength < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLength), $"Argument {nameof(maxLength)} cannot be negative");
            }
            if(maxLength == 0)
            {
                return String.Empty;
            }
            if(maxLength <= 3)
            {
                return String.Concat(str.Select(s => '.'));
            }
            if(str.Length < maxLength)
            {
                return str;
            }
            return String.Concat(str.Take(maxLength - 3)) + (hideDots ? "" : "...");
        }
    }
}
