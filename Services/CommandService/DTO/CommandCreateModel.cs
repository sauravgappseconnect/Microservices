namespace CommandService.DTO
{
    public class CommandCreateModel
    {
        public required string HowTo { get; set; }
        public required string CommandLine { get; set; }
        public required Guid PlatformId { get; set; }
    }
}
