using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Core_Discord.CoreDatabase;
using System.Data.SqlClient;
using Core_Discord.CoreServices.Interfaces;
using System.IO;
using System.Linq;

namespace Core_Discord.CoreServices
{
    //provides connection to Database
    public class DbService
    {
        private readonly DbContextOptions<CoreContext> options;

        public DbService(IBotCredentials creds)
        {
            //gets the database 
            var builder = new SqlConnectionStringBuilder(creds.Db.ConnectionString);
            builder.DataSource = Path.Combine(AppContext.BaseDirectory, builder.DataSource);

            //builds sql server using context
            var optionBuilder = new DbContextOptionsBuilder<CoreContext>();
            optionBuilder.UseSqlServer(builder.ToString());
            options = optionBuilder.Options;

            //migrate options if implemented
        }

        public CoreContext GetDbCoreContext()
        {
            var context = new CoreContext(options);
            //migration context implementation here

            //sets default data and configurations are set
            //sets time before database closes
            context.Database.SetCommandTimeout(60);
            context.EnsureSeeData();

            //connection to database
            var conn = context.Database.GetDbConnection();
            conn.Open();
            //not sure if SQL has WAL(write ahead log)
            context.Database.ExecuteSqlCommand("PRAGMA journal_mode=WAL");
           
            using(var dbconn = conn.CreateCommand())
            {
                dbconn.CommandText = "PRAGMA journal_mode=WAL; PRAGMA syncronous=off";
                dbconn.ExecuteNonQuery();
            }
            return context;
        }
        public IUnitOfWork UnitOfWork => new UnitOfWork(GetDbCoreContext());
    }
}
