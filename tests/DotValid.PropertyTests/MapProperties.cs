using FsCheck;
using FsCheck.Xunit;

namespace DotValid.PropertyTests;

/// <summary>
/// <c>Map</c> is a functor over the value: identity and composition hold, the errors pass through unchanged, and the
/// delegate runs exactly once for a valid validation and never for an invalid one.
/// </summary>
public sealed class MapProperties
{
    [Property]
    public void Mapping_the_identity_changes_nothing(Validated<int, string> validated) =>
        Assert.Equal(validated, validated.Map(value => value));

    [Property]
    public void Mapping_a_composition_is_mapping_twice(Validated<int, string> validated, Func<int, long> first, Func<long, string> second) =>
        Assert.Equal(validated.Map(value => second(first(value))), validated.Map(first).Map(second));

    [Property]
    public void A_valid_validation_holds_the_mapped_value(int value, Func<int, string> map) =>
        ValidatedAssert.IsValid(map(value), Validations.Valid<int, string>(value).Map(map));

    [Property]
    public void The_errors_pass_through_unchanged(NonEmptyArray<int> errors)
    {
        var mapped = Validations.Invalid<string, int>(errors.Get).Map(text => text.Length);

        ValidatedAssert.IsInvalid(errors.Get, mapped);
    }

    [Property]
    public void The_delegate_runs_once_for_a_valid_validation_and_never_for_an_invalid_one(Validated<int, string> validated)
    {
        var calls = 0;

        _ = validated.Map(value => ++calls + value);

        Assert.Equal(validated is Valid<int> ? 1 : 0, calls);
    }

    [Property]
    public void A_null_from_the_delegate_is_rejected_and_the_delegate_named(int value)
    {
        var exception = Assert.Throws<InvalidOperationException>(
            () => Validations.Valid<int, string>(value).Map(_ => (string)null!));

        Assert.StartsWith("'map' returned null", exception.Message, StringComparison.Ordinal);
    }

    [Property]
    public void A_missing_delegate_is_rejected_whatever_the_case(Validated<int, string> validated) =>
        Assert.Equal("map", Assert.Throws<ArgumentNullException>(() => validated.Map((Func<int, string>)null!)).ParamName);
}
