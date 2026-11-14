using static DotValid.Prelude;

namespace DotValid.PropertyTests;

/// <summary>
/// Builds and reads validations in generic code.
/// </summary>
/// <remarks>
/// Building goes through the union conversions from <see cref="Valid{T}"/> and <see cref="Invalid{TError}"/>, which are
/// never ambiguous, even when <c>T</c> and <c>TError</c> are the same type. Reading uses pattern matching.
/// </remarks>
internal static class Validations
{
    public static Validated<T, TError> Valid<T, TError>(T value)
        where T : notnull
        where TError : notnull => valid(value);

    public static Validated<T, TError> Invalid<T, TError>(params ReadOnlySpan<TError> errors)
        where T : notnull
        where TError : notnull => invalid(errors);

    /// <summary>
    /// Describes a validation independently of <c>ToString</c>, for assertion messages.
    /// </summary>
    public static string Describe<T, TError>(Validated<T, TError> validated)
        where T : notnull
        where TError : notnull
    {
        if (validated is Valid<T>(var value))
        {
            return $"Valid ({value})";
        }

        if (validated is Invalid<TError>(var errors))
        {
            return $"Invalid ({string.Join(", ", errors)})";
        }

        return "uninitialized";
    }
}
