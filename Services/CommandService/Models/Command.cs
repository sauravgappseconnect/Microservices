namespace CommandService.Models
{
    public class Command : Base
    {
        public required string HowTo { get; set; }
        public required string CommandLine { get; set; }
        public Guid PlatformId { get; set; }
        public Platform? Platform { get; set; }
    }
}
