using Marten.Events.Aggregation;

using StrongConsistencyDemo.Models.Events;

namespace StrongConsistencyDemo.Models.Projections
{
    public class UserSummaryProjection : SingleStreamProjection<UserSummary, Guid>
    {
        public static UserSummary Create(UserCreated @event)
        {
            return new UserSummary()
            {
                Id = @event.User.Id,
                FirstName = @event.User.FirstName,
                LastName = @event.User.LastName,
            };
        }

        public static void Apply(UserSummary userSummary, UserGeneralUpdated @event)
        {
            if (!string.IsNullOrEmpty(@event.FirstName))
            {
                userSummary.FirstName = @event.FirstName;
            }

            if (!string.IsNullOrEmpty(@event.LastName))
            {
                userSummary.LastName = @event.LastName;
            }
        }
    }
}
