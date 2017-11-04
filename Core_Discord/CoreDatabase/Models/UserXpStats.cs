using System;

namespace Core_Discord.CoreDatabase.Models
{
    public class UserXpStats : DbEntity
    {
        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }
        public int Xp { get; set; }
        public int AwardedXp { get; set; }
        public ExpNotificationType NotifyOnLevelUp { get; set; }
        public DateTime LastLevelUp { get; set; } = DateTime.UtcNow;
    }

    public enum ExpNotificationType { None, Dm, Channel }
}
