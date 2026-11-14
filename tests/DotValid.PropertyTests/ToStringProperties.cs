using System.Diagnostics;
using FsCheck;
using FsCheck.Xunit;
using static DotValid.Prelude;

namespace DotValid.PropertyTests;

/// <summary>
/// The text form used in assertion messages, logs and the debugger: <c>Valid(value)</c>, <c>Invalid([e1, e2])</c>, or
/// <c>Uninitialized</c> for <c>default</c>.
/// </summary>
public sealed class ToStringProperties
{
    [Property]
    public void A_valid_validation_is_shown_as_Valid_of_the_value(int value) =>
        Assert.Equal($"Valid({value})", Validations.Valid<int, string>(value).ToString());

    [Property]
    public void An_invalid_validation_is_shown_as_Invalid_of_every_error_in_order(NonEmptyArray<int> errors) =>
        Assert.Equal(
            $"Invalid([{string.Join(", ", errors.Get)}])",
            Validations.Invalid<string, int>(errors.Get).ToString());

    [Property]
    public void The_cases_are_shown_like_the_validations_that_hold_them(int value, NonNull<string> failure)
    {
        Assert.Equal($"Valid({value})", valid(value).ToString());
        Assert.Equal($"Invalid([{failure.Get}])", invalid(failure.Get).ToString());
        Assert.Equal($"Valid({value})", $"{valid(value)}");
    }

    [Fact]
    public void A_single_error_and_several_errors_read_naturally()
    {
        Assert.Equal("Invalid([empty])", Validations.Invalid<int, string>("empty").ToString());
        Assert.Equal("Invalid([empty, too short])", Validations.Invalid<int, string>("empty", "too short").ToString());
    }

    [Fact]
    public void An_uninitialized_validation_and_a_default_case_do_not_throw()
    {
        Assert.Equal("Uninitialized", default(Validated<int, string>).ToString());
        Assert.Equal("Invalid([])", default(Invalid<string>).ToString());
    }

    [Fact]
    public void Nested_validations_show_every_level()
    {
        Assert.Equal(
            "Valid(Valid(42))",
            Validations.Valid<Validated<int, string>, string>(Validations.Valid<int, string>(42)).ToString());
        Assert.Equal(
            "Valid(Invalid([inner]))",
            Validations.Valid<Validated<int, string>, string>(Validations.Invalid<int, string>("inner")).ToString());
        Assert.Equal("Invalid([outer])", Validations.Invalid<Validated<int, string>, string>("outer").ToString());
    }

    [Theory]
    [InlineData(typeof(Validated<int, string>))]
    [InlineData(typeof(Valid<int>))]
    [InlineData(typeof(Invalid<string>))]
    public void The_debugger_shows_the_same_text_without_quotes(Type type)
    {
        var display = type
            .GetCustomAttributes(typeof(DebuggerDisplayAttribute), inherit: false)
            .Cast<DebuggerDisplayAttribute>()
            .Single();

        Assert.Equal("{ToString(),nq}", display.Value);
    }
}
