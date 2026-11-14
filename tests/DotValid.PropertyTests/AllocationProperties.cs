using FsCheck.Xunit;
using static DotValid.Prelude;

namespace DotValid.PropertyTests;

/// <summary>
/// V9: for value types <c>T</c> and <c>TError</c>, working with a validation allocates nothing on the valid path. Every
/// delegate below is a <c>static</c> lambda or captures only what exists before the measurement, so any allocation
/// comes from the library or from the code the compiler generates for unions.
/// </summary>
/// <remarks>
/// Skipped in Debug builds (see <see cref="Allocations.SkipInDebug"/>); CI runs them in Release.
/// Not covered on purpose: creating an invalid case, mapping its errors or accumulating errors (each builds an array of
/// errors), <c>Equals(object)</c> (boxes
/// its argument), and <c>validated is null</c> / <c>is not null</c> on an initialized validation (the compiler reads the
/// union's <c>Value</c>, which boxes the case). Reading an existing invalid case allocates nothing.
/// </remarks>
public sealed class AllocationProperties
{
    [Property(Skip = Allocations.SkipInDebug)]
    public void Creating_a_valid_validation_allocates_nothing(int value)
    {
        Allocations.AssertNone("valid", () => _ = valid(value));
        Allocations.AssertNone("new Valid", () => _ = new Valid<int>(value));
        Allocations.AssertNone("conversion from Valid", () => _ = Succeed(value));
        Allocations.AssertNone("conversion in generic code", () => _ = SucceedGeneric<int, int>(value));
    }

    [Property(Skip = Allocations.SkipInDebug)]
    public void Converting_an_existing_invalid_case_allocates_nothing(int failure)
    {
        var existing = invalid(failure);

        Allocations.AssertNone("conversion from Invalid", () => _ = Fail(existing));
    }

    [Property(Skip = Allocations.SkipInDebug)]
    public void Pattern_matching_allocates_nothing(Validated<int, int> validated)
    {
        Allocations.AssertNone(
            "switch (positional)",
            () => _ = validated switch
            {
                Valid<int>(var value) => value,
                Invalid<int>(var errors) => -errors.Length,
            });
        Allocations.AssertNone(
            "switch (type patterns)",
            () => _ = validated switch
            {
                Valid<int> success => success.Value,
                Invalid<int> failure => -failure.Errors[0],
            });
        Allocations.AssertNone("is Valid", () => _ = validated is Valid<int>(var value) && value > 0);
        Allocations.AssertNone("is Invalid", () => _ = validated is Invalid<int>);
        Allocations.AssertNone("switch (generic)", () => _ = Describe(validated));
    }

    [Property(Skip = Allocations.SkipInDebug)]
    public void Reading_allocates_nothing(Validated<int, int> validated)
    {
        Allocations.AssertNone("Match", () => _ = validated.Match(static value => value, static errors => -errors.Length));
        Allocations.AssertNone("OrDefault", () => _ = validated.OrDefault(0));
        Allocations.AssertNone("OrDefault(factory)", () => _ = validated.OrDefault(static errors => errors[0]));
    }

    [Property(Skip = Allocations.SkipInDebug)]
    public void Transforming_a_valid_validation_allocates_nothing(int value)
    {
        var validated = Validations.Valid<int, int>(value);

        Allocations.AssertNone("Map", () => _ = validated.Map(static number => number + 1));
        Allocations.AssertNone("MapError", () => _ = validated.MapError(static error => (long)error));
        Allocations.AssertNone("Then", () => _ = validated.Then(static number => Validations.Valid<int, int>(number + 1)));
        Allocations.AssertNone("Ensure (kept)", () => _ = validated.Ensure(static _ => true, -1));
        Allocations.AssertNone("Ensure(factory) (kept)", () => _ = validated.Ensure(static _ => true, static number => -number));
        Allocations.AssertNone("Select", () => _ = validated.Select(static number => number + 1));
        Allocations.AssertNone("query", () => _ = from number in validated select number + 1);
    }

    [Property(Skip = Allocations.SkipInDebug)]
    public void Combining_valid_validations_allocates_nothing(int value, int other)
    {
        var validated = Validations.Valid<int, int>(value);
        var second = Validations.Valid<int, int>(other);
        var function = Validations.Valid<Func<int, int>, int>(static number => number + 1);

        Allocations.AssertNone("Zip", () => _ = validated.Zip(second));
        Allocations.AssertNone("Zip(combine)", () => _ = validated.Zip(second, static (left, right) => left + right));
        Allocations.AssertNone("Apply", () => _ = function.Apply(second));
    }

    [Property(Skip = Allocations.SkipInDebug)]
    public void Comparing_allocates_nothing(Validated<int, int> validated, Validated<int, int> other)
    {
        Allocations.AssertNone("Equals", () => _ = validated.Equals(other));
        Allocations.AssertNone("==", () => _ = validated == other);
        Allocations.AssertNone("!=", () => _ = validated != other);
        Allocations.AssertNone("GetHashCode", () => _ = validated.GetHashCode());
        Allocations.AssertNone("Valid ==", () => _ = new Valid<int>(1) == new Valid<int>(2));
    }

    private static Validated<int, int> Succeed(int value) => valid(value);

    private static Validated<int, int> Fail(Invalid<int> failure) => failure;

    private static Validated<T, TError> SucceedGeneric<T, TError>(T value)
        where T : notnull
        where TError : notnull => valid(value);

    private static int Describe<T, TError>(Validated<T, TError> validated)
        where T : notnull
        where TError : notnull =>
        validated switch
        {
            Valid<T> => 1,
            Invalid<TError> => 2,
        };
}
