using Core_Discord.CoreDatabase.Models;
using Core_Discord.CoreMusic.ResolveStrats;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core_Discord.CoreMusic
{
    public class CoreResolver : ICoreResolver
    {
        public async Task<IResolver> GetResolveStrategy(string query, MusicType? musicType)
        {
            await Task.Yield(); //for async warning
            switch (musicType)
            {
                case MusicType.YouTube:
                    return new YoutubeResolve();
                case MusicType.Local:
                    return new LocalResolve();
                default:
                    // maybe add a check for local files in the future
                        return new YoutubeResolve();
            }
        }
    }
}
