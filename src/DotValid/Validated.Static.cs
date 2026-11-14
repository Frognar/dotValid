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
}
