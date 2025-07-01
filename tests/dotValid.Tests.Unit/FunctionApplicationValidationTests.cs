using DotValid;

namespace dotValid.Tests.Unit;

public sealed class FunctionApplicationValidationTests
{
    private const string ValueErrorKey = "value";
    private const string NotIntErrorMessage = "not int";
    private const string FunctionErrorKey = "function";
    private const string FunctionFailedMessage = "function failed";

    private static readonly Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>
        _sum =
            (i1, i2, i3, i4, i5, i6, i7, i8, i9, i10, i11, i12, i13, i14, i15, i16) =>
                i1 + i2 + i3 + i4 + i5 + i6 + i7 + i8 + i9 + i10 + i11 + i12 + i13 + i14 + i15 + i16;

    [Fact]
    public void AllValidInputs_ShouldSucceed()
    {
        Validated<int> result = _sum
            .Apply(ValidateInt("1"))
            .Apply(ValidateInt("2"))
            .Apply(ValidateInt("3"))
            .Apply(ValidateInt("4"))
            .Apply(ValidateInt("5"))
            .Apply(ValidateInt("6"))
            .Apply(ValidateInt("7"))
            .Apply(ValidateInt("8"))
            .Apply(ValidateInt("9"))
            .Apply(ValidateInt("10"))
            .Apply(ValidateInt("11"))
            .Apply(ValidateInt("12"))
            .Apply(ValidateInt("13"))
            .Apply(ValidateInt("14"))
            .Apply(ValidateInt("15"))
            .Apply(ValidateInt("16"));

        Assert.Equal(Validation.Succeed(_sum(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16)), result);
    }

    [Fact]
    public void OneInvalidInputs_ShouldReturnSingleError()
    {
        Validated<int> result = _sum
            .Apply(ValidateInt("one"))
            .Apply(ValidateInt("2"))
            .Apply(ValidateInt("3"))
            .Apply(ValidateInt("4"))
            .Apply(ValidateInt("5"))
            .Apply(ValidateInt("6"))
            .Apply(ValidateInt("7"))
            .Apply(ValidateInt("8"))
            .Apply(ValidateInt("9"))
            .Apply(ValidateInt("10"))
            .Apply(ValidateInt("11"))
            .Apply(ValidateInt("12"))
            .Apply(ValidateInt("13"))
            .Apply(ValidateInt("14"))
            .Apply(ValidateInt("15"))
            .Apply(ValidateInt("16"));

        List<ValidationError> errors = GetErrors(result);
        ValidationError error = Assert.Single(errors);
        Assert.Equal(ValueErrorKey, error.PropertyName);
        Assert.Equal(NotIntErrorMessage, error.Error);
    }

    [Fact]
    public void AllInvalidInputs_ShouldReturnAllErrors()
    {
        Validated<int> result = _sum
            .Apply(ValidateInt("one"))
            .Apply(ValidateInt("two"))
            .Apply(ValidateInt("three"))
            .Apply(ValidateInt("four"))
            .Apply(ValidateInt("five"))
            .Apply(ValidateInt("six"))
            .Apply(ValidateInt("seven"))
            .Apply(ValidateInt("eight"))
            .Apply(ValidateInt("nine"))
            .Apply(ValidateInt("ten"))
            .Apply(ValidateInt("eleven"))
            .Apply(ValidateInt("twelve"))
            .Apply(ValidateInt("thirteen"))
            .Apply(ValidateInt("fourteen"))
            .Apply(ValidateInt("fifteen"))
            .Apply(ValidateInt("sixteen"));

        List<ValidationError> errors = GetErrors(result);
        Assert.Equal(16, errors.Count);
    }

    [Fact]
    public void FunctionFailedWithValidInput_ShouldReturnOnlyFunctionError()
    {
        Validated<Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>> result =
            CreateFailedValidationFunction()
            .Apply(ValidateInt("1"));

        List<ValidationError> errors = GetErrors(result);
        Assert.Equal(new ValidationError(FunctionErrorKey, FunctionFailedMessage), Assert.Single(errors));
    }

