

namespace PlatformService.Models
{
    public class Platform : Base
    {
        public required string Name { get; set; }

        public required string Publisher { get; set; }

        public required double Cost { get; set; }
    }
}