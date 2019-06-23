using System;
using MoqTestSuit.Tests.TestServices;
using NUnit.Framework;

namespace MoqTestSuit.Tests.Examples
{
    // Example uses NUnit, but you can use anything instead
    public class SimpleTestWithMocks
    {
        // Create a field for the suit instance. MyService should be the type you are testing
        private static readonly ITestSuit<MyService> _testSuit = TestSuit.Create<MyService>();

        [TearDown]
        public void TearDown()
        {
            // You could create a new test suit for each test, but using Reset() after (or before) each test
            // is more performant. Becomes meaningful when you have some hundreds of tests.
            // For xunit this should happen in the constructor of the test.
            _testSuit.Reset();
        }

        [Test]
        public void Always_ShouldParseTheNumber()
        {
            // Arrange

            // Setup your dependencies here as you do in Mock.Of<>()
            // If you call something in the test which was not setup you will get exception
            // "invocation failed with mock behavior Strict".
            _testSuit.SetupMock<IDependency1>(s => s.GetNumber() == "1" && s.SomethingElse == 2);

            // You can also use the traditional Moq form to setup advanced behaviour and void methods
            _testSuit.SetupMockAdv<IDependency2>(m =>
                m.Setup(d => d.Action2()).Callback(() => Console.WriteLine("Test")));

            // Act

            // Sut (system under test) is created lazily on first usage,
            // all constructor dependencies are automatically injected using mocks
            var result = _testSuit.Sut.ParseNumberFromDependency();

            // Assert
            Assert.AreEqual(1, result);
        }
    }
}