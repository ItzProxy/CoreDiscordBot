﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project_Webserver.OAuth2
{
    public class DiscordDefaults
    {
        public const string AuthenticationScheme = "Discord";
        public const string DisplayName = "Discord";

        public static readonly string AuthorizationEndpoint = "https://discordapp.com/api/oauth2/authorize";
        public static readonly string TokenEndpoint = "https://discordapp.com/api/oauth2/token";
        public static readonly string UserInformationEndpoint = "https://discordapp.com/api/users/@me";
    }
}
