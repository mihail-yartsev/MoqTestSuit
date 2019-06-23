using System;
using System.Linq.Expressions;
using Moq;

namespace MoqTestSuit
{
    /// <summary>
    /// Covariant interface of a suit. Can be used to represent TestSuit{Service} as ITestSuit{IService}.
    /// </summary>
    public interface ITestSuit<out TSut>: ITestSuit where TSut : class
    {
        /// <summary>
        /// Instance of type which is being tested (the root of the dependency tree)
        /// </summary>
        TSut Sut { get; }

        /// <summary>
        /// Setup mocks as you usually do in Mock.Of().
        /// Can be called several times for the same dependency, which results in applying them to the same object.
        /// This is different from the original Moq library API, where this can be done only once on the mock creation.
        /// </summary>
        ITestSuit<TSut> SetupMock<TDependency>(Expression<Func<TDependency, bool>> setup)
            where TDependency : class;

        /// <summary>
        /// Allows some advanced setup of mocks in the test. Also should be used to setup void methods.
        /// </summary>
        ITestSuit<TSut> SetupMockAdv<TDependency>(params Action<Mock<TDependency>>[] setups) where TDependency : class;

        /// <summary>
        /// Allows some advanced setup of test suit dependencies in the test
        /// </summary>
        ITestSuit<TSut> SetupDependencySuit<TDependency>(params Action<ITestSuit<TDependency>>[] setups) 
            where TDependency : class;

        /// <summary>
        /// Setups a dependency to an instance of a real type
        /// </summary>
        ITestSuit<TSut> SetDependencyToInstance<TDependency, TInstance>(TInstance dependencyInstance)
            where TDependency : class where TInstance : class, TDependency;

        /// <summary>
        /// Setups a dependency to an instance of custom mock
        /// </summary>
        ITestSuit<TSut> SetDependencyToCustomMock<TDependency>(IMock<TDependency> mock) 
            where TDependency : class;

        /// <summary>
        /// Setups a dependency to a loose mock
        /// </summary>
        ITestSuit<TSut> SetDependencyToLooseMock<TDependency>() 
            where TDependency : class;

        /// <summary>
        /// Setups a dependency to be an instance of another suit
        /// </summary>
        ITestSuit<TSut> SetDependencyToTestSuit<TDependency>(ITestSuit<TDependency> suit)
            where TDependency : class;

        /// <summary>
        /// Setups a dependency to be an instance of another suit which is created from <typeparamref name="TImpl"/>
        /// </summary>
        ITestSuit<TSut> SetDependencyToTestSuit<TDependency, TImpl>()
            where TDependency : class where TImpl : class, TDependency;
    }
}