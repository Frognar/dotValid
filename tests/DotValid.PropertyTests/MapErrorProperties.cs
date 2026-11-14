using FsCheck;
using FsCheck.Xunit;

namespace DotValid.PropertyTests;

/// <summary>
/// <c>MapError</c> is a functor over every error: identity and composition hold, the order and the count of the errors
/// are kept, the value passes through unchanged, and the delegate runs once per error.
/// </summary>
public sealed class MapErrorProperties
{
    [Property]
    public void Mapping_the_identity_changes_nothing(Validated<int, string> validated) =>
        Assert.Equal(validated, validated.MapError(error => error));

    [Property]
    public void Mapping_a_composition_is_mapping_twice(Validated<int, string> validated, Func<string, int> first, Func<int, bool> second) =>
        Assert.Equal(validated.MapError(error => second(first(error))), validated.MapError(first).MapError(second));

    [Property]
    public void Every_error_is_mapped_in_order(NonEmptyArray<int> errors, Func<int, string> map) =>
        ValidatedAssert.IsInvalid(errors.Get.Select(map), Validations.Invalid<bool, int>(errors.Get).MapError(map));

    [Property]
    public void The_value_passes_through_unchanged(int value) =>
        ValidatedAssert.IsValid(value, Validations.Valid<int, string>(value).MapError(error => error.Length));

    [Property]
    public void The_delegate_runs_once_per_error_and_never_for_a_valid_validation(Validated<int, string> validated)
    {
        var calls = 0;

        _ = validated.MapError(error => ++calls + error.Length);

        Assert.Equal(validated.Match(_ => 0, errors => errors.Length), calls);
    }

    [Property]
    public void A_null_from_the_delegate_is_rejected(NonEmptyArray<int> errors) =>
        Assert.Throws<InvalidOperationException>(() => Validations.Invalid<bool, int>(errors.Get).MapError(_ => (string)null!));

    [Property]
    public void A_missing_delegate_is_rejected_whatever_the_case(Validated<int, string> validated) =>
        Assert.Equal(
            "map",
            Assert.Throws<ArgumentNullException>(() => validated.MapError((Func<string, int>)null!)).ParamName);
}
