﻿using System.Data.Entity;

namespace ScoreboardAPI.Models
{
    public class ScoreboardAPIContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, add the following
        // code to the Application_Start method in your Global.asax file.
        // Note: this will destroy and re-create your database with every model change.
        // 
        // System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<ScoreboardAPI.Models.ScoreboardAPIContext>());

        public ScoreboardAPIContext() : base("name=ScoreboardAPIContext")
        {
            // ensures that the DB structure get's updated if it has changed
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ScoreboardAPIContext, Migrations.Configuration>());
        }

        public DbSet<Score> Scores { get; set; }
    }
}
