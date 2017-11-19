using Core_Discord.CoreDatabase.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Core_Discord.CoreMusic.ResolveStrats
{
    public class LocalResolve : IResolver
    {
        public Task<MusicInfo> ResolveSong(string query)
        {
            return Task.FromResult(new MusicInfo
            {
                Url = () => Task.FromResult("\"" + Path.GetFullPath(query) + "\""),
                Title = Path.GetFileNameWithoutExtension(query),
                Provider = "Local File",
                ProviderType = MusicType.Local,
                Query = query,
                Thumbnail = "https://cdn.discordapp.com/attachments/155726317222887425/261850914783100928/1482522077_music.png",
            });
        }
    }
}
