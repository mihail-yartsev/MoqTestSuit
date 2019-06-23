namespace MoqTestSuit.Tests.TestServices
{
    public class Dependency1 : IDependency1
    {
        private readonly IDependency2 _dependency2;

        public Dependency1(IDependency2 dependency2)
        {
            _dependency2 = dependency2;
        }

        public string GetNumber()
        {
            return _dependency2.Action2();
        }

        public int SomethingElse => -1;
    }
}