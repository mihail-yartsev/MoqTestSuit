using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Moq;

namespace MoqTestSuit
{
    internal static class TestSuitQueryable<T> where T : class
    {
        private static readonly Lazy<Func<MethodCallExpression, IQueryable<T>>> _queryableCreator =
            new Lazy<Func<MethodCallExpression, IQueryable<T>>>(BuildQueryableCreator);

        private static readonly MethodInfo _createQueryableMethod =
            typeof(TestSuitQueryable<T>).GetMethod(nameof(CreateQueryable),
                BindingFlags.NonPublic | BindingFlags.Static);

        /// <summary>
        ///     Creates the mock query with the underlying queryable implementation.
        /// </summary>
        /// <param name="mock"></param>
        public static IQueryable<T> CreateMockQuery(Mock<T> mock)
        {
            var methodCallExpression = Expression.Call(null, _createQueryableMethod, Expression.Constant(mock));
            return _queryableCreator.Value.Invoke(methodCallExpression);
        }

        /// <summary>Wraps the enumerator inside a queryable.</summary>
        private static IQueryable<T> CreateQueryable(Mock<T> mock)
        {
            return CreateMocks(mock).AsQueryable();
        }

        /// <summary>
        ///     Method that is turned into the actual call from .Query{T}, to
        ///     transform the queryable query into a normal enumerable query.
        ///     This method is never used directly by consumers.
        /// </summary>
        /// <param name="mock"></param>
        private static IEnumerable<T> CreateMocks(Mock<T> mock)
        {
            while (true)
            {
                yield return mock.Object;
            }
            // ReSharper disable once IteratorNeverReturns
        }

        private static Func<MethodCallExpression, IQueryable<T>> BuildQueryableCreator()
        {
            var suit = Expression.Parameter(typeof(MethodCallExpression));
            var constructor = typeof(Mock).Assembly.GetType("Moq.Linq.MockQueryable`1")
                                  .MakeGenericType(typeof(T))
                                  .GetConstructor(new[] {typeof(MethodCallExpression)})
                              ?? throw new Exception($"Constructor Moq.Linq.MockQueryable<{typeof(T).Name}>(MethodCallExpression) was not found");
            var ctor = Expression.New(constructor, suit);
            return Expression.Lambda<Func<MethodCallExpression, IQueryable<T>>>(ctor, suit).Compile();
        }
    }
}