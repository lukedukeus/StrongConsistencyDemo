using JasperFx;
using JasperFx.Events.Projections;

using Marten;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NetTopologySuite.Utilities;

using StrongConsistencyDemo.Models;
using StrongConsistencyDemo.Models.Events;
using StrongConsistencyDemo.Models.Projections;

using Testcontainers.PostgreSql;

namespace StrongConsistencyDemo
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var postgreSql = new PostgreSqlBuilder().WithImage("postgres:16").Build();

            await postgreSql.StartAsync();

            var host = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
            {
                services.AddMarten(options =>
                {
                    options.Connection(postgreSql.GetConnectionString());
                    options.UseSystemTextJsonForSerialization();
                    options.AutoCreateSchemaObjects = AutoCreate.All;

                    options.Events.AddEventType<UserCreated>();
                    options.Events.AddEventType<UserGeneralUpdated>();
                    options.Events.AddEventType<UserLoggedIn>();
                    options.Events.AddEventType<UserProfilePicUpdated>();

                    options.Schema.For<UserGeneral>();
                    options.Projections.Add<UserGeneralProjection>(ProjectionLifecycle.Inline);

                    options.Schema.For<UserSummary>();
                    options.Projections.Add<UserSummaryProjection>(ProjectionLifecycle.Inline);
                });
            })
           .Build();

            IQuerySession querySession = host.Services.GetRequiredService<IQuerySession>();
            IDocumentStore documentStore = host.Services.GetRequiredService<IDocumentStore>();
            IServiceProvider serviceProvider = host.Services.GetRequiredService<IServiceProvider>();

            long actualStreamVersion;

            // Add a user
            var userId = Guid.CreateVersion7();
            UserRecordReference user = new UserRecordReference(userId, "testuser", "secret");

            await using (var session = documentStore.LightweightSession())
            {
                var userCreatedEvent = new UserCreated(user);

                var stream = session.Events.StartStream(userId, userCreatedEvent);

                await session.SaveChangesAsync();
            }

            // update the user's profile pic
            await using (var session = documentStore.LightweightSession())
            {
                var profilePicUpdateEvent = new UserProfilePicUpdated(user, "somebase64here");

                var stream = session.Events.Append(user.Id, 2, profilePicUpdateEvent);

                await session.SaveChangesAsync();
            }

            // user logs in
            await using (var session = documentStore.LightweightSession())
            {
                var userLoginEvent = new UserLoggedIn(user);

                var stream = session.Events.Append(user.Id, 3, userLoginEvent);

                await session.SaveChangesAsync();

                actualStreamVersion = stream.Version;
            }

            // Works fine
            var userGeneral = querySession.Query<UserGeneral>().First();
            var userSummary = querySession.Query<UserSummary>().First();

            // summary version and general version are different, stream = 3, usergeneral = 2, usersummary = 1
            Assert.Equals(actualStreamVersion, userGeneral.Version);
            Assert.Equals(actualStreamVersion, userSummary.Version);
        }
    }
}
