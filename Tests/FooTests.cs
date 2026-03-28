namespace Tests;

public class FooTests
{
    [Test]
    public void Add_ReturnsSumOfArguments()
    {
        Foo.Bar.Add(2, 3).Should().Be(5);
    }
}
