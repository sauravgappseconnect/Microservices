

namespace Microservices.Common.Models
{
    /// <summary>
    /// Common model for platform messages sent over messaging services.
    /// </summary>
    public class PlatformMessageModel
    {
        public required string Event { get; set; }
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Publisher { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string? CreatedBy { get; set; }
    }
}
