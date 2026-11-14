using System.Collections.Immutable;

namespace DotValid;

/// <content>
/// Validations and sequences (V12): all-or-every-error (<c>Sequence</c>, <c>Traverse</c>), picking out the values or the
/// errors (<c>Values</c>, <c>Errors</c>), and splitting them (<c>Partition</c>).
/// </content>
/// <remarks>
/// Arguments are validated immediately. <c>Values</c> and <c>Errors</c> are deferred like LINQ; the other methods run
/// at once and read the whole sequence. Reading an uninitialized (default) validation from the sequence throws
/// <see cref="InvalidOperationException"/>.
/// </remarks>
public static partial class Validated
{
    /// <summary>
    /// All the values, when every validation is valid; otherwise every error of every invalid one, in order.
    /// An empty sequence gives an empty list.
    /// </summary>
    /// <remarks>Reads the whole sequence: unlike a result, an error does not stop it (V7, V12).</remarks>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <typeparam name="TError">The type of the errors.</typeparam>
    /// <param name="source">The validations.</param>
    /// <returns>A valid validation with the values in order, or every error in order.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    /// <exception cref="InvalidOperationException">A validation in the sequence is uninitialized (default).</exception>
    public static Validated<IReadOnlyList<T>, TError> Sequence<T, TError>(this IEnumerable<Validated<T, TError>> source)
        where T : notnull
        where TError : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        List<T> values = [];
        var errors = ImmutableArray.CreateBuilder<TError>();
        foreach (var validated in source)
        {
            if (validated.IsValid(out var value, out var some))
            {
                values.Add(value);
            }
            else
            {
                errors.AddRange(some);
            }
        }

        return errors.Count == 0
            ? Validated<IReadOnlyList<T>, TError>.FromValue(values)
            : Validated<IReadOnlyList<T>, TError>.FromErrors(errors.ToImmutable());
    }

    /// <summary>
    /// Validates every element with <paramref name="selector"/>: all the values when every one is valid; otherwise
    /// every error of every invalid one, in order. An empty sequence gives an empty list.
    /// </summary>
    /// <remarks>
    /// <paramref name="selector"/> runs for every element, even after an error.
    /// <c>source.Traverse(selector)</c> is <c>source.Select(selector).Sequence()</c>.
    /// </remarks>
    /// <typeparam name="T">The type of the elements.</typeparam>
    /// <typeparam name="TResult">The type of the values the validations produce.</typeparam>
    /// <typeparam name="TError">The type of the errors.</typeparam>
    /// <param name="source">The elements.</param>
    /// <param name="selector">Validates one element.</param>
    /// <returns>A valid validation with the values in order, or every error in order.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null.</exception>
    /// <exception cref="InvalidOperationException"><paramref name="selector"/> returned an uninitialized validation.</exception>
    public static Validated<IReadOnlyList<TResult>, TError> Traverse<T, TResult, TError>(
        this IEnumerable<T> source,
        Func<T, Validated<TResult, TError>> selector)
        where TResult : notnull
        where TError : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(selector);
        List<TResult> values = [];
        var errors = ImmutableArray.CreateBuilder<TError>();
        foreach (var element in source)
        {
            var validated = Validated<TResult, TError>.Initialized(selector(element), nameof(selector));
            if (validated.IsValid(out var value, out var some))
            {
                values.Add(value);
            }
            else
            {
                errors.AddRange(some);
            }
        }

        return errors.Count == 0
            ? Validated<IReadOnlyList<TResult>, TError>.FromValue(values)
            : Validated<IReadOnlyList<TResult>, TError>.FromErrors(errors.ToImmutable());
    }

    /// <summary>
    /// The values of the valid validations, in order; invalid ones are skipped.
    /// </summary>
    /// <remarks>Deferred: nothing is read until the values are enumerated.</remarks>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <typeparam name="TError">The type of the errors.</typeparam>
    /// <param name="source">The validations.</param>
    /// <returns>The values.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    public static IEnumerable<T> Values<T, TError>(this IEnumerable<Validated<T, TError>> source)
        where T : notnull
        where TError : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        return ValuesOf(source);

        static IEnumerable<T> ValuesOf(IEnumerable<Validated<T, TError>> source)
        {
            foreach (var validated in source)
            {
                if (validated.IsValid(out var value, out _))
                {
                    yield return value;
                }
            }
        }
    }

    /// <summary>
    /// Every error of the invalid validations, flattened, in order; valid ones are skipped.
    /// </summary>
    /// <remarks>Deferred: nothing is read until the errors are enumerated.</remarks>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <typeparam name="TError">The type of the errors.</typeparam>
    /// <param name="source">The validations.</param>
    /// <returns>The errors.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    public static IEnumerable<TError> Errors<T, TError>(this IEnumerable<Validated<T, TError>> source)
        where T : notnull
        where TError : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        return ErrorsOf(source);

        static IEnumerable<TError> ErrorsOf(IEnumerable<Validated<T, TError>> source)
        {
            foreach (var validated in source)
            {
                if (!validated.IsValid(out _, out var errors))
                {
                    foreach (var error in errors)
                    {
                        yield return error;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Splits the validations into the values of the valid ones and every error of the invalid ones, each in order.
    /// </summary>
    /// <remarks>Reads the sequence once.</remarks>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <typeparam name="TError">The type of the errors.</typeparam>
    /// <param name="source">The validations.</param>
    /// <returns>The values and the errors.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
    /// <exception cref="InvalidOperationException">A validation in the sequence is uninitialized (default).</exception>
    public static (IReadOnlyList<T> Values, IReadOnlyList<TError> Errors) Partition<T, TError>(
        this IEnumerable<Validated<T, TError>> source)
        where T : notnull
        where TError : notnull
    {
        ArgumentNullException.ThrowIfNull(source);
        List<T> values = [];
        List<TError> errors = [];
        foreach (var validated in source)
        {
            if (validated.IsValid(out var value, out var some))
            {
                values.Add(value);
            }
            else
            {
                errors.AddRange(some);
            }
        }

        return (values, errors);
    }
}
