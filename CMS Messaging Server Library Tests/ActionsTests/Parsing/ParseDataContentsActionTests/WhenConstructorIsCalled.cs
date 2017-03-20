namespace CMS.Messaging.Server.Library.Tests.ActionsTests.Parsing.ParseDataContentsActionTests
{
    using System;
    using Actions.Parsing;
    using Xunit;

    public class WhenConstructorIsCalled : ActionsTestsBase
    {
        [Fact]
        public void ItShouldThrowAnArgumentNullExceptionIfFactoryIsMissing()
        {
            Assert.Throws<ArgumentNullException>(() => new ParseDataContentsAction(null, this.IdentifierKey));
        }

        [Fact]
        public void ItShouldThrowAnArgumentNullExceptionIfIdentifierKeyIsMissing()
        {
            Assert.Throws<ArgumentNullException>(() => new ParseDataContentsAction(this.Factory.Object, null));
        }
    }
}