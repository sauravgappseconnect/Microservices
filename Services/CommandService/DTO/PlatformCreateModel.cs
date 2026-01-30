

namespace CommandService.DTO
{
    public class PlatformCreateModel
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required string Publisher { get; set; }
    }
}