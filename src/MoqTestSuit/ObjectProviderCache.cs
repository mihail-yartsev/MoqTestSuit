using System;
using System.Linq;
using System.Linq.Expressions;
using Moq;

namespace MoqTestSuit
{
    internal class ObjectProviderCache<T> where T : class
    {
        public static readonly Lazy<Func<TestSuit, T>> CreateInstance =
            new Lazy<Func<TestSuit, T>>(BuildCreateInstance);

        public static readonly Lazy<Func<Mock<T>>> CreateMock = new Lazy<Func<Mock<T>>>(BuildCreateMock);

        private static Func<TestSuit, T> BuildCreateInstance()
        {
            var suit = Expression.Parameter(typeof(TestSuit));
            var constructor = typeof(T).GetConstructors()
                .Select(c => new {Info = c, Parameters = c.GetParameters()})
                .OrderByDescending(c => c.Parameters!.Length).First();

            var args = constructor.Parameters
                .Select(p => Expression.Call(suit, nameof(TestSuit.GetDependency), new[] {p.ParameterType}))
                .ToList();
            var ctor = Expression.New(constructor.Info, args);
            return Expression.Lambda<Func<TestSuit, T>>(ctor, suit).Compile();
        }

        private static Func<Mock<T>> BuildCreateMock()
        {
            var constructor = typeof(Mock<>).MakeGenericType(typeof(T)).GetConstructor(new[] {typeof(MockBehavior)})
                              ?? throw new Exception($"Constructor Mock<{typeof(T).Name}>(MockBehavior) was not found");
            var ctor = Expression.New(constructor, Expression.Constant(MockBehavior.Strict));
            return Expression.Lambda<Func<Mock<T>>>(ctor).Compile();
        }
    }
}