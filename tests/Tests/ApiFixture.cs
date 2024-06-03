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
using Xunit;

namespace SomeBasicEFApp.Tests;
public class DbFixture : IDisposable
{
    const string db = "ApiFixture.db";
    public string ConnectionString => "Data Source=" + db;
    public void Dispose()
    {
        if (!File.Exists(db)) return;
        try { File.Delete(db); }
        catch
        {
            // ignored
        }
    }
}
public class ApiFixture : IAsyncLifetime
{
    private readonly TestServer _testServer;
    private readonly DbFixture _dbFixture = new();
    public ApiFixture()
    {
        _testServer = 
            new TestServer(new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>{
                    {"ConnectionStrings:DefaultConnection",_dbFixture.ConnectionString}
                }).Build())
                .UseStartup<UseSqliteStartup>())
            { AllowSynchronousIO = true };
    }

    public async Task InitializeAsync()
    {
        using var serviceScope = _testServer.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<CoreDbContext>();
        await context.Database.MigrateAsync();
    }

    public Task DisposeAsync()
    {
        _testServer.Dispose();
        _dbFixture.Dispose();
        return Task.CompletedTask;
    }

    public TestServer Server => _testServer;
}
class UseSqliteStartup : Startup
{
    public UseSqliteStartup(IConfiguration configuration, IWebHostEnvironment env) : base(configuration, env)
    {
    }
    protected override void ConfigureDbContext(DbContextOptionsBuilder options)
    {
        options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"));
    }
}
