using DbUp;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace AnonymousInfrastructure.Migrations
{
    public static class DbUpMigrationService
    {
        public static void MigrateDatabase(string connectionString)
        {
            EnsureDatabase.For.SqlDatabase(connectionString);

            var upgrader = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(
                    Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();

            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                throw new Exception(result.Error.ToString());
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Database Migration Successfull.");
            Console.ResetColor();
        }
    }
}
