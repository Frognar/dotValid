namespace DotValid;

/// <summary>
/// Functions on validations that cannot be members of <see cref="Validated{T, TError}"/>, because they need a more
/// specific shape of validation (for example a validation of a function) or several validations at once.
/// </summary>
public static partial class Validated
{
    /// <summary>
    /// Applies the function of <paramref name="function"/> to the value of <paramref name="argument"/>. Both sides are
    /// always read and their errors accumulate: the function's errors first, then the argument's.
    /// </summary>
    /// <remarks>
    /// The applicative core: with curried functions, <c>f.Apply(a).Apply(b)</c> validates every argument and collects
    /// every error. <see cref="Validated{T, TError}.Zip{TOther, TResult}"/> and <c>Combine</c> are usually easier to read.
    /// </remarks>
    /// <typeparam name="T">The type of the argument.</typeparam>
    /// <typeparam name="TResult">The type of the result of the function.</typeparam>
    /// <typeparam name="TError">The type of the errors.</typeparam>
    /// <param name="function">The validated function.</param>
    /// <param name="argument">The validated argument.</param>
    /// <returns>A valid validation with the result of the function, or every error of both sides.</returns>
    /// <exception cref="ArgumentException"><paramref name="argument"/> is uninitialized (default).</exception>
    /// <exception cref="InvalidOperationException">
    /// <paramref name="function"/> is uninitialized (default), or the function returned <see langword="null"/>.
    /// </exception>
    public static Validated<TResult, TError> Apply<T, TResult, TError>(
        this Validated<Func<T, TResult>, TError> function,
        Validated<T, TError> argument)
        where T : notnull
        where TResult : notnull
        where TError : notnull =>
        function.ZipWith(argument, static (apply, value) => apply(value), nameof(argument), nameof(function));

    /// <summary>
    /// A validation of <paramref name="value"/>: valid when it satisfies <paramref name="predicate"/>; otherwise invalid
    /// with <paramref name="error"/>.
    /// </summary>
    /// <remarks>The building block for rules: <c>Validated.Check(age, a => a >= 0, "age is negative")</c>.</remarks>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="predicate">The condition the value must satisfy.</param>
    /// <param name="error">The error when it does not.</param>
    /// <returns>A valid validation of <paramref name="value"/>, or an invalid one with <paramref name="error"/>.</returns>
    /// <exception cref="ArgumentNullException">An argument is null.</exception>
    public static Validated<T, TError> Check<T, TError>(T value, Func<T, bool> predicate, TError error)
        where T : notnull
        where TError : notnull
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(error);
        return predicate(value) ? Validated<T, TError>.FromValue(value) : Validated<T, TError>.FromError(error);
    }

    /// <summary>
    /// A validation of <paramref name="value"/>: valid when it satisfies <paramref name="predicate"/>; otherwise invalid
    /// with the error <paramref name="error"/> produces from the value.
    /// </summary>
    /// <remarks><paramref name="error"/> runs only when <paramref name="predicate"/> fails.</remarks>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="predicate">The condition the value must satisfy.</param>
    /// <param name="error">Produces the error from the value that does not satisfy <paramref name="predicate"/>.</param>
    /// <returns>A valid validation of <paramref name="value"/>, or an invalid one with the produced error.</returns>
    /// <exception cref="ArgumentNullException">An argument is null.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="error"/> returned <see langword="null"/>.</exception>
    public static Validated<T, TError> Check<T, TError>(T value, Func<T, bool> predicate, Func<T, TError> error)
        where T : notnull
        where TError : notnull
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(error);
        return predicate(value)
            ? Validated<T, TError>.FromValue(value)
            : Validated<T, TError>.FromError(Validated<T, TError>.Produced(error(value), nameof(error)));
    }

    /// <summary>
    /// A validation of a nullable value type: valid with the value, or invalid with <paramref name="error"/> for
    /// <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="value">The nullable value.</param>
    /// <param name="error">The error when <paramref name="value"/> is <see langword="null"/>.</param>
    /// <returns>A valid validation with the value, or an invalid one with <paramref name="error"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
    public static Validated<T, TError> FromNullable<T, TError>(T? value, TError error)
        where T : struct
        where TError : notnull
    {
        ArgumentNullException.ThrowIfNull(error);
        return value.HasValue ? Validated<T, TError>.FromValue(value.Value) : Validated<T, TError>.FromError(error);
    }

    /// <summary>
    /// A validation of a nullable value type: valid with the value, or invalid with the error <paramref name="error"/>
    /// produces for <see langword="null"/>.
    /// </summary>
    /// <remarks><paramref name="error"/> runs only for <see langword="null"/>, exactly once.</remarks>
    /// <typeparam name="T">The value type.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="value">The nullable value.</param>
    /// <param name="error">Produces the error when <paramref name="value"/> is <see langword="null"/>.</param>
    /// <returns>A valid validation with the value, or an invalid one with the produced error.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="error"/> returned <see langword="null"/>.</exception>
    public static Validated<T, TError> FromNullable<T, TError>(T? value, Func<TError> error)
        where T : struct
        where TError : notnull
    {
        ArgumentNullException.ThrowIfNull(error);
        return value.HasValue
            ? Validated<T, TError>.FromValue(value.Value)
            : Validated<T, TError>.FromError(Validated<T, TError>.Produced(error(), nameof(error)));
    }

    /// <summary>
    /// A validation of a nullable reference: valid with the reference, or invalid with <paramref name="error"/> for
    /// <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">The reference type.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="value">The nullable reference.</param>
    /// <param name="error">The error when <paramref name="value"/> is <see langword="null"/>.</param>
    /// <returns>A valid validation with the reference, or an invalid one with <paramref name="error"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
    public static Validated<T, TError> FromNullable<T, TError>(T? value, TError error)
        where T : class
        where TError : notnull
    {
        ArgumentNullException.ThrowIfNull(error);
        return value is not null ? Validated<T, TError>.FromValue(value) : Validated<T, TError>.FromError(error);
    }

    /// <summary>
    /// A validation of a nullable reference: valid with the reference, or invalid with the error
    /// <paramref name="error"/> produces for <see langword="null"/>.
    /// </summary>
    /// <remarks><paramref name="error"/> runs only for <see langword="null"/>, exactly once.</remarks>
    /// <typeparam name="T">The reference type.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    /// <param name="value">The nullable reference.</param>
    /// <param name="error">Produces the error when <paramref name="value"/> is <see langword="null"/>.</param>
    /// <returns>A valid validation with the reference, or an invalid one with the produced error.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="error"/> returned <see langword="null"/>.</exception>
    public static Validated<T, TError> FromNullable<T, TError>(T? value, Func<TError> error)
        where T : class
        where TError : notnull
    {
        ArgumentNullException.ThrowIfNull(error);
        return value is not null
            ? Validated<T, TError>.FromValue(value)
            : Validated<T, TError>.FromError(Validated<T, TError>.Produced(error(), nameof(error)));
    }
}
