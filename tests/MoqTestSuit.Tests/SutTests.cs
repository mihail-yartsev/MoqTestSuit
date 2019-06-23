using FluentAssertions;
using MoqTestSuit.Tests.TestServices;
using NUnit.Framework;

namespace MoqTestSuit.Tests
{
    public class SutTests
    {
        private static readonly ITestSuit<MyService> _testSuit = TestSuit.Create<MyService>();

        [TearDown]
        public void AfterTest()
        {
            _testSuit.Reset();
        }

        [Test]
        public void Reset_SutShouldBeRecreated()
        {
            //arrange
            _testSuit.Sut.I = 10;

            //act
            _testSuit.Reset();

            //assert
            _testSuit.Sut.I.Should().Be(1);
        }

        [Test]
        public void Always_ShouldCacheSut()
        {
            //arrange

            //act
            var suit = TestSuit.Create<MyService>();
            var sut1 = suit.Sut;
            var sut2 = suit.Sut;

            //assert
            sut1.Should().Be(sut2);
        }
    }
}
