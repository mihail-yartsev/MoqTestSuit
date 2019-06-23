using System;
using FluentAssertions;
using Moq;
using MoqTestSuit.Tests.TestServices;
using NUnit.Framework;

namespace MoqTestSuit.Tests
{
    public class InstanceTests
    {
        private static readonly ITestSuit<MyService> _testSuit = TestSuit.Create<MyService>();

        [TearDown]
        public void AfterTest()
        {
            _testSuit.Reset();
        }

        [Test]
        public void SutCreated_CannotSetDependencyInstance()
        {
            //arrange
            _ = _testSuit.Sut;

            //act
            var action = _testSuit.Invoking(s =>
                s.SetDependencyToInstance<IDependency1, Dependency1>(new Dependency1(null)));

            //assert
            action.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void SetInstanceToMocked_Throws()
        {
            //arrange

            //act
            var action = _testSuit.Invoking(s =>
                s.SetDependencyToInstance<IDependency1, IDependency1>(Mock.Of<IDependency1>()));

            //assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void SetInstanceToMock_Throws()
        {
            //arrange

            //act
            var action = _testSuit.Invoking(s =>
                s.SetDependencyToInstance<Mock<IDependency1>, Mock<IDependency1>>(new Mock<IDependency1>()));

            //assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void SetInstanceToSut_Throws()
        {
            //arrange

            //act
            var action = _testSuit.Invoking(s =>
                s.SetDependencyToInstance<ITestSuit<IDependency1>, ITestSuit<Dependency1>>(TestSuit.Create<Dependency1>()));

            //assert
            action.Should().Throw<ArgumentException>();
        }

        [Test]
        public void SutNotCreated_SetsDependencyInstance()
        {
            //arrange
            var dependencyInstance = new Dependency1(null);

            //act
            _testSuit.SetDependencyToInstance<IDependency1, Dependency1>(dependencyInstance);

            //assert
            _testSuit.GetDependency<IDependency1>().Should().Be(dependencyInstance);
            _testSuit.Sut.Dependency1.Should().Be(dependencyInstance);
        }

        [Test]
        public void Reset_CleansDependencyInstance()
        {
            //arrange
            var dependencyInstance = new Dependency1(null);
            _testSuit.SetDependencyToInstance<IDependency1, Dependency1>(dependencyInstance);

            //act
            _testSuit.Reset();

            //assert
            _testSuit.GetDependency<IDependency1>().Should().BeAssignableTo<IMocked>();
            _testSuit.Sut.Dependency1.Should().BeAssignableTo<IMocked>();
        }
    }
}