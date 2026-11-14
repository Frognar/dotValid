using FsCheck;
using FsCheck.Xunit;

namespace DotValid.PropertyTests;

/// <summary>
/// <c>Ensure</c> keeps a valid validation whose value satisfies the predicate and turns the others into the given error.
/// Like <c>Then</c>, it short-circuits: an invalid validation passes through and nothing runs.
/// </summary>
public sealed class EnsureProperties
{
    [Property]
    public void A_value_that_satisfies_the_predicate_is_kept(int value) =>
        ValidatedAssert.IsValid(value, Validations.Valid<int, string>(value).Ensure(_ => true, "never"));

    [Property]
    public void A_value_that_fails_the_predicate_becomes_the_error(int value, NonNull<string> error)
    {
        ValidatedAssert.IsInvalid([error.Get], Validations.Valid<int, string>(value).Ensure(_ => false, error.Get));
        ValidatedAssert.IsInvalid([$"{value} rejected"], Validations.Valid<int, string>(value).Ensure(_ => false, number => $"{number} rejected"));
    }

    [Property]
    public void Ensure_is_Then_with_a_check(Validated<int, string> validated, Func<int, bool> predicate, NonNull<string> error) =>
        Assert.Equal(
            validated.Then(value => predicate(value) ? Validations.Valid<int, string>(value) : Validations.Invalid<int, string>(error.Get)),
            validated.Ensure(predicate, error.Get));

    [Property]
    public void Ensure_with_a_factory_is_Ensure_with_the_produced_error(Validated<int, string> validated, Func<int, bool> predicate, Func<int, string> error) =>
        Assert.Equal(
            validated.Then(value => predicate(value) ? Validations.Valid<int, string>(value) : Validations.Invalid<int, string>(error(value))),
            validated.Ensure(predicate, error));

    [Property]
    public void An_invalid_validation_passes_through_and_nothing_runs(NonEmptyArray<int> errors)
    {
        var calls = 0;
        var invalidated = Validations.Invalid<string, int>(errors.Get);

        ValidatedAssert.IsInvalid(errors.Get, invalidated.Ensure(_ => ++calls > 0, -1));
        ValidatedAssert.IsInvalid(errors.Get, invalidated.Ensure(_ => ++calls > 0, _ => ++calls));
        Assert.Equal(0, calls);
    }

    [Property]
    public void The_predicate_runs_once_and_the_factory_only_when_it_fails(int value, bool holds)
    {
        var predicateCalls = 0;
        var factoryCalls = 0;

        _ = Validations.Valid<int, string>(value).Ensure(_ => ++predicateCalls > 0 && holds, _ => $"{++factoryCalls}");

        Assert.Equal(1, predicateCalls);
        Assert.Equal(holds ? 0 : 1, factoryCalls);
    }

    [Property]
    public void A_null_from_the_factory_is_rejected(int value) =>
        Assert.Throws<InvalidOperationException>(
            () => Validations.Valid<int, string>(value).Ensure(_ => false, _ => (string)null!));

    [Property]
    public void Missing_arguments_are_rejected_whatever_the_case(Validated<int, string> validated)
    {
        Assert.Equal("predicate", Assert.Throws<ArgumentNullException>(() => validated.Ensure(null!, "error")).ParamName);
        Assert.Equal("error", Assert.Throws<ArgumentNullException>(() => validated.Ensure(_ => true, (string)null!)).ParamName);
        Assert.Equal(
            "predicate",
            Assert.Throws<ArgumentNullException>(() => validated.Ensure(null!, (Func<int, string>)(_ => "error"))).ParamName);
        Assert.Equal(
            "error",
            Assert.Throws<ArgumentNullException>(() => validated.Ensure(_ => true, (Func<int, string>)null!)).ParamName);
    }
}
