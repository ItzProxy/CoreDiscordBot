using System;
using System.Collections.Generic;
using System.Text;

namespace Core_Discord.CoreDatabase.Models
{
    /// <summary>
    /// encapsulates the local configuration of bot, this is different from the global configuration file (CoreConfig.cs)
    /// </summary>
    public class GuildConfig : DbEntity
    {
        public long ServerId { get; set; }
        public string Prefix { get; set; }
        public long AutoAssignRoleId { get; set; }

        //greet stuff to be implemented later

        //music settings/voice settings
        public float DefaultMusicVolume = 1.0f;
        public bool AutoDcFromVc { get; set; }

        //server time
        public string Locale { get; set; } = null;
        public string TimeZoneId { get; set; } = null;

        //experience system
        public ExpSettings ExpSettings { get; set; }
    }
}
