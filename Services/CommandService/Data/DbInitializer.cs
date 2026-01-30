using Microsoft.EntityFrameworkCore;

namespace CommandService.Data
{
    public static class DbInitializer
    {
        public static async Task Initialiser(this IServiceProvider serviceProvider) {
            using (var scope = serviceProvider.CreateScope()) {
                var db = scope.ServiceProvider.GetService<CommandServiceContext>();
                await db!.Database.MigrateAsync();
            }
        }
    }
}
