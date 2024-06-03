using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SomeBasicEFApp.Web;
using SomeBasicEFApp.Web.Data;
using Testcontainers.MsSql;
using Xunit;

namespace SomeBasicEFApp.Tests;
public class DbFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _dbContainer;

    public DbFixture()
    {
        _dbContainer = new MsSqlBuilder()
           .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
           .WithPassword("Strong_password_123!")
           .WithHostname(Guid.NewGuid().ToString("N"))
           .Build();
    }

    public string ConnectionString => _dbContainer.GetConnectionString();

    public async Task DisposeAsync() => await _dbContainer.DisposeAsync();

    public async Task InitializeAsync() => await _dbContainer.StartAsync();
}
public class ApiFixture : IAsyncLifetime
{
    private TestServer? _testServer;
    private readonly DbFixture _dbFixture = new();

    public async Task InitializeAsync()
    {
        await _dbFixture.InitializeAsync();
        _testServer = 
            new TestServer(new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>{
                    {"ConnectionStrings:DefaultConnection",_dbFixture.ConnectionString}
                }).Build())
                .UseStartup<Startup>())
            { AllowSynchronousIO = true };
        using var serviceScope = _testServer.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<CoreDbContext>();
        await context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        _testServer?.Dispose();
        await _dbFixture.DisposeAsync();
    }

    public TestServer Server => _testServer!;
}
