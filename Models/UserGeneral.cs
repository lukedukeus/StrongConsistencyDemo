using JasperFx;

namespace StrongConsistencyDemo.Models
{
    public class UserGeneral : IRevisioned
    {
        public required Guid Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string ProfilePicBase64 { get; set; }
        public int Version { get; set; }
    }
}
