

namespace CommandService.Models
{
    public class Base
    {
        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string? CreatedBy { get; set; }
    }
}