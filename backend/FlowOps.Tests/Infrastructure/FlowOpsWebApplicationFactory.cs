using FlowOps.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace FlowOps.Tests.Infrastructure;

public class FlowOpsWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove the application's PostgreSQL DbContext registration.
            var dbContextDescriptor = services.SingleOrDefault(
                service => service.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (dbContextDescriptor is not null)
            {
                services.Remove(dbContextDescriptor);
            }

            // Read the test database connection string
            // from FlowOps.Tests user secrets.
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<FlowOpsWebApplicationFactory>()
                .Build();

            var connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Test database connection string was not found.");

            // Register AppDbContext using the test database.
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Apply migrations and seed data to the test database.
            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            dbContext.Database.Migrate();
        });
    }
}