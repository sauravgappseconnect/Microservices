using Microsoft.EntityFrameworkCore;

namespace CommandService.Data
{
    public static class DbInitializer
    {
        public static async Task Initialiser(this IServiceProvider serviceProvider) {
            using (var scope = serviceProvider.CreateScope()) {
                await Task.Delay(3000); // Wait for the database server to be ready
                var db = scope.ServiceProvider.GetService<CommandServiceContext>();
                await db!.Database.MigrateAsync();
            }
        }
    }
}
