using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using Core_Discord.CoreDatabase.Models;
using Microsoft.EntityFrameworkCore.Design;
using System.Data.SqlClient;
using System.IO;

namespace Core_Discord.CoreDatabase
{
    public class CoreContextFactory : IDesignTimeDbContextFactory<CoreContext>
    {
        public CoreContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CoreContext>();
            var builder = new SqlConnectionStringBuilder("Data Source=data/Core_Discord.db");
            builder.DataSource = Path.Combine(AppContext.BaseDirectory, builder.DataSource);
            optionsBuilder.UseSqlServer(optionsBuilder.ToString());
            var ctx = new CoreContext(optionsBuilder.Options);
            ctx.Database.SetCommandTimeout(60);
            return ctx;
        }
    }

    public class CoreContext : DbContext
    {
        public virtual DbSet<BotConfig> BotConfig { get; set; }
        public virtual DbSet<GuildConfig> ServerConfig { get; set; }
        public virtual DbSet<UserExpStats> UserExpStats { get; set; }
        public virtual DbSet<ExpSettings> ExpSettings { get; set; }
        public virtual DbSet<PlaylistUser> PlaylistUser { get; set; }
        public virtual DbSet<LoadedPackage> LoadedPackages { get; set; }
        //public DbSet<PlaylistSong> PlaylistSong { get; set; }

        //constructor
        public CoreContext(DbContextOptions<CoreContext> options) : base(options)
        {
        }

        public void EnsureSeedData()
        {
            if (!BotConfig.Any())
            {
                var botc = new BotConfig();
                BotConfig.Add(botc);
                SaveChanges();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ///<summary>
            ///
            /// Creates a table in sql for serverconfig
            /// 
            /// </summary>
            #region GuildConfig
            //modelBuilder.Entity<GuildConfig>(entity =>
            //{
            //    entity.ToTable("core_discord_config");
            //    entity.HasIndex(e => e.Id)
            //    .HasName("guild_config_id")
            //    .IsUnique();

            //    entity
            //});
            var configEntity = modelBuilder.Entity<GuildConfig>();
            configEntity
                .HasIndex(c => c.ServerId)
                .IsUnique();
            #endregion

            ///<summary>
            ///
            /// Creates a table in sql for BotConfig
            /// 
            /// Defaults Expsettings
            /// 
            /// </summary>
            #region BotConfig

     
            var botConfigEntity = modelBuilder.Entity<BotConfig>();
            botConfigEntity
                .Property(m => m.ExpMinutesTimeout)
                .HasDefaultValue(5);
            botConfigEntity.Property(m => m.ExpPerMessage)
                .HasDefaultValue(3);
            #endregion

            ///<summary>
            /// To be implemented - Currency
            /// </summary>
            #region Currency
            #endregion

            ///<summary>
            /// To be implemented 
            /// </summary>
            #region playlistUser

            var pluEntity = modelBuilder.Entity<PlaylistUser>();
            pluEntity
                .HasMany(x => x.Songs)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

            #endregion
            #region DiscordUser
            var dis = modelBuilder.Entity<DiscordUser>();
            dis.HasAlternateKey(w => w.UserId);
            modelBuilder.Entity<DiscordUser>()
                .Property(x => x.LastLevelUp)
                .HasDefaultValue(new DateTime(2017, 11, 13, 0, 0, 0, 0, DateTimeKind.Local));
            #endregion

            #region ExpSettings
            modelBuilder.Entity<ExpSettings>()
                .HasOne(x => x.GuildConfig)
                .WithOne(x => x.ExpSettings);
            #endregion

            #region Permission

            #endregion   


        }
    }


}