    [Fact]
    public void FunctionFailedWithInvalidInput_ShouldReturnBothErrors ()
    {
        Validated<Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>> result =
            CreateFailedValidationFunction()
            .Apply(ValidateInt("one"));

        List<ValidationError> errors = result.Match(errors => errors, _ => []).ToList();
        Assert.Collection(errors,
            fst => Assert.Equal(new ValidationError(FunctionErrorKey, FunctionFailedMessage), fst),
            snd => Assert.Equal(new ValidationError(ValueErrorKey, NotIntErrorMessage), snd));
    }

    [Theory]
    [InlineData(15, "2")]
    [InlineData(14, "3")]
    [InlineData(13, "4")]
    [InlineData(12, "5")]
    [InlineData(11, "6")]
    [InlineData(10, "7")]
    [InlineData(9, "8")]
    [InlineData(8, "9")]
    [InlineData(7, "10")]
    [InlineData(6, "11")]
    [InlineData(5, "12")]
    [InlineData(4, "13")]
    [InlineData(3, "14")]
    [InlineData(2, "15")]
    [InlineData(1, "16")]
    public void PartiallyAppliedFunctions_ShouldSucceedWithValidInput(int remainingParams, string validInput)
    {
        Validated<bool> result = ApplyValidationForRemainingParams(remainingParams, validInput);
        Assert.True(result.IsValid);
    }

    private static Validated<int> ValidateInt(string value) =>
        int.TryParse(value, out int result)
            ? Validation.Succeed(result)
            : Validation.Failed<int>([new ValidationError(ValueErrorKey, NotIntErrorMessage)]);

    private static List<ValidationError> GetErrors<T>(Validated<T> validated) =>
        validated.Match(errors => errors, _ => []).ToList();

    private static Validated<Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>>
        CreateFailedValidationFunction() =>
        Validation.Failed<Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>>(
            [new ValidationError(FunctionErrorKey, FunctionFailedMessage)]);

    private static Validated<bool> ApplyValidationForRemainingParams(int remainingParams, string validInput)
    {
        Validated<int> validated = ValidateInt(validInput);

        return remainingParams switch
        {
            15 => Apply15(validated),
            14 => Apply14(validated),
            13 => Apply13(validated),
            12 => Apply12(validated),
            11 => Apply11(validated),
            10 => Apply10(validated),
            9 => Apply9(validated),
            8 => Apply8(validated),
            7 => Apply7(validated),
            6 => Apply6(validated),
            5 => Apply5(validated),
            4 => Apply4(validated),
            3 => Apply3(validated),
            2 => Apply2(validated),
            1 => Apply1(validated),
            _ => throw new ArgumentOutOfRangeException(nameof(remainingParams))
        };
    }

    private static Validated<bool> Apply15(Validated<int> validated)
    {
        Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int> func =
            (i2, i3, i4, i5, i6, i7, i8, i9, i10, i11, i12, i13, i14, i15, i16) =>
                _sum(1, i2, i3, i4, i5, i6, i7, i8, i9, i10, i11, i12, i13, i14, i15, i16);

        return func.Apply(validated).Match(Validation.Failed<bool>, _ => Validation.Succeed(true));
    }

    private static Validated<bool> Apply14(Validated<int> validated)
    {
        Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int> func =
            (i3, i4, i5, i6, i7, i8, i9, i10, i11, i12, i13, i14, i15, i16) =>
                _sum(1, 2, i3, i4, i5, i6, i7, i8, i9, i10, i11, i12, i13, i14, i15, i16);

        return func.Apply(validated).Match(Validation.Failed<bool>, _ => Validation.Succeed(true));
    }

    private static Validated<bool> Apply13(Validated<int> validated)
    {
        Func<int, int, int, int, int, int, int, int, int, int, int, int, int, int> func =
            (i4, i5, i6, i7, i8, i9, i10, i11, i12, i13, i14, i15, i16) =>
                _sum(1, 2, 3, i4, i5, i6, i7, i8, i9, i10, i11, i12, i13, i14, i15, i16);

        return func.Apply(validated).Match(Validation.Failed<bool>, _ => Validation.Succeed(true));
    }

    private static Validated<bool> Apply12(Validated<int> validated)
    {
        Func<int, int, int, int, int, int, int, int, int, int, int, int, int> func =
            (i5, i6, i7, i8, i9, i10, i11, i12, i13, i14, i15, i16) =>
                _sum(1, 2, 3, 4, i5, i6, i7, i8, i9, i10, i11, i12, i13, i14, i15, i16);

        return func.Apply(validated).Match(Validation.Failed<bool>, _ => Validation.Succeed(true));
    }

