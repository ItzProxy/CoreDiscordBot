using System;

namespace Core_Discord.CoreDatabase.Models
{
    public class DiscordUser : DbEntity
    {
        public ulong UserId { get; set; }
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public string AvatarId { get; set; }

        public int TotalXp { get; set; }
        public DateTime LastLevelUp { get; set; } = DateTime.UtcNow;
        public DateTime LastXpGain { get; set; } = DateTime.MinValue;
        public ExpNotificationType NotifyOnLevelUp { get; set; }

        public override bool Equals(object obj)
        {
            return obj is DiscordUser du
                ? du.UserId == UserId
                : false;
        }

        public override int GetHashCode()
        {
            return UserId.GetHashCode();
        }

        public override string ToString() =>
            Username + "#" + Discriminator;
    }
}
