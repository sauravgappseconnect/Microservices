

namespace PlatformService.DTO
{
    public class PlatformCreateModel
    {
        public required string Name { get; set; }

        public required string Publisher { get; set; }

        public required double Cost { get; set; }
    }
}