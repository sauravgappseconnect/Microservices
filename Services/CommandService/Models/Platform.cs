namespace CommandService.Models
{
    public class Platform
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required string Publisher { get; set; }

        public ICollection<Command> Commands { get; set; } = null!;
    }
}
