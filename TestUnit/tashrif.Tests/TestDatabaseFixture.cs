using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using tashrif.Context;
using tashrif.Core;
using tashrif.Data.Interfaces;
using tashrif.Repository;

namespace tashrif.Tests;

public class TestDatabaseFixture : IDisposable
{
    public IServiceProvider ServiceProvider { get; }

    public TestDatabaseFixture()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Test.json", optional: false)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var services = new ServiceCollection();
        services.AddDbContext<tashrifDBContext>(options => options.UseNpgsql(connectionString), ServiceLifetime.Scoped);
        services.AddtashrifRepositoryDependencies();
        services.AddtashrifCoreDependencies();
        services.AddSingleton<IWebHostEnvironment>(new TestWebHostEnvironment());

        ServiceProvider = services.BuildServiceProvider();
    }

    /// <summary>
    /// Creates a scope with IUnitOfWork, tashrifDBContext, and contractsService
    /// all sharing the same DbContext instance.
    /// Returns (service, context, scope).
    /// </summary>
    public (contractsService service, tashrifDBContext db, IServiceScope scope) CreateScopedService()
    {
        var scope = ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<tashrifDBContext>();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var fileStorage = scope.ServiceProvider.GetRequiredService<IFileStorageService>();
        var statusHistory = scope.ServiceProvider.GetRequiredService<IstatusHistoryService>();
        var service = new contractsService(uow, fileStorage, statusHistory);
        return (service, db, scope);
    }

    public void Dispose()
    {
        (ServiceProvider as IDisposable)?.Dispose();
    }
}

public class TestWebHostEnvironment : IWebHostEnvironment
{
    public string EnvironmentName { get; set; } = Microsoft.AspNetCore.Hosting.EnvironmentName.Development;
    public string ApplicationName { get; set; } = "tashrif.Tests";
    public string ContentRootPath { get; set; } = Path.GetTempPath();
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    public string WebRootPath { get; set; } = Path.GetTempPath();
    public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
}
