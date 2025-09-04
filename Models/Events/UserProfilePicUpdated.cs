namespace StrongConsistencyDemo.Models.Events
{
    public record UserProfilePicUpdated(UserRecordReference User, string ProfilePicBase64);
}
