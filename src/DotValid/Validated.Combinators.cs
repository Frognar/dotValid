namespace DotValid;

/// <content>
/// Accumulating combinators: <c>Zip</c>, and the core that <c>Apply</c> and <c>Combine</c> build on.
/// </content>
public readonly partial struct Validated<T, TError>
{
    /// <summary>
    /// Pairs the value of this validation with the value of <paramref name="other"/>. Unlike
    /// <see cref="Then{TResult}"/>, both sides are always read and their errors accumulate: this validation's errors
    /// first, then <paramref name="other"/>'s.
    /// </summary>
    /// <typeparam name="TOther">The type of the other value.</typeparam>
    /// <param name="other">The other validation.</param>
    /// <returns>A valid validation with both values as a pair, or every error of both sides.</returns>
    /// <exception cref="ArgumentException"><paramref name="other"/> is uninitialized (default).</exception>
    /// <exception cref="InvalidOperationException">This validation is uninitialized (default).</exception>
    public Validated<(T, TOther), TError> Zip<TOther>(Validated<TOther, TError> other)
        where TOther : notnull => ZipWith(other, static (value, otherValue) => (value, otherValue), nameof(other), "pair");

    /// <summary>
    /// Combines the value of this validation with the value of <paramref name="other"/>. Both sides are always read and
    /// their errors accumulate: this validation's errors first, then <paramref name="other"/>'s.
    /// </summary>
    /// <remarks><paramref name="combine"/> runs only when both are valid, exactly once.</remarks>
    /// <typeparam name="TOther">The type of the other value.</typeparam>
    /// <typeparam name="TResult">The type of the combined value.</typeparam>
    /// <param name="other">The other validation.</param>
    /// <param name="combine">Combines both values.</param>
    /// <returns>A valid validation with the combined value, or every error of both sides.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="combine"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="other"/> is uninitialized (default).</exception>
    /// <exception cref="InvalidOperationException">
    /// This validation is uninitialized (default), or <paramref name="combine"/> returned <see langword="null"/>.
    /// </exception>
    public Validated<TResult, TError> Zip<TOther, TResult>(Validated<TOther, TError> other, Func<T, TOther, TResult> combine)
        where TOther : notnull
        where TResult : notnull
    {
        ArgumentNullException.ThrowIfNull(combine);
        return ZipWith(other, combine, nameof(other), nameof(combine));
    }

    // V7: the accumulating core behind Zip and Apply. Both sides are always read; the errors of both come out, this
    // side's first. Names the parameters the public member exposes.
    internal Validated<TResult, TError> ZipWith<TOther, TResult>(
        Validated<TOther, TError> other,
        Func<T, TOther, TResult> combine,
        string otherName,
        string producer)
        where TOther : notnull
        where TResult : notnull
    {
        Validated<TOther, TError>.RejectUninitialized(other, otherName);
        return (_case, other._case) switch
        {
            (ValidatedCase.Valid, ValidatedCase.Valid) => new Validated<TResult, TError>(
                new Valid<TResult>(Produced(combine(_valid.Value, other._valid.Value), producer))),
            (ValidatedCase.Valid, _) => new Validated<TResult, TError>(other._invalid),
            (ValidatedCase.Invalid, ValidatedCase.Valid) => new Validated<TResult, TError>(_invalid),
            (ValidatedCase.Invalid, _) => new Validated<TResult, TError>(
                Invalid<TError>.FromTrusted(_invalid.Errors.AddRange(other._invalid.Errors))),
            _ => throw Uninitialized(),
        };
    }

    // An argument that is itself a validation must be initialized (V3); checked without boxing, unlike `is null`.
    internal static void RejectUninitialized(Validated<T, TError> validated, string parameterName)
    {
        if (validated._case == ValidatedCase.Uninitialized)
        {
            throw new ArgumentException("The validation is uninitialized (default).", parameterName);
        }
    }
}
