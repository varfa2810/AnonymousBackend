using AnonymousInfrastructure.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

var apiProjectPath = Path.GetFullPath(
    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "AnonymousApi"));

if (!Directory.Exists(apiProjectPath))
{
    throw new DirectoryNotFoundException($"API project path not found: {apiProjectPath}");
}

builder.Configuration
    .SetBasePath(apiProjectPath)
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

var configuration = builder.Configuration;

var connectionString = configuration.GetConnectionString("Default");

if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine(builder.Environment.EnvironmentName);
    Console.WriteLine(apiProjectPath);
    throw new InvalidOperationException("Connection string 'Default' was not found.");
}

DbUpMigrationService.MigrateDatabase(connectionString);

Console.WriteLine("Database migration completed successfully.");