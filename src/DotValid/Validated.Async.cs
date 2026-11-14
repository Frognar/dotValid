using System.Collections.Immutable;

namespace DotValid;

/// <content>
/// Asynchronous counterparts of <c>Match</c>, <c>Map</c> and <c>Then</c> (V10, minimal on purpose).
/// </content>
/// <remarks>
/// <para>
/// The delegates return <see cref="Task{TResult}"/>, so any async lambda or Task-returning method fits. The methods
/// return <see cref="ValueTask{TResult}"/>: when there is nothing to wait for, the result is already complete and
/// nothing is allocated.
/// </para>
/// <para>
/// Arguments and the uninitialized default are rejected immediately, when the method is called. Anything a delegate
/// does, including throwing before it returns a task, comes out when the result is awaited; an
/// <see cref="OperationCanceledException"/> for a requested token gives a canceled result.
/// </para>
/// </remarks>
public readonly partial struct Validated<T, TError>
{
    /// <summary>
    /// Asynchronously reduces this validation to a single value: <paramref name="valid"/> runs on the value,
    /// <paramref name="invalid"/> on the errors. Exactly one of them runs, exactly once.
    /// </summary>
    /// <typeparam name="TResult">The type of the value both branches produce.</typeparam>
    /// <param name="valid">Produces the value from the value of a valid validation.</param>
    /// <param name="invalid">Produces the value from the errors, in order.</param>
    /// <returns>The value produced by the branch that ran.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="valid"/> or <paramref name="invalid"/> is null.</exception>
    /// <exception cref="InvalidOperationException">The validation is uninitialized (default).</exception>
    public ValueTask<TResult> MatchAsync<TResult>(Func<T, Task<TResult>> valid, Func<ImmutableArray<TError>, Task<TResult>> invalid)
    {
        ArgumentNullException.ThrowIfNull(valid);
        ArgumentNullException.ThrowIfNull(invalid);
        return _case switch
        {
            ValidatedCase.Valid => Start(valid, _valid.Value),
            ValidatedCase.Invalid => Start(invalid, _invalid.Errors),
            _ => throw Uninitialized(),
        };
    }

    /// <summary>
    /// Asynchronously reduces this validation to a single value, with a synchronous <paramref name="invalid"/> branch
    /// (typically turning the errors into a message). For an invalid validation the result is already complete.
    /// </summary>
    /// <typeparam name="TResult">The type of the value both branches produce.</typeparam>
    /// <param name="valid">Produces the value from the value of a valid validation.</param>
    /// <param name="invalid">Produces the value from the errors, in order.</param>
    /// <returns>The value produced by the branch that ran.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="valid"/> or <paramref name="invalid"/> is null.</exception>
    /// <exception cref="InvalidOperationException">The validation is uninitialized (default).</exception>
    public ValueTask<TResult> MatchAsync<TResult>(Func<T, Task<TResult>> valid, Func<ImmutableArray<TError>, TResult> invalid)
    {
        ArgumentNullException.ThrowIfNull(valid);
        ArgumentNullException.ThrowIfNull(invalid);
        return _case switch
        {
            ValidatedCase.Valid => Start(valid, _valid.Value),
            ValidatedCase.Invalid => Evaluate(invalid, _invalid.Errors),
            _ => throw Uninitialized(),
        };
    }

    /// <summary>
    /// Asynchronously transforms the value of a valid validation; the errors of an invalid one pass through at once,
    /// without waiting.
    /// </summary>
    /// <typeparam name="TResult">The type of the transformed value.</typeparam>
    /// <param name="map">Transforms the value.</param>
    /// <returns>A valid validation with the transformed value, or the same errors.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="map"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// The validation is uninitialized (default); or, when awaited, <paramref name="map"/> produced <see langword="null"/>.
    /// </exception>
    public ValueTask<Validated<TResult, TError>> MapAsync<TResult>(Func<T, Task<TResult>> map)
        where TResult : notnull
    {
        ArgumentNullException.ThrowIfNull(map);
        return _case switch
        {
            ValidatedCase.Valid => MapValueAsync(_valid.Value, map),
            ValidatedCase.Invalid => ValueTask.FromResult(new Validated<TResult, TError>(_invalid)),
            _ => throw Uninitialized(),
        };

        static async ValueTask<Validated<TResult, TError>> MapValueAsync(T value, Func<T, Task<TResult>> map) =>
            new(new Valid<TResult>(Produced(await map(value).ConfigureAwait(false), nameof(map))));
    }

    /// <summary>
    /// Continues a valid validation with an asynchronous step that depends on the value, for example a database check;
    /// the errors of an invalid one pass through at once, without waiting. Like <see cref="Then{TResult}"/>, it never
    /// accumulates errors.
    /// </summary>
    /// <typeparam name="TResult">The type of the value produced by the next step.</typeparam>
    /// <param name="next">The next step.</param>
    /// <returns>The validation produced by the next step, or the same errors.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="next"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// The validation is uninitialized (default); or, when awaited, <paramref name="next"/> produced an uninitialized
    /// validation.
    /// </exception>
    public ValueTask<Validated<TResult, TError>> ThenAsync<TResult>(Func<T, Task<Validated<TResult, TError>>> next)
        where TResult : notnull
    {
        ArgumentNullException.ThrowIfNull(next);
        return _case switch
        {
            ValidatedCase.Valid => ThenValueAsync(_valid.Value, next),
            ValidatedCase.Invalid => ValueTask.FromResult(new Validated<TResult, TError>(_invalid)),
            _ => throw Uninitialized(),
        };

        static async ValueTask<Validated<TResult, TError>> ThenValueAsync(
            T value,
            Func<T, Task<Validated<TResult, TError>>> next) =>
            Validated<TResult, TError>.Initialized(await next(value).ConfigureAwait(false), nameof(next));
    }

    // R8: a delegate's synchronous throw comes out through the ValueTask; a requested cancellation cancels it.
    private static ValueTask<TResult> Start<TArg, TResult>(Func<TArg, Task<TResult>> step, TArg arg)
    {
        try
        {
            return new ValueTask<TResult>(step(arg));
        }
        catch (OperationCanceledException canceled) when (canceled.CancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromCanceled<TResult>(canceled.CancellationToken);
        }
        catch (Exception exception)
        {
            return ValueTask.FromException<TResult>(exception);
        }
    }

    private static ValueTask<TResult> Evaluate<TArg, TResult>(Func<TArg, TResult> step, TArg arg)
    {
        try
        {
            return ValueTask.FromResult(step(arg));
        }
        catch (OperationCanceledException canceled) when (canceled.CancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromCanceled<TResult>(canceled.CancellationToken);
        }
        catch (Exception exception)
        {
            return ValueTask.FromException<TResult>(exception);
        }
    }
}
