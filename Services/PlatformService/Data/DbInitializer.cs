using Microsoft.EntityFrameworkCore;

namespace PlatformService.Data
{
    public static class DbInitializer
    {
        public static async Task Initialiser(this IServiceProvider serviceProvider) {
            using (var scope = serviceProvider.CreateScope()) {
                var db = scope.ServiceProvider.GetService<PlatformServiceContext>();
                await db!.Database.MigrateAsync();
            }
        }
    }
}
