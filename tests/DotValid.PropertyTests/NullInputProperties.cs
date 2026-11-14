using static DotValid.Prelude;

namespace DotValid.PropertyTests;

/// <summary>
/// V4: <see langword="null"/> is never a value or an error, and an invalid case is never empty (V1). Both are rejected
/// where they would enter a validation, with the name of the parameter that carried them.
/// </summary>
public sealed class NullInputProperties
{
    [Fact]
    public void Valid_rejects_null()
    {
        AssertRejected<ArgumentNullException>("value", () => valid<string>(null!));
        AssertRejected<ArgumentNullException>("value", () => new Valid<string>(null!));
    }

    [Fact]
    public void Invalid_rejects_a_null_error()
    {
        AssertRejected<ArgumentNullException>("error", () => invalid((string)null!));
        AssertRejected<ArgumentNullException>("error", () => new Invalid<string>((string)null!));
    }

    [Fact]
    public void Invalid_rejects_a_null_among_several_errors()
    {
        AssertRejected<ArgumentException>("errors", () => invalid("first", null!), "cannot be null");
        AssertRejected<ArgumentException>("errors", () => new Invalid<string>("first", null!), "cannot be null");
    }

    [Fact]
    public void Invalid_rejects_no_errors()
    {
        AssertRejected<ArgumentException>("errors", () => invalid<string>(), "at least one error");
        AssertRejected<ArgumentException>("errors", () => invalid<string>([]), "at least one error");
        AssertRejected<ArgumentException>("errors", () => new Invalid<string>([]), "at least one error");
    }

    [Fact]
    public void Converting_a_default_valid_case_that_holds_null_is_rejected()
    {
        AssertRejected<ArgumentException>("value", () => Convert(default(Valid<string>)), "valid(value)");

        static Validated<string, int> Convert(Valid<string> success) => success;
    }

    [Fact]
    public void Converting_a_default_invalid_case_that_holds_no_errors_is_rejected()
    {
        AssertRejected<ArgumentException>("value", () => Convert(default(Invalid<int>)), "invalid(error)");

        static Validated<string, int> Convert(Invalid<int> failure) => failure;
    }

    // The message explains the rule, or how to create the case properly.
    private static void AssertRejected<TException>(string parameter, Func<object> create, string? explanation = null)
        where TException : ArgumentException
    {
        var exception = Assert.Throws<TException>(create);

        Assert.Equal(parameter, exception.ParamName);
        if (explanation is not null)
        {
            Assert.Contains(explanation, exception.Message, StringComparison.Ordinal);
        }
    }
}
