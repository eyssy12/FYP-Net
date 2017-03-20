namespace CMS.Messaging.Server.Library.Tests.ActionsTests.Decisions.RegistrateMobileClientActionTests
{
    using System.Collections.Generic;
    using System.Linq;
    using Actions.Decisions;
    using CMS.Shared.Library.Models;
    using CMS.Shared.Library.Providers;
    using Models;
    using Moq;
    using Xunit;

    public class WhenRunIsCalled : ActionsTestsBase
    {
        protected readonly Mock<IGcmMobileClientProvider> Provider;
        protected readonly RegistrateMobileClientAction Action;

        protected readonly string Token = "123455abc",
            EntityId = "entity-id-1234";

        public WhenRunIsCalled()
        {
            this.Provider = new Mock<IGcmMobileClientProvider>();
            this.Action = new RegistrateMobileClientAction(
                this.Factory.Object, 
                this.Provider.Object,
                identifierKey: this.IdentifierKey);
        }

        [Fact]
        public void ItShouldCreateANewMobileClientIfThereIsNotAnExistingMobileToken()
        {
            IEnumerable<GcmMobileClient> clients = new[] { new GcmMobileClient { Token = "any_token" } };

            this.Provider.Setup(s => s.GetAll()).Returns(clients);

            ParsedGcmMessageStateHolder parsedStateholder = new ParsedGcmMessageStateHolder();
            parsedStateholder.Action = this.IdentifierKey;
            parsedStateholder.Value = this.Token;
            parsedStateholder.EntityId = this.EntityId;

            this.Action.Run(parsedStateholder);

            this.Provider.Verify(s => s.GetAll(), Times.Once);
            this.Provider.Verify(
                s => s.Create(
                    It.Is<GcmMobileClient>(e => 
                        e.Token == this.Token &&
                        e.EntityId == this.EntityId)), 
                Times.Once);

        }
    }
}
