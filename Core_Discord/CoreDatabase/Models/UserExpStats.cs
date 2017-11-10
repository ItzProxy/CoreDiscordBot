﻿using System;

namespace Core_Discord.CoreDatabase.Models
{
    public class UserExpStats : DbEntity
    {
        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }
        public int Exp { get; set; }
        public int AwardedExp { get; set; }
        public ExpNotificationType NotifyOnLevelUp { get; set; }
        public DateTime LastLevelUp { get; set; } = DateTime.UtcNow;
    }

    public enum ExpNotificationType { None, Dm, Channel }
}
