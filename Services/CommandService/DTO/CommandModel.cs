namespace CommandService.DTO
{
    public class CommandModel
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; } 
        public DateTime UpdatedAt { get; set; } 
        public string? CreatedBy { get; set; }
        public required string HowTo { get; set; }
        public required string CommandLine { get; set; }

        public required string PlatformName { get; set; }
    }
}
