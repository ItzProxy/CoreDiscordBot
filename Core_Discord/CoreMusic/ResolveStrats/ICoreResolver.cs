using Core_Discord.CoreDatabase.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core_Discord.CoreMusic.ResolveStrats
{
    public interface ICoreResolver
    {
        Task<IResolver> GetResolveStrategy(string query, MusicType? musicType);
    }
}
