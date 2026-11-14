using System.Collections.Immutable;
using FsCheck;
using FsCheck.Xunit;

namespace DotValid.PropertyTests;

/// <summary>
/// <c>Match</c> is the total way out of a validation: one branch per case, exactly one of them runs, exactly once, and
/// the invalid branch sees every error in order.
/// </summary>
public sealed class MatchProperties
{
    [Property]
    public void A_valid_validation_goes_to_the_valid_branch(int value, Func<int, string> valid) =>
        Assert.Equal(valid(value), Validations.Valid<int, string>(value).Match(valid, Joined));

    [Property]
    public void An_invalid_validation_goes_to_the_invalid_branch_with_every_error(NonEmptyArray<NonNull<string>> errors)
    {
        var expected = errors.Get.Select(error => error.Get).ToArray();

        Assert.Equal(
            string.Join("|", expected),
            Validations.Invalid<int, string>(expected).Match(value => $"{value}", Joined));
    }

    [Property]
    public void Match_agrees_with_a_switch(Validated<int, string> validated)
    {
        var bySwitch = validated switch
        {
            Valid<int>(var value) => $"valid {value}",
            Invalid<string>(var errors) => $"invalid {Joined(errors)}",
        };

        Assert.Equal(bySwitch, validated.Match(value => $"valid {value}", errors => $"invalid {Joined(errors)}"));
    }

    [Property]
    public void Exactly_one_branch_runs_exactly_once(Validated<int, string> validated)
    {
        var validCalls = 0;
        var invalidCalls = 0;

        _ = validated.Match(_ => ++validCalls, _ => ++invalidCalls);

        Assert.Equal(1, validCalls + invalidCalls);
        Assert.Equal(validated is Valid<int> ? 1 : 0, validCalls);
    }

    [Property]
    public void Rebuilding_from_the_branches_gives_back_the_same_validation(Validated<int, string> validated) =>
        Assert.Equal(
            validated,
            validated.Match(value => Validations.Valid<int, string>(value), errors => Validations.Invalid<int, string>([.. errors])));

    [Property]
    public void Match_reads_both_cases_when_value_and_error_have_the_same_type(Validated<string, string> validated) =>
        Assert.Equal(
            Validations.Describe(validated),
            validated.Match(value => $"Valid ({value})", errors => $"Invalid ({string.Join(", ", errors)})"));

    [Property]
    public void A_missing_branch_is_rejected_whatever_the_case(Validated<int, string> validated)
    {
        var missingValid = Assert.Throws<ArgumentNullException>(() => validated.Match<int>(null!, _ => 0));
        var missingInvalid = Assert.Throws<ArgumentNullException>(() => validated.Match(_ => 0, null!));

        Assert.Equal("valid", missingValid.ParamName);
        Assert.Equal("invalid", missingInvalid.ParamName);
    }

    private static string Joined(ImmutableArray<string> errors) => string.Join("|", errors);
}
