using System;
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

public class ApiFixture
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

    static TestServer Create()
    {
        return new TestServer(new WebHostBuilder()
            .UseKestrel()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseConfiguration(new ConfigurationBuilder().Build())
            .UseStartup<TestStartup>()) { AllowSynchronousIO=true };
    }
    private readonly TestServer _testServer;
    public ApiFixture() => _testServer = Create();
    public void Dispose()
    {
        _testServer.Dispose();
    }
    public TestServer Server=>_testServer;

    const string db = "ApiFixture.db";
    class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
        {
        }
        protected override void ConfigureDbContext(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(_container.Value.GetConnectionString());
        }
        protected override void OnConfigured(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<CoreDbContext>();
            context.Database.Migrate();
        }
    }
}
