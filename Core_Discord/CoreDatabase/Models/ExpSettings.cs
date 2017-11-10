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
        public int ServerConfigId { get; set; }
        public ServerConfig ServerConfig { get; set; }

        //public HashSet<XpRoleReward> RoleRewards { get; set; } = new HashSet<XpRoleReward>();
        public bool XpRoleRewardExclusive { get; set; }
        public string NotifyMessage { get; set; } = "Congratulations {0}! You have reached level {1}!";
        //public HashSet<ExcludedItem> ExclusionList { get; set; } = new HashSet<ExcludedItem>();
        public bool ServerExcluded { get; set; }
    }
}
