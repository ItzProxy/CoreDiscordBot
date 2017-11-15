using System.Collections.Generic;

namespace Core_Discord.CoreDatabase.Models
{
    public class BotConfig : DbEntity
    {
        //size of file that bot will hold at one time
        public long BufferSize { get; set; } = 3500000; // 3.5 mb
        //ability to 
        public bool ForwardMessages { get; set; } = true;
        //permissions that bot has
        public int PermissionVersion { get; set; }
        public string DefaultPrefix { get; set; } = ".";

        //currency stuff...this is just the preliminary implementation
        public float CurrencyGenerationChance { get; set; } = 0.02f;
        public int CurrencyGenerationCooldown { get; set; } = 5;
        public List<PlayingStatus> RotatingPlayStatus { get; set; }  = new List<PlayingStatus>();

        public bool RotateStatus { get; set; } = false;

        //currency information
        public string CurrencyIcon { get; set; } = "⚙";
        public string CurrencyName { get; set; } = "Core Frag";
        public string CurrencyPlural { get; set; } = "Core Frags";
        
        //currency constraints for server
        public int CurrencyDropAmount { get; set; } = 10;
        public int? CurrencyMaxDropAmount { get; set; } = null;
        
        //exp system
        public int ExpPerMessage { get; set; } = 3;
        public int ExpMinutesTimeout { get; set; } = 5;
        public HashSet<LoadedPackage> LoadedPackages { get; set; } = new HashSet<LoadedPackage>();
    }

    public class PlayingStatus : DbEntity
    {
        public string Status { get; set; }
    }
}
