using Moq;

namespace MoqTestSuit
{
    /// <summary>
    /// Non-generic interface for the test suit, containing methods independent of the type being tested
    /// </summary>
    public interface ITestSuit
    {
        /// <summary>
        /// Gets the mock instance for the type
        /// </summary>  
        Mock<TDependency> GetMock<TDependency>() where TDependency : class;

        /// <summary>
        /// Gets the TestSuit instance for the type
        /// </summary>  
        ITestSuit<TDependency> GetSuit<TDependency>() where TDependency : class;

        /// <summary>
        /// Gets the dependency instance
        /// </summary>
        TDependency GetDependency<TDependency>() where TDependency : class;

        /// <summary>
        /// Resets all mocks and suits to the clean state and removes all the real instances
        /// You could create a new test suit for each test, but using Reset() after (or before) each test
        /// is more performant.
        /// Becomes meaningful when you have some hundreds of tests 
        /// </summary>
        void Reset();
    }
}