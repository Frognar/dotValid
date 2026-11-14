using FsCheck;
using FsCheck.Xunit;

namespace DotValid.PropertyTests;

/// <summary>
/// <c>Zip</c> combines two independent validations and, unlike <c>Then</c>, accumulates (V7): both sides are always
/// read, and the errors of both come out, this side's first.
/// </summary>
public sealed class ZipProperties
{
    [Property]
    public void Two_valid_validations_are_paired(int value, bool other) =>
        ValidatedAssert.IsValid((value, other), Validations.Valid<int, string>(value).Zip(Validations.Valid<bool, string>(other)));

    [Property]
    public void The_errors_of_both_sides_accumulate_in_order(NonEmptyArray<int> first, NonEmptyArray<int> second) =>
        ValidatedAssert.IsInvalid(
            first.Get.Concat(second.Get),
            Validations.Invalid<string, int>(first.Get).Zip(Validations.Invalid<bool, int>(second.Get)));

    [Property]
    public void One_invalid_side_gives_its_errors(int value, NonEmptyArray<int> errors)
    {
        ValidatedAssert.IsInvalid(errors.Get, Validations.Valid<int, int>(value).Zip(Validations.Invalid<bool, int>(errors.Get)));
        ValidatedAssert.IsInvalid(errors.Get, Validations.Invalid<bool, int>(errors.Get).Zip(Validations.Valid<int, int>(value)));
    }

    [Property]
    public void Zip_gives_every_error_of_both_sides(Validated<int, string> validated, Validated<bool, string> other)
    {
        var expected = Errors(validated).Concat(Errors(other)).ToArray();

        var zipped = validated.Zip(other);

        Assert.Equal(expected, Errors(zipped));
        Assert.Equal(expected.Length == 0, zipped is Valid<(int, bool)>);
    }

    [Property]
    public void Zip_with_a_combiner_is_Zip_then_Map(Validated<int, string> validated, Validated<bool, string> other, Func<int, bool, string> combine) =>
        Assert.Equal(validated.Zip(other).Map(pair => combine(pair.Item1, pair.Item2)), validated.Zip(other, combine));

    [Property]
    public void The_combiner_runs_once_when_both_are_valid_and_never_otherwise(Validated<int, string> validated, Validated<int, string> other)
    {
        var calls = 0;

        var combined = validated.Zip(other, (left, right) => ++calls + left + right);

        Assert.Equal(combined is Valid<int> ? 1 : 0, calls);
    }

    [Property]
    public void A_null_from_the_combiner_is_rejected(int left, int right) =>
        Assert.Throws<InvalidOperationException>(
            () => Validations.Valid<int, string>(left).Zip(Validations.Valid<int, string>(right), (_, _) => (string)null!));

    [Property]
    public void Missing_or_uninitialized_arguments_are_rejected_whatever_the_case(Validated<int, string> validated)
    {
        var uninitialized = Assert.Throws<ArgumentException>(() => validated.Zip(default(Validated<int, string>)));
        Assert.Equal("other", uninitialized.ParamName);
        Assert.StartsWith("The validation is uninitialized", uninitialized.Message, StringComparison.Ordinal);
        Assert.Equal(
            "other",
            Assert.Throws<ArgumentException>(() => validated.Zip(default(Validated<int, string>), (a, b) => a + b)).ParamName);
        Assert.Equal(
            "combine",
            Assert.Throws<ArgumentNullException>(
                () => validated.Zip(Validations.Valid<int, string>(1), (Func<int, int, int>)null!)).ParamName);
    }

    internal static TError[] Errors<T, TError>(Validated<T, TError> validated)
        where T : notnull
        where TError : notnull => validated is Invalid<TError>(var errors) ? [.. errors] : [];
}
