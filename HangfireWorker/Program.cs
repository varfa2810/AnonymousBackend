using AnonymousApplication.Interfaces;
using AnonymousApplication.Services;
using AnonymousInfrastructure.Data;
using Hangfire;
using HangfireWorker;
using Microsoft.AspNetCore.Connections;


var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddHangfire(x =>
    x.UseSqlServerStorage(
        builder.Configuration.GetConnectionString("Default")
    ));

builder.Services.AddHangfireServer();

builder.Services.AddScoped<IHangFire, HangfireService>();
builder.Services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var manager = scope.ServiceProvider
        .GetRequiredService<IRecurringJobManager>();

    JobScheduler.Register(manager);
}
host.Run();
