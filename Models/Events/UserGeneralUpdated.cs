namespace StrongConsistencyDemo.Models.Events
{
    public record UserGeneralUpdated(UserRecordReference User, string? Email, string? FirstName, string? LastName);
}
