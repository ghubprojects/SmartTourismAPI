using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartTourismAPI.Infrastructure.DataSeeding;

namespace SmartTourismAPI.Infrastructure.Extensions;

public static class HostExtensions {
    public static async Task SeedDatabaseAsync(this IHost host) {
        using var scope = host.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<AppDbSeeder>();
        await seeder.InitialiseAsync().ConfigureAwait(false);
        await seeder.SeedAsync().ConfigureAwait(false);
    }
}
