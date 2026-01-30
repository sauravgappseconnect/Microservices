using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data
{
    public class CommandServiceContext : DbContext
    {
        public CommandServiceContext(DbContextOptions<CommandServiceContext> options)
            :base(options)
        {
            
        }

        public DbSet<Platform> Platforms { get; set; } = null!;

        public DbSet<Command> Commands { get; set; } = null!;
    }
}
