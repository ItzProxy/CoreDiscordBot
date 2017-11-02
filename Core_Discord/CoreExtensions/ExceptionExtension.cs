using System;
using System.Collections.Generic;
using System.Text;

namespace Core_Discord.CoreExtensions
{
    public static class ExceptionExtension
    {
        /// <summary>
        /// Taken from NadekoModuleExtension
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        public static void ThrowIfNull<T>(this T obj, string name) where T : class
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(name));
        }
    }
}
