namespace DotValid.PropertyTests;

/// <summary>
/// Generic assertions about the case a validation is in.
/// </summary>
internal static class ValidatedAssert
{
    public static void IsValid<T, TError>(T expected, Validated<T, TError> actual)
        where T : notnull
        where TError : notnull
    {
        if (actual is Valid<T>(var value))
        {
            Assert.Equal(expected, value);
            return;
        }

        Assert.Fail($"Expected Valid ({expected}), but the validation is {Validations.Describe(actual)}.");
    }

    public static void IsInvalid<T, TError>(IEnumerable<TError> expected, Validated<T, TError> actual)
        where T : notnull
        where TError : notnull
    {
        if (actual is Invalid<TError>(var errors))
        {
            Assert.Equal(expected, errors);
            return;
        }

        Assert.Fail($"Expected Invalid ({string.Join(", ", expected)}), but the validation is {Validations.Describe(actual)}.");
    }
}
