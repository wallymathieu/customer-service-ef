using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SomeBasicEFApp.Web;
using SomeBasicEFApp.Web.Data;
using Testcontainers.MsSql;

namespace SomeBasicEFApp.Tests;
public class DbFixture : IDisposable
{
    // ugly:
    protected static Lazy<MsSqlContainer> _container = new(() =>
    {
        var _dbContainer = new MsSqlBuilder()
           .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
           .WithPassword("Strong_password_123!")
           .WithHostname(Guid.NewGuid().ToString("N"))
           .Build();
        _dbContainer.StartAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        var opts = new DbContextOptionsBuilder()
            .UseSqlServer(_dbContainer.GetConnectionString())
            .Options;
        using (var db = new CoreDbContext(opts))
        {
            db.Database.Migrate();
        }
        return _dbContainer;
    });
    public string ConnectionString => _container.Value.GetConnectionString();
    public void Dispose()
    {
        _container.Value.DisposeAsync();
    }
}

public class ApiFixture: IDisposable
{
    DbFixture _dbFixture = new ();

    TestServer Create()
    {
        return new TestServer(new WebHostBuilder()
            .UseKestrel()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseConfiguration(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?>
            {
                {"ConnectionStrings:DefaultConnection",_dbFixture.ConnectionString}
            }).Build())
            .UseStartup<Startup>()) { AllowSynchronousIO=true };
    }
    private readonly TestServer _testServer;
    public ApiFixture() => _testServer = Create();
    public void Dispose()
    {
        _testServer.Dispose();
        _dbFixture.Dispose();
    }
    public TestServer Server => _testServer;
}
