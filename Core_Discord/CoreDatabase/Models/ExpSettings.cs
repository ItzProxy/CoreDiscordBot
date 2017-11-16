using System;
using System.Collections.Generic;
using System.Text;

namespace Core_Discord.CoreDatabase.Models
{
    /// <summary>
    /// Provides details of the exp module
    /// </summary>
    public class ExpSettings : DbEntity
    {
        public int GuildConfigId { get; set; }
        public GuildConfig GuildConfig { get; set; }

        //public HashSet<ExpRoleReward> RoleRewards { get; set; } = new HashSet<XpRoleReward>();
        public bool ExpRoleRewardExclusive { get; set; }
        public string NotifyMessage { get; set; } = "Congratulations {0}! You have reached level {1}!";
        //public HashSet<ExcludedItem> ExclusionList { get; set; } = new HashSet<ExcludedItem>();
        public bool ServerExcluded { get; set; }
    }
}
