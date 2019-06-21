# MoqTestSuit
An opinionated helper adding syntaxic sugar around Moq library.
Main benefits compared to a vanilla Moq:
1. It automatically creates mocks for all constructor dependencies of the type being tested and injects them when it is created.
2. It allows the expressions style setup of Mock.Of<>() to be applied several types on the same mock object, instead of only once on mock creation
3. It serves as a repository of dependencies, you no longer need separate fields for each of them
4. Cleaner usage, less clutter to make a test


## Usage 
Add the library to your test project. 
Follow this example to build a test.

```csharp
// Example uses NUnit, but you can use anything instead
public class MyServiceTests
{
    // Create a field for the suit instance. MyService should be the type you are testing
    private static readonly TestSuit<MyService> _testSuit = TestSuit.Create<MyService>();

    [TearDown]
    public void TearDown()
    {
        // You could create a new test suit for each test, but using Reset() after (or before) each test
        // is more performant. Becomes meaningful when you have some hundreds of tests 
        _testSuit.Reset();
    }

    [Test]
    public void Always_ShouldParseTheNumber()
    {
        // Arrange

        // Setup your dependencies here as you do in Mock.Of<>()
        // If you call something in the test which was not setup you will get exception
        // "invocation failed with mock behavior Strict".
        _testSuit.Setup<IDependency1>(s => s.GetNumber() == "1" && s.SomethingElse == 2);

        // You can also use the traditional Moq form to setup advanced behaviour and void methods
        _testSuit.SetupAdv<IDependency2>(m =>
            m.Setup(d => d.Action2()).Callback(() => Console.WriteLine("Test")));

        // Act

        // Sut (system under test) is created lazily on first usage,
        // all constructor dependencies are automatically injected using mocks
        var result = _testSuit.Sut.ParseNumberFromDependency();

        // Assert
        Assert.AreEqual(1, result);
    }
}    
```
