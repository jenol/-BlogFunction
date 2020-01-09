using Blog.Service.DomainObjects;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class EmailTests
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public void TryParseTests()
        {
            var o = Email.TryParse("test1@example.org", out var email);

            Assert.Pass();
        }
    }
}