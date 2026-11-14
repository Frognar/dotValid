using FsCheck;
using FsCheck.Xunit;

namespace DotValid.PropertyTests;

/// <summary>
/// <c>OrDefault</c> gives the value of a valid validation and a fallback for an invalid one; the fallback factory sees
/// every error.
/// </summary>
public sealed class OrDefaultProperties
{
    [Property]
    public void A_valid_validation_gives_its_value(int value, int fallback) =>
        Assert.Equal(value, Validations.Valid<int, string>(value).OrDefault(fallback));

    [Property]
    public void An_invalid_validation_gives_the_fallback(NonNull<string> failure, int fallback) =>
        Assert.Equal(fallback, Validations.Invalid<int, string>(failure.Get).OrDefault(fallback));

    [Property]
    public void OrDefault_is_Match_with_a_constant_invalid_branch(Validated<int, string> validated, int fallback) =>
        Assert.Equal(validated.Match(value => value, _ => fallback), validated.OrDefault(fallback));

    [Property]
    public void The_factory_receives_every_error(NonEmptyArray<int> errors) =>
        Assert.Equal(errors.Get.Sum(), Validations.Invalid<int, int>(errors.Get).OrDefault(all => all.Sum()));

    [Property]
    public void The_factory_runs_once_for_an_invalid_validation_and_never_for_a_valid_one(Validated<int, string> validated)
    {
        var calls = 0;

        _ = validated.OrDefault(_ => ++calls);

        Assert.Equal(validated is Invalid<string> ? 1 : 0, calls);
    }

    [Property]
    public void OrDefault_with_a_factory_is_Match_with_that_invalid_branch(Validated<int, string> validated) =>
        Assert.Equal(
            validated.Match(value => value, errors => errors.Length),
            validated.OrDefault(errors => errors.Length));

    [Property]
    public void A_missing_fallback_is_rejected_whatever_the_case(Validated<string, int> validated)
    {
        var missingValue = Assert.Throws<ArgumentNullException>(() => validated.OrDefault((string)null!));
        var missingFactory = Assert.Throws<ArgumentNullException>(
            () => validated.OrDefault((Func<System.Collections.Immutable.ImmutableArray<int>, string>)null!));

        Assert.Equal("fallback", missingValue.ParamName);
        Assert.Equal("fallback", missingFactory.ParamName);
    }
}