    private static Validated<bool> Apply11(Validated<int> validated)
    {
        Func<int, int, int, int, int, int, int, int, int, int, int, int> func =
            (i6, i7, i8, i9, i10, i11, i12, i13, i14, i15, i16) =>
                _sum(1, 2, 3, 4, 5, i6, i7, i8, i9, i10, i11, i12, i13, i14, i15, i16);

        return func.Apply(validated).Match(Validation.Failed<bool>, _ => Validation.Succeed(true));
    }

    private static Validated<bool> Apply10(Validated<int> validated)
    {
        Func<int, int, int, int, int, int, int, int, int, int, int> func =
            (i7, i8, i9, i10, i11, i12, i13, i14, i15, i16) =>
                _sum(1, 2, 3, 4, 5, 6, i7, i8, i9, i10, i11, i12, i13, i14, i15, i16);

        return func.Apply(validated).Match(Validation.Failed<bool>, _ => Validation.Succeed(true));
    }

    private static Validated<bool> Apply9(Validated<int> validated)
    {
        Func<int, int, int, int, int, int, int, int, int, int> func =
            (i8, i9, i10, i11, i12, i13, i14, i15, i16) =>
                _sum(1, 2, 3, 4, 5, 6, 7, i8, i9, i10, i11, i12, i13, i14, i15, i16);

        return func.Apply(validated).Match(Validation.Failed<bool>, _ => Validation.Succeed(true));
    }

    private static Validated<bool> Apply8(Validated<int> validated)
    {
        Func<int, int, int, int, int, int, int, int, int> func =
            (i9, i10, i11, i12, i13, i14, i15, i16) =>
                _sum(1, 2, 3, 4, 5, 6, 7, 8, i9, i10, i11, i12, i13, i14, i15, i16);

        return func.Apply(validated).Match(Validation.Failed<bool>, _ => Validation.Succeed(true));
    }

    private static Validated<bool> Apply7(Validated<int> validated)
    {
        Func<int, int, int, int, int, int, int, int> func =
            (i10, i11, i12, i13, i14, i15, i16) =>
                _sum(1, 2, 3, 4, 5, 6, 7, 8, 9, i10, i11, i12, i13, i14, i15, i16);

        return func.Apply(validated).Match(Validation.Failed<bool>, _ => Validation.Succeed(true));
    }

    private static Validated<bool> Apply6(Validated<int> validated)
    {
        Func<int, int, int, int, int, int, int> func =
            (i11, i12, i13, i14, i15, i16) =>
                _sum(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, i11, i12, i13, i14, i15, i16);

        return func.Apply(validated).Match(Validation.Failed<bool>, _ => Validation.Succeed(true));
    }

    private static Validated<bool> Apply5(Validated<int> validated)
    {
        Func<int, int, int, int, int, int> func =
            (i12, i13, i14, i15, i16) =>
                _sum(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, i12, i13, i14, i15, i16);

        return func.Apply(validated).Match(Validation.Failed<bool>, _ => Validation.Succeed(true));
    }

    private static Validated<bool> Apply4(Validated<int> validated)
    {
        Func<int, int, int, int, int> func =
            (i13, i14, i15, i16) =>
                _sum(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, i13, i14, i15, i16);

        return func.Apply(validated).Match(Validation.Failed<bool>, _ => Validation.Succeed(true));
    }

    private static Validated<bool> Apply3(Validated<int> validated)
    {
        Func<int, int, int, int> func =
            (i14, i15, i16) =>
                _sum(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, i14, i15, i16);

        return func.Apply(validated).Match(Validation.Failed<bool>, _ => Validation.Succeed(true));
    }

    private static Validated<bool> Apply2(Validated<int> validated)
    {
        Func<int, int, int> func =
            (i15, i16) =>
                _sum(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, i15, i16);

        return func.Apply(validated).Match(Validation.Failed<bool>, _ => Validation.Succeed(true));
    }

    private static Validated<bool> Apply1(Validated<int> validated)
    {
        Func<int, int> func =
            i16 =>
                _sum(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, i16);

        return func.Apply(validated).Match(Validation.Failed<bool>, _ => Validation.Succeed(true));
    }
}
