using Explorer.API.Controllers.User.Messaging;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration.Messaging
{
    [Collection("Sequential")]
    public class MessagesControllerTests : BaseStakeholdersIntegrationTest
    {
        public MessagesControllerTests(StakeholdersTestFactory factory) : base(factory) { }

        protected static MessagesController CreateController(IServiceScope scope)
        {
            var service = scope.ServiceProvider.GetRequiredService<IMessageService>();

            return new MessagesController(service)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                        {
                    new Claim(ClaimTypes.NameIdentifier, "-1"),
                    new Claim(ClaimTypes.Role, "tourist")
                }))
                    }
                }
            };
        }

    }
}