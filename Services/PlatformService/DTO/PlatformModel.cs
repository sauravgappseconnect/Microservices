

namespace PlatformService.DTO
{
    public class PlatformModel
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required string Publisher { get; set; }

        public required double Cost { get; set; }
    }
}