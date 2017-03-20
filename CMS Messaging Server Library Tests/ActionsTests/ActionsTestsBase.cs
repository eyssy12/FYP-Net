namespace CMS.Messaging.Server.Library.Tests.ActionsTests
{
    using CMS.Library.Factories;
    using Moq;

    public abstract class ActionsTestsBase
    {
        protected readonly Mock<IFactory> Factory;
        protected readonly string IdentifierKey;

        protected ActionsTestsBase()
        {
            this.Factory = new Mock<IFactory>();
            this.IdentifierKey = "some_key";
        }
    }
}