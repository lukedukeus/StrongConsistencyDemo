using JasperFx;

namespace StrongConsistencyDemo.Models
{
    public class UserSummary : IRevisioned
    {
        public required Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public int Version { get; set; }
    }
}
