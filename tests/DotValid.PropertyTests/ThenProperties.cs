using FsCheck;
using FsCheck.Xunit;
using static DotValid.Prelude;

namespace DotValid.PropertyTests;

/// <summary>
/// <c>Then</c> chains a step that depends on the value (V8). It short-circuits like a monadic bind: an invalid validation
/// passes its errors through and the step never runs, so nothing is accumulated. That is why it is not called
/// <c>Bind</c>, and why only <c>Zip</c>/<c>Combine</c> collect errors.
/// </summary>
public sealed class ThenProperties
{
    [Property]
    public void Left_identity_continuing_a_valid_validation_is_applying_the_step(int value, Func<int, Validated<string, string>> next) =>
        Assert.Equal(next(value), Validations.Valid<int, string>(value).Then(next));

    [Property]
    public void Right_identity_continuing_with_valid_changes_nothing(Validated<int, string> validated) =>
        Assert.Equal(validated, validated.Then(value => Validations.Valid<int, string>(value)));

    [Property]
    public void Associativity_the_grouping_of_steps_does_not_matter(
        Validated<int, string> validated,
        Func<int, Validated<bool, string>> first,
        Func<bool, Validated<string, string>> second) =>
        Assert.Equal(validated.Then(first).Then(second), validated.Then(value => first(value).Then(second)));

    [Property]
    public void An_invalid_validation_passes_its_errors_through_and_the_step_never_runs(NonEmptyArray<int> errors)
    {
        var calls = 0;

        var continued = Validations.Invalid<string, int>(errors.Get).Then(text => Validations.Invalid<bool, int>(++calls));

        ValidatedAssert.IsInvalid(errors.Get, continued);
        Assert.Equal(0, calls);
    }

    [Property]
    public void The_step_runs_once_for_a_valid_validation(int value)
    {
        var calls = 0;

        _ = Validations.Valid<int, string>(value).Then(number => Validations.Valid<int, string>(++calls + number));

        Assert.Equal(1, calls);
    }

    [Property]
    public void An_uninitialized_validation_from_the_step_is_rejected(int value)
    {
        var exception = Assert.Throws<InvalidOperationException>(
            () => Validations.Valid<int, string>(value).Then(_ => default(Validated<int, string>)));

        Assert.StartsWith("'next' returned an uninitialized", exception.Message, StringComparison.Ordinal);
    }

    [Property]
    public void Select_is_Map(Validated<int, string> validated, Func<int, string> selector) =>
        Assert.Equal(validated.Map(selector), validated.Select(selector));

    [Property]
    public void A_query_with_one_from_is_Map(Validated<int, string> validated, Func<int, string> selector) =>
        Assert.Equal(validated.Map(selector), from value in validated select selector(value));

    [Property]
    public void Missing_steps_are_rejected_whatever_the_case(Validated<int, string> validated)
    {
        Assert.Equal(
            "next",
            Assert.Throws<ArgumentNullException>(() => validated.Then((Func<int, Validated<int, string>>)null!)).ParamName);
        Assert.Equal(
            "selector",
            Assert.Throws<ArgumentNullException>(() => validated.Select((Func<int, int>)null!)).ParamName);
    }

    [Fact]
    public void Reads_like_parsing_then_checking_a_range()
    {
        static Validated<int, string> Parse(string text) =>
            int.TryParse(text, out var number) ? valid(number) : invalid($"'{text}' is not a number");

        static Validated<int, string> Adult(int age) => age >= 18 ? valid(age) : invalid("too young");

        ValidatedAssert.IsValid(42, Parse("42").Then(Adult));
        ValidatedAssert.IsInvalid(["too young"], Parse("7").Then(Adult));
        ValidatedAssert.IsInvalid(["'x' is not a number"], Parse("x").Then(Adult));
    }
}
