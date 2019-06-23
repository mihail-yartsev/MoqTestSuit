using System;
using FluentAssertions;
using MoqTestSuit.Tests.TestServices;
using NUnit.Framework;

namespace MoqTestSuit.Tests
{
    public class TestSuitDependenciesTests
    {
        private static readonly ITestSuit<MyService> _testSuit = TestSuit.Create<MyService>();

        [TearDown]
        public void AfterTest()
        {
            _testSuit.Reset();
        }

        [Test]
        public void SutCreated_CannotSetDependencyToTestSuit()
        {
            //arrange
            _ = _testSuit.Sut;

            //act
            var action = _testSuit.Invoking(s => s.SetDependencyToTestSuit<IDependency1>(TestSuit.Create<Dependency1>()));

            //assert
            action.Should().Throw<InvalidOperationException>();
        }

        [Test]
        public void SutNotCreated_SetsDependencyToTestSuitInstance()
        {
            //arrange
            var dependencyTestSuit = TestSuit.Create<Dependency1>();

            //act
            _testSuit.SetDependencyToTestSuit<IDependency1>(dependencyTestSuit);

            //assert
            _testSuit.GetSuit<IDependency1>().Should().Be(dependencyTestSuit);
            _testSuit.GetDependency<IDependency1>().Should().Be(dependencyTestSuit.Sut);
            _testSuit.Sut.Dependency1.Should().Be(dependencyTestSuit.Sut);
        }

        [Test]
        public void SutNotCreated_SetsDependencyToTestSuitType()
        {
            //arrange

            //act
            _testSuit.SetDependencyToTestSuit<IDependency1, Dependency1>();

            //assert
            _testSuit.Sut.Dependency1.Should().Be(_testSuit.GetSuit<IDependency1>().Sut);
            _testSuit.Sut.Dependency1.Should().BeOfType<Dependency1>();
        }

        [Test]
        public void DependencySetAsSuitInstance_CanConfigureIt()
        {
            //arrange
            _testSuit.SetDependencyToTestSuit<IDependency1>(TestSuit.Create<Dependency1>());

            //act
            _testSuit.SetupDependencySuit<IDependency1>(s => s.SetupMock<IDependency2>(d => d.Action2() == "42"));

            //assert
            // note: ParseNumberFromDependency() calls Dependency1.GetNumber() which in turn calls
            // IDependency2.Action2() which we mocked above
            _testSuit.Sut.ParseNumberFromDependency().Should().Be(42);
        }

        [Test]
        public void Reset_ResetsTestSuit()
        {
            //arrange
            var dependencyTestSuit = (TestSuit<Dependency1>)TestSuit.Create<Dependency1>();
            _testSuit.SetDependencyToTestSuit<IDependency1>(dependencyTestSuit);
            _ = dependencyTestSuit.Sut; // create the dependency object

            //act
            _testSuit.Reset();

            //assert
            dependencyTestSuit.IsSutCreated.Should().BeFalse();
        }
    }
}
