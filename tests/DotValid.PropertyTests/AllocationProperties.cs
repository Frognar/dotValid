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
/// union's <c>Value</c>, which boxes the case). Reading an existing invalid case allocates nothing. Sequences
/// (<c>Sequence</c>, <c>Traverse</c>, <c>Values</c>, <c>Errors</c>, <c>Partition</c>) allocate like LINQ (enumerators
/// and the lists they return; see <c>CollectionBenchmarks</c>).
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
        Allocations.AssertNone("Combine(2)", () => _ = Validated.Combine(validated, second, static (a, b) => a + b));
        Allocations.AssertNone("Combine(3)", () => _ = Validated.Combine(validated, second, validated, static (a, b, c) => a + b + c));
        Allocations.AssertNone(
            "Combine(8)",
            () => _ = Validated.Combine(
                validated, second, validated, second, validated, second, validated, second,
                static (a, b, c, d, e, f, g, h) => a + b + c + d + e + f + g + h));
    }

    [Property(Skip = Allocations.SkipInDebug)]
    public void Building_blocks_on_the_valid_path_allocate_nothing(int value)
    {
        int? present = value;

        Allocations.AssertNone("Check (passes)", () => _ = Validated.Check(value, static _ => true, -1));
        Allocations.AssertNone("Check(factory) (passes)", () => _ = Validated.Check(value, static _ => true, static number => -number));
        Allocations.AssertNone("FromNullable (present)", () => _ = Validated.FromNullable(present, -1));
        Allocations.AssertNone("FromNullable(factory) (present)", () => _ = Validated.FromNullable(present, static () => -1));
        Allocations.AssertNone("FromNullable (reference)", () => _ = Validated.FromNullable(Text, -1));
        Allocations.AssertNone("ValidationError", () => _ = new ValidationError("Name", "is empty"));
    }

    [Property(Skip = Allocations.SkipInDebug)]
    public void Async_members_with_completed_tasks_allocate_nothing(Validated<int, int> validated)
    {
        Allocations.AssertNone(
            "MatchAsync(async, async)",
            () => _ = validated.MatchAsync(static _ => CompletedOne, static _ => CompletedOne).IsCompleted);
        Allocations.AssertNone(
            "MatchAsync(async, sync)",
            () => _ = validated.MatchAsync(static _ => CompletedOne, static errors => errors.Length).IsCompleted);
        Allocations.AssertNone("MapAsync", () => _ = validated.MapAsync(static _ => CompletedOne).IsCompleted);
        Allocations.AssertNone("ThenAsync", () => _ = validated.ThenAsync(static _ => CompletedValidOne).IsCompleted);
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

    private const string? Text = "text";

    // Tasks completed in advance: any allocation in the async tests comes from the library, not from the delegate.
    private static readonly Task<int> CompletedOne = Task.FromResult(1);
    private static readonly Task<Validated<int, int>> CompletedValidOne = Task.FromResult(Validations.Valid<int, int>(1));

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
