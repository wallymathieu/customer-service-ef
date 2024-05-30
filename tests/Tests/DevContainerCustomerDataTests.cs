using SomeBasicEFApp.Web.Data;
using Microsoft.EntityFrameworkCore;
using System;
using Testcontainers.MsSql;

namespace SomeBasicEFApp.Tests;

public class DevContainerCustomerDataTests : CustomerDataTests
{
    private static Lazy<DbContextOptions> options = new(() =>
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
        return Setup(opts);
    });
    public override DbContextOptions Options => options.Value;
}
