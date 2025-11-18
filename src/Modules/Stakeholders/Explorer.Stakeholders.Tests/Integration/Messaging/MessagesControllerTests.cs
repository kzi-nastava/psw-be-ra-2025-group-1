using Explorer.API.Controllers.User.Messaging;
using Explorer.Stakeholders.API.Public;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration.Messaging
{
    [Collection("Sequential")]
    public class MessagesControllerTests : BaseStakeholdersIntegrationTest
    {
        public MessagesControllerTests(StakeholdersTestFactory factory) : base(factory) { }

        protected static MessagesController CreateController(IServiceScope scope)
        {
            return new MessagesController(
                scope.ServiceProvider.GetRequiredService<IMessageService>()
            );
        }
    }
}