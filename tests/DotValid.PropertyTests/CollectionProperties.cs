using System.Collections.Immutable;
using FsCheck;
using FsCheck.Xunit;

namespace DotValid.PropertyTests;

/// <summary>
/// Sequences of validations (V12). Unlike a result, <c>Sequence</c> and <c>Traverse</c> accumulate: they read every
/// element and give every value, or every error of every invalid element in order. <c>Values</c> and <c>Errors</c> are
/// deferred like LINQ; <c>Partition</c> reads the sequence once.
/// </summary>
public sealed class CollectionProperties
{
    [Property]
    public void Sequence_gives_all_values_when_every_validation_is_valid(int[] values) =>
        ValidatedAssert.IsValid(Joined(values), Read(values.Select(Validations.Valid<int, string>).Sequence()));

    [Property]
    public void Sequence_gives_every_error_of_every_invalid_element_in_order(Validated<int, string>[] validations)
    {
        var errors = validations.SelectMany(ZipProperties.Errors).ToArray();
        var expected = errors.Length == 0
            ? Validations.Valid<string, string>(Joined(validations.Select(validated => validated.OrDefault(0))))
            : Validations.Invalid<string, string>(errors);

        Assert.Equal(expected, Read(validations.Sequence()));
    }

    [Property]
    public void Sequence_is_Zip_folded_over_the_validations(Validated<int, string>[] validations)
    {
        var folded = validations.Aggregate(
            Validations.Valid<ImmutableList<int>, string>([]),
            (values, validated) => values.Zip(validated, (list, value) => list.Add(value)));

        Assert.Equal(folded.Map(values => Joined(values)), Read(validations.Sequence()));
    }

    [Fact]
    public void Sequence_reads_every_element_even_after_an_error()
    {
        var validations = new Probe<Validated<int, string>>(
            [Validations.Valid<int, string>(1), Validations.Invalid<int, string>("first"), Validations.Valid<int, string>(2), Validations.Invalid<int, string>("second")]);

        ValidatedAssert.IsInvalid(["first", "second"], validations.Sequence());
        Assert.Equal(4, validations.Pulled);
    }

    [Property]
    public void Traverse_is_Select_then_Sequence(int[] values, Func<int, Validated<int, string>> selector) =>
        Assert.Equal(Read(values.Select(selector).Sequence()), Read(values.Traverse(selector)));

    [Property]
    public void Sequence_is_Traverse_with_the_identity(Validated<int, string>[] validations) =>
        Assert.Equal(Read(validations.Traverse(validated => validated)), Read(validations.Sequence()));

    [Property]
    public void Traverse_calls_the_selector_for_every_element(int[] values)
    {
        var calls = 0;

        _ = values.Traverse(value => ++calls % 2 == 0 ? Validations.Invalid<int, string>("even call") : Validations.Valid<int, string>(value));

        Assert.Equal(values.Length, calls);
    }

    [Fact]
    public void An_empty_sequence_gives_an_empty_list_and_nothing_on_either_side()
    {
        ValidatedAssert.IsValid("", Read(Array.Empty<Validated<int, string>>().Sequence()));
        ValidatedAssert.IsValid("", Read(Array.Empty<int>().Traverse(_ => Validations.Invalid<int, string>("never"))));
        var (values, errors) = Array.Empty<Validated<int, string>>().Partition();
        Assert.Empty(values);
        Assert.Empty(errors);
    }

    [Fact]
    public void The_results_are_snapshots_of_the_source()
    {
        var validations = new List<Validated<int, string>> { Validations.Valid<int, string>(1), Validations.Invalid<int, string>("boom") };
        var numbers = new List<int> { 1 };

        var sequenced = validations.Sequence();
        var traversed = numbers.Traverse(Validations.Valid<int, string>);
        var (values, errors) = validations.Partition();
        validations.Add(Validations.Invalid<int, string>("later"));
        numbers.Add(2);

        ValidatedAssert.IsInvalid(["boom"], sequenced);
        ValidatedAssert.IsValid("1", Read(traversed));
        Assert.Equal(new[] { 1 }, values);
        Assert.Equal(new[] { "boom" }, errors);
    }

    [Property]
    public void Values_keeps_the_values_and_Errors_flattens_the_errors_in_order(Validated<int, string>[] validations)
    {
        Assert.Equal(validations.SelectMany(validated => validated.Match(value => new[] { value }, _ => [])), validations.Values());
        Assert.Equal(validations.SelectMany(ZipProperties.Errors), validations.Errors());
    }

    [Property]
    public void Partition_is_Values_and_Errors(Validated<int, string>[] validations)
    {
        var (values, errors) = validations.Partition();

        Assert.Equal(validations.Values(), values);
        Assert.Equal(validations.Errors(), errors);
    }

    [Property]
    public void Sequence_is_valid_exactly_when_there_are_no_errors(Validated<int, string>[] validations)
    {
        var expected = validations.Errors().Any()
            ? Validations.Invalid<string, string>([.. validations.Errors()])
            : Validations.Valid<string, string>(Joined(validations.Values()));

        Assert.Equal(expected, Read(validations.Sequence()));
    }

    [Fact]
    public void Values_and_Errors_are_deferred_and_Partition_reads_once()
    {
        var validations = new Probe<Validated<int, string>>(
            [Validations.Valid<int, string>(1), Validations.Invalid<int, string>("a", "b"), Validations.Valid<int, string>(2)]);

        var values = validations.Values();
        var errors = validations.Errors();

        Assert.Equal(0, validations.Pulled);
        Assert.Equal(new[] { 1, 2 }, values);
        Assert.Equal(new[] { "a", "b" }, errors);

        var partition = new Probe<Validated<int, string>>([Validations.Invalid<int, string>("x"), Validations.Valid<int, string>(3)]);
        _ = partition.Partition();
        Assert.Equal(2, partition.Pulled);
    }

    [Fact]
    public void Reads_like_validating_every_line_of_a_file()
    {
        static Validated<int, string> Parse(string text) =>
            int.TryParse(text, out var number) ? Validations.Valid<int, string>(number) : Validations.Invalid<int, string>($"'{text}' is not a number");

        ValidatedAssert.IsValid("1,2,3", Read(new[] { "1", "2", "3" }.Traverse(Parse)));
        ValidatedAssert.IsInvalid(["'two' is not a number", "'three' is not a number"], new[] { "1", "two", "three" }.Traverse(Parse));
    }

    [Fact]
    public void Missing_arguments_are_rejected_immediately()
    {
        IEnumerable<Validated<int, string>> validations = null!;
        IEnumerable<int> numbers = null!;

        AssertRejects("source", () => validations.Sequence());
        AssertRejects("source", () => numbers.Traverse(Validations.Valid<int, string>));
        AssertRejects("selector", () => new[] { 1 }.Traverse((Func<int, Validated<int, string>>)null!));
        AssertRejects("source", () => validations.Values());
        AssertRejects("source", () => validations.Errors());
        AssertRejects("source", () => validations.Partition());
    }

    // A list validation compared by content: the values joined into a string, or the same errors.
    private static Validated<string, TError> Read<T, TError>(Validated<IReadOnlyList<T>, TError> validated)
        where T : notnull
        where TError : notnull => validated.Map(values => Joined(values));

    private static string Joined<T>(IEnumerable<T> values) => string.Join(",", values);

    private static void AssertRejects(string parameter, Func<object> call) =>
        Assert.Equal(parameter, Assert.Throws<ArgumentNullException>(call).ParamName);
}
