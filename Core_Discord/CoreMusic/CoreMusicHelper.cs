using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Built using an Outdated Music Player for Discord
/// Retooled to help play music through voice channel
/// </summary>

namespace Core_Discord.Music
{
    public static class CoreMusicHelper  
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
