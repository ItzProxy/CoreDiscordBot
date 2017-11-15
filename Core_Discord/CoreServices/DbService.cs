using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Core_Discord.CoreDatabase;
using System.Data.SqlClient;
using Core_Discord.CoreServices.Interfaces;
using System.IO;
using System.Linq;
using NLog;

namespace Core_Discord.CoreServices
{
    //provides connection to Database
    public class DbService
    {
        private Logger _log;
        private DbContextOptions<CoreContext> options;
        private DbContextOptions<CoreContext> migrateOptions;

        public DbService(ICoreCredentials creds)
        {
            //gets the database 
            var builder = new SqlConnectionStringBuilder(creds.Db.ConnectionString);
            //builder.DataSource = Path.Combine(AppContext.BaseDirectory, builder.DataSource);
            
            //builds sql server using context
            var optionBuilder = new DbContextOptionsBuilder<CoreContext>();
            optionBuilder.UseSqlServer(builder.ToString());
            options = optionBuilder.Options;

            optionBuilder = new DbContextOptionsBuilder<CoreContext>();
            //migrate options if implemented
            optionBuilder.UseSqlServer(builder.ToString());
            migrateOptions = optionBuilder.Options;
        }

        public CoreContext GetDbCoreContext()
        {
            _log = LogManager.GetCurrentClassLogger();
            var context = new CoreContext(options);
            //migration context implementation here
            if (context.Database.GetPendingMigrations().Any())
            {
                var mContext = new CoreContext(migrateOptions);
                mContext.Database.Migrate();
                mContext.SaveChanges();
                mContext.Dispose();
            }
            //sets default data and configurations are set
            //sets time before database closes
            context.Database.SetCommandTimeout(60);
            Console.Write(context.Database.IsSqlServer());
            context.EnsureSeedData();

            //connection to database
            var conn = context.Database.GetDbConnection();
            _log.Info(conn.ConnectionString);
            conn.Open();
            //not sure if SQL has WAL(write ahead log)
            //context.Database.ExecuteSqlCommand("PRAGMA journal_mode=WAL");
           
            //using(var dbconn = conn.CreateCommand())
            //{
            //    dbconn.CommandText = "PRAGMA syncronous=off";
            //    dbconn.ExecuteNonQuery();
            //}
            return context;
        }
        public IUnitOfWork UnitOfWork { get =>
             new UnitOfWork(GetDbCoreContext()); }
    }
}
