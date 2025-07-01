using System.Reflection;
using DotValid;

namespace dotValid.Tests.Unit;

public sealed class ValidatedMatchTests
{
    [Fact]
    public void Match_WhenSuccessState_ShouldCallSuccessFunction()
    {
        Validated<string> validated = Validation.Succeed("Success");
        Assert.True(validated.Match(_ => false, _ => true));
    }

    [Fact]
    public void Match_WhenFailedState_ShouldCallErrorFunction()
    {
        Validated<string> validated = Validation.Failed<string>([]);
        Assert.False(validated.Match(_ => false, _ => true));
    }

    [Fact]
    public void Match_WithInvalidState_ThrowsInvalidOperationException()
    {
        Validated<string> validated = CreateValidatedWithInvalidState();

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() =>
            validated.Match(
                _ => "Error case",
                _ => "Success case"
            )
        );

        Assert.Equal("Impossible state: This part of the code should never be reached.", exception.Message);
    }

    /// <summary>
    /// Helper method to create a Validated instance with an invalid internal state
    /// using reflection to bypass normal initialization paths.
    /// </summary>
    private static Validated<string> CreateValidatedWithInvalidState()
    {
        Validated<string> validated = Validation.Succeed("Success");

        FieldInfo? field = validated
            .GetType()
            .GetField(
                "_validated",
                BindingFlags.NonPublic | BindingFlags.Instance);

        object boxes = validated;
        field?.SetValue(boxes, null);
        return (Validated<string>)boxes;
    }
}
