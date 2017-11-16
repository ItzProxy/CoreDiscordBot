using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Core_Discord.Migrations
{
    public partial class First : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BotConfig",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BufferSize = table.Column<long>(nullable: false),
                    CurrencyDropAmount = table.Column<int>(nullable: false),
                    CurrencyGenerationChance = table.Column<float>(nullable: false),
                    CurrencyGenerationCooldown = table.Column<int>(nullable: false),
                    CurrencyIcon = table.Column<string>(nullable: true),
                    CurrencyMaxDropAmount = table.Column<int>(nullable: true),
                    CurrencyName = table.Column<string>(nullable: true),
                    CurrencyPlural = table.Column<string>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: true),
                    DefaultPrefix = table.Column<string>(nullable: true),
                    ExpMinutesTimeout = table.Column<int>(nullable: false, defaultValue: 5),
                    ExpPerMessage = table.Column<int>(nullable: false, defaultValue: 3),
                    ForwardMessages = table.Column<bool>(nullable: false),
                    PermissionVersion = table.Column<int>(nullable: false),
                    RotateStatus = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscordUser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AvatarId = table.Column<string>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: true),
                    Discriminator = table.Column<string>(nullable: true),
                    LastLevelUp = table.Column<DateTime>(nullable: false, defaultValue: new DateTime(2017, 11, 13, 0, 0, 0, 0, DateTimeKind.Local)),
                    LastXpGain = table.Column<DateTime>(nullable: false),
                    NotifyOnLevelUp = table.Column<int>(nullable: false),
                    TotalXp = table.Column<int>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    Username = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordUser", x => x.Id);
                    table.UniqueConstraint("AK_DiscordUser_UserId", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "PlaylistUser",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Author = table.Column<string>(nullable: true),
                    AuthorId = table.Column<long>(nullable: false),
                    DateAdded = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServerConfig",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AutoAssignRoleId = table.Column<long>(nullable: false),
                    AutoDcFromVc = table.Column<bool>(nullable: false),
                    DateAdded = table.Column<DateTime>(nullable: true),
                    Locale = table.Column<string>(nullable: true),
                    Prefix = table.Column<string>(nullable: true),
                    ServerId = table.Column<long>(nullable: false),
                    TimeZoneId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerConfig", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserExpStats",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AwardedExp = table.Column<int>(nullable: false),
                    DateAdded = table.Column<DateTime>(nullable: true),
                    Exp = table.Column<int>(nullable: false),
                    GuildId = table.Column<long>(nullable: false),
                    LastLevelUp = table.Column<DateTime>(nullable: false),
                    NotifyOnLevelUp = table.Column<int>(nullable: false),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserExpStats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoadedPackages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BotConfigId = table.Column<int>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadedPackages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoadedPackages_BotConfig_BotConfigId",
                        column: x => x.BotConfigId,
                        principalTable: "BotConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayingStatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BotConfigId = table.Column<int>(nullable: true),
                    DateAdded = table.Column<DateTime>(nullable: true),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayingStatus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlayingStatus_BotConfig_BotConfigId",
                        column: x => x.BotConfigId,
                        principalTable: "BotConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlaylistSong",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateAdded = table.Column<DateTime>(nullable: true),
                    PlaylistUserId = table.Column<int>(nullable: true),
                    Provider = table.Column<string>(nullable: true),
                    ProviderType = table.Column<int>(nullable: false),
                    Query = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Uri = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistSong", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaylistSong_PlaylistUser_PlaylistUserId",
                        column: x => x.PlaylistUserId,
                        principalTable: "PlaylistUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExpSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateAdded = table.Column<DateTime>(nullable: true),
                    ExpRoleRewardExclusive = table.Column<bool>(nullable: false),
                    GuildConfigId = table.Column<int>(nullable: false),
                    NotifyMessage = table.Column<string>(nullable: true),
                    ServerExcluded = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpSettings_ServerConfig_GuildConfigId",
                        column: x => x.GuildConfigId,
                        principalTable: "ServerConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpSettings_GuildConfigId",
                table: "ExpSettings",
                column: "GuildConfigId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoadedPackages_BotConfigId",
                table: "LoadedPackages",
                column: "BotConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayingStatus_BotConfigId",
                table: "PlayingStatus",
                column: "BotConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistSong_PlaylistUserId",
                table: "PlaylistSong",
                column: "PlaylistUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerConfig_ServerId",
                table: "ServerConfig",
                column: "ServerId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscordUser");

            migrationBuilder.DropTable(
                name: "ExpSettings");

            migrationBuilder.DropTable(
                name: "LoadedPackages");

            migrationBuilder.DropTable(
                name: "PlayingStatus");

            migrationBuilder.DropTable(
                name: "PlaylistSong");

            migrationBuilder.DropTable(
                name: "UserExpStats");

            migrationBuilder.DropTable(
                name: "ServerConfig");

            migrationBuilder.DropTable(
                name: "BotConfig");

            migrationBuilder.DropTable(
                name: "PlaylistUser");
        }
    }
}
