using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core_Discord.CoreMusic.ResolveStrats
{
    public interface IResolver
    {
        Task<MusicInfo> ResolveSong(string query);
    }
}
