using System;
using FluentAssertions;
using Moq;
using MoqTestSuit.Tests.TestServices;
using NUnit.Framework;

namespace MoqTestSuit.Tests
{
    public class MocksTests
    {
        private static readonly ITestSuit<MyService> _testSuit = TestSuit.Create<MyService>();

        [TearDown]
        public void AfterTest()
        {
            _testSuit.Reset();
        }

        [Test]
        public void Reset_MocksShouldBeReset()
        {
            //arrange
            _testSuit.SetupMock<IDependency1>(d => d.GetNumber() == "one");
            _testSuit.SetupMock<IDependency2>(d => d.Action2() == "two");
            _testSuit.Sut.Dependency1.GetNumber();
            _testSuit.Sut.Dependency2.Action2();

            //act
            _testSuit.Reset();

            //assert
            //note: checking that the invocations are cleared only because its hard to check anything else
            _testSuit.GetMock<IDependency1>().Invocations.Should().BeEmpty();
            _testSuit.GetMock<IDependency2>().Invocations.Should().BeEmpty();
        }

        [Test]
        public void UsingTraditionalSetups_ShouldCorrectlySetupMocks()
        {
            //arrange

            //act
            _testSuit
                .SetupMockAdv<IDependency1>(d => d.Setup(m => m.GetNumber()).Returns("one"))
                .SetupMockAdv<IDependency2>(d => d.Setup(m => m.Action2()).Returns("two"));

            //assert
            _testSuit.Sut.Dependency1.GetNumber().Should().Be("one");
            _testSuit.Sut.Dependency2.Action2().Should().Be("two");
        }

        [Test]
        public void UsingExpressionSetups_ShouldCorrectlySetupMocks()
        {
            //arrange

            //act
            _testSuit
                .SetupMock<IDependency1>(d => d.GetNumber() == "one")
                .SetupMock<IDependency2>(d => d.Action2() == "two");

            //assert
            _testSuit.Sut.Dependency1.GetNumber().Should().Be("one");
            _testSuit.Sut.Dependency2.Action2().Should().Be("two");
        }

        [Test]
        public void Always_ShouldCacheMocks()
        {
            //arrange

            //act

            //assert
            _testSuit.Sut.Dependency1.Should().Be(_testSuit.GetDependency<IDependency1>());
            _testSuit.Sut.Dependency2.Should().Be(_testSuit.GetDependency<IDependency2>());
        }

        [Test]
        public void SutCreated_CannotSetDependencyAsCustomMock()
        {
            //arrange
            _ = _testSuit.Sut;

            //act
            var action = _testSuit.Invoking(s => s.SetDependencyToCustomMock(new Mock<IDependency1>()));

            //assert
            action.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void SutNotCreated_SetsDependencyAsCustomMock()
        {
            //arrange
            var mock = new Mock<IDependency1>();

            //act
            _testSuit.SetDependencyToCustomMock(mock);

            //assert
            _testSuit.GetMock<IDependency1>().Should().Be(mock);
            _testSuit.GetDependency<IDependency1>().Should().Be(mock.Object);
            _testSuit.Sut.Dependency1.Should().Be(mock.Object);
        }

        [Test]
        public void SutNotCreated_SetsDependencyAsLooseMock()
        {
            //arrange

            //act
            _testSuit.SetDependencyToLooseMock<IDependency2>();

            //assert
            _testSuit.Sut.Dependency2.Invoking(d => d.Action2()).Should().NotThrow(" its a loose mock");
        }
    }
}
