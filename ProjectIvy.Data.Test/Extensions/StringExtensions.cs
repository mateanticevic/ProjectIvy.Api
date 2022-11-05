using ProjectIvy.Data.Extensions;

namespace ProjectIvy.Data.Test.Extensions;

public class UnitTest1
{
    [Theory]
    [InlineData("Good & Bad", "good-and-bad")]
    [InlineData("červeni", "cerveni")]
    [InlineData("Toña", "tona")]
    [InlineData("SeDaM-i osam", "sedam-i-osam")]
    public void ToValueId(string input, string expected)
    {
        Assert.Equal(expected, input.ToValueId());
    }
}
