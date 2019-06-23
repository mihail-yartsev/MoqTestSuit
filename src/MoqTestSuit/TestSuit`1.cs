using System;
using System.Linq;
using System.Linq.Expressions;
using Moq;

namespace MoqTestSuit
{
    /// <summary>
    /// The main type representing the test suit.
    /// Contains methods to setup the dependencies and the <see cref="Sut"/> property
    /// to get instance of type to be tested.
    /// </summary>
    public class TestSuit<TSut> : TestSuit, ITestSuit<TSut> where TSut : class
    {
        private Lazy<TSut> _sut;

        /// <summary>
        /// Instance of type which is being tested (the root of the dependency tree)
        /// </summary>
        public TSut Sut => _sut.Value;

        /// <summary>
        /// Gets value if Sut was already created
        /// </summary>
        public bool IsSutCreated => _sut.IsValueCreated;

        internal TestSuit()
        {
            _sut = CreateSutLazy();
        }

        /// <summary>
        /// Setup mocks as you usually do in Mock.Of().
        /// Can be called several times for the same dependency, which results in applying them to the same object.
        /// This is different from the original Moq library API, where this can be done only once on the mock creation.
        /// </summary>
        public ITestSuit<TSut> SetupMock<TDependency>(Expression<Func<TDependency, bool>> setup)
            where TDependency : class
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed - it has side effects
            TestSuitQueryable<TDependency>.CreateMockQuery(GetMock<TDependency>()).First(setup);
            return this;
        }

        /// <summary>
        /// Allows some advanced setup of mocks in the test. Also should be used to setup void methods.
        /// </summary>
        public ITestSuit<TSut> SetupMockAdv<TDependency>(params Action<Mock<TDependency>>[] setups) where TDependency : class
        {
            var mock = GetMock<TDependency>();
            foreach (var setup in setups)
            {
                setup(mock);
            }

            return this;
        }

        /// <summary>
        /// Allows some advanced setup of test suit dependencies in the test
        /// </summary>
        public ITestSuit<TSut> SetupDependencySuit<TDependency>(params Action<ITestSuit<TDependency>>[] setups) 
            where TDependency : class
        {
            var suit = GetSuit<TDependency>();
            foreach (var setup in setups)
            {
                setup(suit);
            }

            return this;
        }
        
        /// <summary>
        /// Setups a dependency to an instance of a real type
        /// </summary>
        public ITestSuit<TSut> SetDependencyToInstance<TDependency, TInstance>(TInstance dependencyInstance) 
            where TDependency : class where TInstance: class, TDependency
        {
            switch (dependencyInstance)
            {
                case IMocked _ : throw new ArgumentException("Dependency instance is a mocked object instance. To set a dependency to a mock use " + nameof(SetDependencyToCustomMock) + "()");
                case Mock _ : throw new ArgumentException("Dependency instance is a mock object instance. To set a dependency to a mock use " + nameof(SetDependencyToCustomMock) + "()");
                case ITestSuit _ : throw new ArgumentException("Dependency instance is a TestSuit object instance. To set a dependency to a TestSuit use " + nameof(SetDependencyToTestSuit) + "()");
            }

            SetDependencyToInstanceCore<TDependency>(dependencyInstance);
            return this;
        }

        /// <summary>
        /// Setups a dependency to an instance of custom mock
        /// </summary>
        public ITestSuit<TSut> SetDependencyToCustomMock<TDependency>(IMock<TDependency> mock) 
            where TDependency : class
        {
            SetDependencyToInstanceCore<TDependency>(mock);
            return this;
        }

        /// <summary>
        /// Setups a dependency to a loose mock
        /// </summary>
        public ITestSuit<TSut> SetDependencyToLooseMock<TDependency>() where TDependency : class
        {
            return SetDependencyToCustomMock(new Mock<TDependency>(MockBehavior.Loose));
        }

        /// <summary>
        /// Setups a dependency to be an instance of another suit
        /// </summary>
        public ITestSuit<TSut> SetDependencyToTestSuit<TDependency>(ITestSuit<TDependency> suit)
            where TDependency : class
        {
            SetDependencyToInstanceCore<TDependency>(suit);
            return this;
        }

        /// <summary>
        /// Setups a dependency to be an instance of another suit which is created from <typeparamref name="TImpl"/>
        /// </summary>
        public ITestSuit<TSut> SetDependencyToTestSuit<TDependency, TImpl>()
            where TDependency : class where TImpl : class, TDependency
        {
            return SetDependencyToTestSuit<TDependency>(Create<TImpl>());
        }

        /// <summary>
        /// Resets all mocks and suits to the clean state and removes all the real instances
        /// You could create a new test suit for each test, but using Reset() after (or before) each test
        /// is more performant.
        /// Becomes meaningful when you have some hundreds of tests 
        /// </summary>
        public override void Reset()
        {
            _sut = CreateSutLazy();
            base.Reset();
        }

        private Lazy<TSut> CreateSutLazy()
        {
            return new Lazy<TSut>(() => ObjectProviderCache<TSut>.CreateInstance.Value.Invoke(this));
        }

        private protected override void SetDependencyToInstanceCore<TDependency>(object dependencyInstance)
        {
            if (_sut.IsValueCreated)
            {
                throw new InvalidOperationException(
                    "The System Under Test instance has already been created and all dependencies have been injected." +
                    "You cannot change the references to the dependencies after that. " +
                    "You should set the dependencies instances before accessing the Sut property to avoid its creation before setting the dependencies." +
                    "You can also call the Reset() method to reset all the state and the Sut instance.");
            }

            base.SetDependencyToInstanceCore<TDependency>(dependencyInstance);
        }
    }
}