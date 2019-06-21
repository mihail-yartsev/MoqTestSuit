namespace MoqTestSuit.Tests.Examples
{
    public class MyService
    {
        public int I { get; set; } = 1;
        public IDependency1 Dependency1 { get; }
        public IDependency2 Dependency2 { get; }

        public MyService(IDependency1 dependency1, IDependency2 dependency2)
        {
            Dependency1 = dependency1;
            Dependency2 = dependency2;
        }

        public int ParseNumberFromDependency()
        {
            return int.Parse(Dependency1.GetNumber());
        }
    }
}