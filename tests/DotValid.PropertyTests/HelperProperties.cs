using FsCheck;
using FsCheck.Xunit;
using static DotValid.Prelude;

namespace DotValid.PropertyTests;

/// <summary>
/// The small building blocks (V13): <c>Check</c> turns a predicate into a validation, <c>FromNullable</c> turns
/// <see langword="null"/> into an error, and <see cref="ValidationError"/> is a ready-made error type.
/// </summary>
public sealed class HelperProperties
{
    // ---- Check ----

    [Property]
    public void Check_is_valid_then_Ensure(int value, Func<int, bool> predicate, NonNull<string> error)
    {
        Assert.Equal(Validations.Valid<int, string>(value).Ensure(predicate, error.Get), Validated.Check(value, predicate, error.Get));
        Assert.Equal(
            Validations.Valid<int, string>(value).Ensure(predicate, number => $"{number} {error.Get}"),
            Validated.Check(value, predicate, number => $"{number} {error.Get}"));
    }

    [Property]
    public void The_predicate_runs_once_and_the_factory_only_when_it_fails(int value, bool holds)
    {
        var predicateCalls = 0;
        var factoryCalls = 0;

        _ = Validated.Check(value, _ => ++predicateCalls > 0 && holds, _ => $"{++factoryCalls}");

        Assert.Equal(1, predicateCalls);
        Assert.Equal(holds ? 0 : 1, factoryCalls);
    }

    [Fact]
    public void Check_rejects_missing_arguments_and_a_null_from_the_factory()
    {
        // A predicate that fails: without the check, a null value would quietly become an invalid validation.
        AssertRejects("value", () => Validated.Check((string)null!, _ => false, "error"));
        AssertRejects("predicate", () => Validated.Check(1, null!, "error"));
        AssertRejects("error", () => Validated.Check(1, _ => true, (string)null!));
        AssertRejects("value", () => Validated.Check((string)null!, _ => false, _ => "error"));
        AssertRejects("predicate", () => Validated.Check(1, null!, _ => "error"));
        AssertRejects("error", () => Validated.Check(1, _ => true, (Func<int, string>)null!));
        Assert.Throws<InvalidOperationException>(() => Validated.Check(1, _ => false, _ => (string)null!));
    }

    // ---- FromNullable ----

    [Property]
    public void FromNullable_of_a_value_type_agrees_with_HasValue(int? value, NonNull<string> missing)
    {
        var expected = value.HasValue ? Validations.Valid<int, string>(value.Value) : Validations.Invalid<int, string>(missing.Get);

        Assert.Equal(expected, Validated.FromNullable(value, missing.Get));
        Assert.Equal(expected, Validated.FromNullable(value, () => missing.Get));
    }

    [Property]
    public void FromNullable_of_a_reference_turns_null_into_the_error(NonNull<string> text, bool isNull, int missing)
    {
        var value = isNull ? null : text.Get;
        var expected = value is null ? Validations.Invalid<string, int>(missing) : Validations.Valid<string, int>(value);

        Assert.Equal(expected, Validated.FromNullable(value, missing));
        Assert.Equal(expected, Validated.FromNullable(value, () => missing));
    }

    [Property]
    public void The_error_factory_runs_once_for_null_and_never_otherwise(int? number, NonNull<string> text, bool isNull)
    {
        var reference = isNull ? null : text.Get;
        var numberCalls = 0;
        var referenceCalls = 0;

        _ = Validated.FromNullable(number, () => ++numberCalls);
        _ = Validated.FromNullable(reference, () => ++referenceCalls);

        Assert.Equal(number.HasValue ? 0 : 1, numberCalls);
        Assert.Equal(isNull ? 1 : 0, referenceCalls);
    }

    [Fact]
    public void FromNullable_rejects_missing_errors_and_a_null_from_the_factory()
    {
        AssertRejects("error", () => Validated.FromNullable((int?)1, (string)null!));
        AssertRejects("error", () => Validated.FromNullable("text", (string)null!));
        AssertRejects("error", () => Validated.FromNullable((int?)1, (Func<string>)null!));
        AssertRejects("error", () => Validated.FromNullable("text", (Func<string>)null!));
        Assert.Throws<InvalidOperationException>(() => Validated.FromNullable((int?)null, () => (string)null!));
        Assert.Throws<InvalidOperationException>(() => Validated.FromNullable((string?)null, () => (string)null!));
    }

    // ---- ValidationError ----

    [Property]
    public void A_validation_error_is_shown_as_property_and_message(NonNull<string> property, NonNull<string> message) =>
        Assert.Equal($"{property.Get}: {message.Get}", new ValidationError(property.Get, message.Get).ToString());

    [Property]
    public void Validation_errors_are_equal_when_both_parts_are(string property, string message, string other)
    {
        property ??= "";
        message ??= "";
        other ??= "";

        Assert.Equal(new ValidationError(property, message), new ValidationError(property, message));
        Assert.Equal(message == other, new ValidationError(property, message) == new ValidationError(property, other));
        var (deconstructedProperty, deconstructedMessage) = new ValidationError(property, message);
        Assert.Equal((property, message), (deconstructedProperty, deconstructedMessage));
    }

    [Fact]
    public void A_validation_error_rejects_null_parts()
    {
        AssertRejects("Property", () => new ValidationError(null!, "message"));
        AssertRejects("Message", () => new ValidationError("Name", null!));
    }

    [Fact]
    public void Validation_errors_accumulate_like_any_error()
    {
        var name = Validated.Check("", text => text.Length > 0, new ValidationError("Name", "is empty"));
        var age = Validated.Check(-1, number => number >= 0, new ValidationError("Age", "is negative"));

        var person = Validated.Combine(name, age, (n, a) => $"{n} {a}");

        Assert.Equal("Invalid([Name: is empty, Age: is negative])", person.ToString());
        _ = invalid(new ValidationError("Form", "is incomplete"));
    }

    private static void AssertRejects(string parameter, Func<object> call) =>
        Assert.Equal(parameter, Assert.Throws<ArgumentNullException>(call).ParamName);
}
