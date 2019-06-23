using System;
using System.Collections.Concurrent;
using Moq;

namespace MoqTestSuit
{
    /// <summary>
    /// Non-generic base type for the test suit, containing methods independent of the type being tested
    /// </summary>
    public abstract class TestSuit : ITestSuit
    {
        private readonly ConcurrentDictionary<Type, DependencyContainer> _dependencies = new ConcurrentDictionary<Type, DependencyContainer>();

        /// <summary>
        /// Creates a <see cref="TestSuit"/> to test the type <typeparamref name="TSut"/>
        /// </summary>
        public static ITestSuit<TSut> Create<TSut>() where TSut : class
        {
            return new TestSuit<TSut>();
        }

        /// <summary>
        /// Gets the mock instance for the type
        /// </summary>  
        public Mock<TDependency> GetMock<TDependency>() where TDependency : class 
            => GetOrAddDependencyAsMock<TDependency>().GetMock<TDependency>();

        /// <summary>
        /// Gets the TestSuit instance for the type
        /// </summary>
        public ITestSuit<TDependency> GetSuit<TDependency>() where TDependency : class 
            => _dependencies.TryGetValue(typeof(TDependency), out var container) 
                ? container.GetSuit<TDependency>()
                : throw new InvalidOperationException($"Dependency {typeof(TDependency)} is not set yet");

        /// <summary>
        /// Gets the dependency instance
        /// </summary>
        public TDependency GetDependency<TDependency>() where TDependency : class 
            => GetOrAddDependencyAsMock<TDependency>().GetInstance<TDependency>();

        /// <summary>
        /// Resets all mocks and suits to the clean state and removes all the real instances
        /// You could create a new test suit for each test, but using Reset() after (or before) each test
        /// is more performant.
        /// Becomes meaningful when you have some hundreds of tests 
        /// </summary>
        public virtual void Reset()
        {
            foreach (var pair in _dependencies.ToArray())
            {
                if (pair.Value.IsMock)
                {
                    pair.Value.GetMock().Reset();
                } 
                else if (pair.Value.IsSuit)
                {
                    pair.Value.GetSuit().Reset();
                }
                else
                {
                    _dependencies.TryRemove(pair.Key, out _);
                }
            }
        }

        private DependencyContainer GetOrAddDependencyAsMock<TDependency>() where TDependency : class
            => _dependencies.GetOrAdd(typeof(TDependency), t => new DependencyContainer(ObjectProviderCache<TDependency>.CreateMock.Value()));

        private protected virtual void SetDependencyToInstanceCore<TDependency>(object dependencyInstance) where TDependency : class
        {
            var dependencyContainer = new DependencyContainer(dependencyInstance);
            dependencyContainer.ValidateDependencyIsAssignableTo<TDependency>();
            _dependencies[typeof(TDependency)] = dependencyContainer;
        }
    }
}