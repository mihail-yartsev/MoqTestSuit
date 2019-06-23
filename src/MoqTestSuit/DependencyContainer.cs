using System;
using System.Diagnostics.Contracts;
using Moq;

namespace MoqTestSuit
{
    /// <summary>
    /// Stores mocks as mocks, suits as suits and real dependencies as is
    /// </summary>
    internal struct DependencyContainer
    {
        public DependencyContainer(object obj)
        {
            Obj = obj switch
            {
                null => throw new ArgumentNullException(nameof(obj)),
                IMocked mocked => mocked.Mock, // unpack the mocked object to a mock
                object o => o,
            };
        }

        public object Obj { get; }

        public bool IsMock => Obj is Mock;
        public bool IsSuit => Obj is TestSuit;

        [Pure]
        public Mock<TDep> GetMock<TDep>() where TDep : class =>
            Obj switch
            {
                Mock<TDep> typedMock => typedMock,
                Mock _ => throw new InvalidOperationException($"The mock is of type {Obj.GetType()} which is not assignable to the requested dependency type {typeof(Mock<TDep>)}"),
                _ => throw new InvalidOperationException($"Dependency {Obj.GetType()} is not a mock"),
            };

        [Pure]
        public Mock GetMock() =>
            Obj switch
            {
                Mock mock => mock,
                _  => throw new InvalidOperationException($"Dependency {Obj.GetType()} is not a Mock"),
            };
            
        [Pure]
        public TDep GetInstance<TDep>() where TDep : class =>
            Obj switch
            {
                TDep dep => dep,
                ITestSuit<TDep> suit => suit.Sut,
                IMock<TDep> mock => mock.Object,
                _ => throw new InvalidOperationException($"The dependency is of type {Obj.GetType()} which cannot be used to obtain to the requested dependency type {typeof(TDep)}"),
            };

        public void ValidateDependencyIsAssignableTo<TDep>() where TDep : class
        {
            switch (Obj)
            {
                case TDep _: break;
                case ITestSuit<TDep> _: break;
                case IMock<TDep> _: break;
                default:
                    throw new InvalidOperationException(
                        $"The dependency is of type {Obj.GetType()} which cannot be used to obtain to the requested dependency type {typeof(TDep)}");
            }
        }

        [Pure]
        public ITestSuit<TDep> GetSuit<TDep>() where TDep : class =>
            Obj switch
            {
                ITestSuit<TDep> suit => suit,
                ITestSuit _ => throw new InvalidOperationException($"The dependency is of type {Obj.GetType()} which cannot be used to obtain to the requested suit for dependency type {typeof(TestSuit<TDep>)}"),
                _ => throw new InvalidOperationException($"Dependency {Obj.GetType()} is not a test suit"),
            };

        [Pure]
        public TestSuit GetSuit() =>
            Obj switch
            {
                TestSuit suit => suit,
                _ => throw new InvalidOperationException($"The dependency is of type {Obj.GetType()} and is not a TestSuit"),
            };
    }
}