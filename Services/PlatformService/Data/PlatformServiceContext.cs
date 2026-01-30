

using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data
{
    public class PlatformServiceContext : DbContext
    {
        public PlatformServiceContext(DbContextOptions<PlatformServiceContext> options)
        : base(options)
        {

        }

        public DbSet<Platform> Platforms { get; set; }
    }
}