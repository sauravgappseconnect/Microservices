

namespace PlatformService.Models
{
    public class Platform
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required string Publisher { get; set; }

        public required double Cost { get; set; }
    }
}