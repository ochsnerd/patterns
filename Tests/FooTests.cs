namespace Tests;

public class FooTests
{
    [Test]
    public void AddReturnsSumOfArguments()
    {
        TypedId.Foo.Add(2, 3).Should().Be(5);
    }
}
