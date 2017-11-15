using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Migrations;


namespace Core_Discord.CoreMigrations
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
                    .Annotation("Autoincrement", true),
                BufferSize = table.Column<ulong>(nullable: false),
                CurrencyGenerationChance = table.Column<float>(nullable: false),
                CurrencyGenerationCooldown = table.Column<int>(nullable: false),
                CurrencyName = table.Column<string>(nullable: true),
                CurrencyPluralName = table.Column<string>(nullable: true),
                CurrencySign = table.Column<string>(nullable: true),
                DMHelpString = table.Column<string>(nullable: true),
                ForwardMessages = table.Column<bool>(nullable: false),
                ForwardToAllOwners = table.Column<bool>(nullable: false),
                HelpString = table.Column<string>(nullable: true),
                MigrationVersion = table.Column<int>(nullable: false),
                RemindMessageFormat = table.Column<string>(nullable: true),
                RotatingStatuses = table.Column<bool>(nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_BotConfig", x => x.Id);
            });
        }
    }
}
