using System;
using System.Collections.Generic;
using System.Text;

namespace Core_Discord.CoreDatabase.Models
{
    public class BotConfig : DbEntity
    {
        public ulong BufferSize { get; set; } = 3500000; // 3.5 mb
        public bool ForwardMessages { get; set; } = true;
        public int PermissionVersion { get; set; }
        public string DefaultPrefix { get; set; } = ".";

        public float CurrencyGenerationChance { get; set; } = 0.02f;
        public int CurrencyGeneration { get; set; } = 5;
        public List<PlayingStatus> RotatingPlayStatus = new List<PlayingStatus>();

        public bool RotateStatus { get; set; } = false;

        //currency information
        public string CurrencyIcon { get; set; } = "⚙";
        public string CurrencyName { get; set; } = "Core Frag";
        public string CurrencyPlural { get; set; } = "Core Frags";
        
        //currency constraints for server
        public int CurrencyDropAmount = 10;
        public int? MaxCurrencyDropAmount = null;
        
        //exp system
        public int ExpPerMessage { get; set; } = 3;
        public int ExMinutesTimeout { get; set; } = 5;
    }

    public class PlayingStatus : DbEntity
    {
        public string Status { get; set; }
    }
}
