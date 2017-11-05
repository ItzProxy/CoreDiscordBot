using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using Core_Discord.CoreExtensions;
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
        public DbSet<BotConfig> BotConfig { get; set; }
        public DbSet<ServerConfig> ServerConfig { get; set; }
        public DbSet<UserExpStats> UserExpStats { get; set; }
        public DbSet<ExpSettings> ExpSettings { get; set; }
        public DbSet<PlaylistUser> PlaylistUser { get; set; }
        public DbSet<PlaylistSong> PlaylistSong { get; set; }

        //constructor
        public CoreContext(DbContextOptions<CoreContext> options) : base(options)
        {
        }

        public void EnsureSeeData()
        {
            if (!BotConfig.Any())
            {
                var botc = new BotConfig();
                BotConfig.Add(botc);
                this.SaveChanges();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ///<summary>
            ///
            /// Creates a table in sql for serverconfig
            /// 
            /// </summary>
            #region ServerConfig
            var configEntity = modelBuilder.Entity<ServerConfig>();
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
                .Property(m => m.ExMinutesTimeout)
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

            var playlistUser = modelBuilder.Entity<PlaylistUser>();

            #endregion

        }
    }


}
