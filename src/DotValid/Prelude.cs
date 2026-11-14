namespace DotValid;

/// <summary>
/// Lower-case functions that create the two cases: <c>using static DotValid.Prelude;</c>, then <c>valid(value)</c> and
/// <c>invalid(error)</c>. Both convert implicitly to any matching <see cref="Validated{T, TError}"/>.
/// </summary>
/// <remarks>
/// Lower case on purpose: they never clash with application types named <c>Valid</c> or <c>Invalid</c>.
/// </remarks>
public static class Prelude
{
    /// <summary>
    /// Creates the valid case.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value.</param>
    /// <returns>The valid case holding <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
#pragma warning disable IDE1006 // Naming rule violation: lower-case names are intentional (keyword-like functions).
    public static Valid<T> valid<T>(T value)
        where T : notnull => new(value);

    /// <summary>
    /// Creates the invalid case with a single error.
    /// </summary>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>The invalid case holding <paramref name="error"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
    public static Invalid<TError> invalid<TError>(TError error)
        where TError : notnull => new(error);

    /// <summary>
    /// Creates the invalid case with several errors, in order: <c>invalid(first, second)</c> or
    /// <c>invalid([.. errors])</c>.
    /// </summary>
    /// <remarks>
    /// Pass a collection with a spread, <c>invalid([.. errors])</c>: <c>invalid(errors)</c> would make the collection
    /// itself a single error, which the compiler then rejects where a validation of its elements is expected.
    /// </remarks>
    /// <typeparam name="TError">The type of the errors.</typeparam>
    /// <param name="errors">The errors; at least one, none of them null.</param>
    /// <returns>The invalid case holding <paramref name="errors"/>.</returns>
    /// <exception cref="ArgumentException"><paramref name="errors"/> is empty or holds a null error.</exception>
    public static Invalid<TError> invalid<TError>(params ReadOnlySpan<TError> errors)
        where TError : notnull => new(errors);
#pragma warning restore IDE1006
}
