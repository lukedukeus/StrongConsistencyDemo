using Marten.Events.Aggregation;

using StrongConsistencyDemo.Models.Events;

namespace StrongConsistencyDemo.Models.Projections
{
    public class UserGeneralProjection : SingleStreamProjection<UserGeneral, Guid>
    {
        public static UserGeneral Create(UserCreated @event)
        {
            return new UserGeneral()
            {
                Id = @event.User.Id,
                FirstName = @event.User.FirstName,
                LastName = @event.User.LastName,
                ProfilePicBase64 = string.Empty
            };
        }

        public static void Apply(UserGeneral userGeneral, UserGeneralUpdated @event)
        {
            if (!string.IsNullOrEmpty(@event.FirstName))
            {
                userGeneral.FirstName = @event.FirstName;
            }

            if (!string.IsNullOrEmpty(@event.LastName))
            {
                userGeneral.LastName = @event.LastName;
            }
        }

        public static void Apply(UserGeneral userGeneral, UserProfilePicUpdated @event)
        {
            userGeneral.ProfilePicBase64 = @event.ProfilePicBase64;
        }
    }
}
