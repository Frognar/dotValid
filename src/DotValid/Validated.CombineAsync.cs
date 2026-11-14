// Generated from one template for 2..8 validations (a script kept outside the repository); change all overloads alike.
namespace DotValid;

/// <content>
/// <c>CombineAsync</c>: two to eight independent validations that run as tasks, every error accumulated (V7, V10).
/// </content>
/// <remarks>
/// Every input is a task: C# cannot infer the types of a mix of plain validations and tasks, so a synchronous one goes
/// in as <c>Task.FromResult(validated)</c>. Every task is awaited and observed before combining (R8: an exception comes
/// out through the result, a cancellation for a requested token cancels it).
/// </remarks>
public static partial class Validated
{
    /// <summary>
    /// Waits for 2 independent validations and combines them like <c>Combine</c>: <paramref name="combine"/> receives
    /// every value when all of them are valid; otherwise the result holds every error, in argument order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The tasks run concurrently (they are started by the caller). A synchronous validation goes in as
    /// <c>Task.FromResult(validated)</c>, or combine the synchronous ones with <c>Combine</c> and join both results with
    /// <c>Zip</c>. When every task has already succeeded, the result is complete at once and nothing is allocated.
    /// </para>
    /// <para>
    /// When awaited: the exception of the first task that failed (every task is observed), a canceled result when one
    /// was canceled, <see cref="ArgumentException"/> when a task produced an uninitialized validation, and
    /// <see cref="InvalidOperationException"/> when <paramref name="combine"/> returned <see langword="null"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="TResult">The type of the combined value.</typeparam>
    /// <typeparam name="TError">The type of the errors.</typeparam>
    /// <param name="first">The first validation, still running or already done.</param>
    /// <param name="second">The second validation, still running or already done.</param>
    /// <param name="combine">Combines the values.</param>
    /// <returns>A valid validation with the combined value, or every error in argument order.</returns>
    /// <exception cref="ArgumentNullException">A task or <paramref name="combine"/> is null.</exception>
    public static ValueTask<Validated<TResult, TError>> CombineAsync<T1, T2, TResult, TError>(
        Task<Validated<T1, TError>> first,
        Task<Validated<T2, TError>> second,
        Func<T1, T2, TResult> combine)
        where T1 : notnull
        where T2 : notnull
        where TResult : notnull
        where TError : notnull
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        ArgumentNullException.ThrowIfNull(combine);
        return CombineAfter(first, second, combine);

        static async ValueTask<Validated<TResult, TError>> CombineAfter(
            Task<Validated<T1, TError>> first,
            Task<Validated<T2, TError>> second,
            Func<T1, T2, TResult> combine)
        {
            if (!(first.IsCompletedSuccessfully
                && second.IsCompletedSuccessfully))
            {
                await Task.WhenAll(first, second).ConfigureAwait(false);
            }

            return Completed(first, second, combine);
        }

        // Every task has succeeded here, so reading the results does not block.
        static Validated<TResult, TError> Completed(
            Task<Validated<T1, TError>> first,
            Task<Validated<T2, TError>> second,
            Func<T1, T2, TResult> combine) =>
            Combine(first.Result, second.Result, combine);
    }

    /// <summary>
    /// Waits for 3 independent validations and combines them like <c>Combine</c>: <paramref name="combine"/> receives
    /// every value when all of them are valid; otherwise the result holds every error, in argument order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The tasks run concurrently (they are started by the caller). A synchronous validation goes in as
    /// <c>Task.FromResult(validated)</c>, or combine the synchronous ones with <c>Combine</c> and join both results with
    /// <c>Zip</c>. When every task has already succeeded, the result is complete at once and nothing is allocated.
    /// </para>
    /// <para>
    /// When awaited: the exception of the first task that failed (every task is observed), a canceled result when one
    /// was canceled, <see cref="ArgumentException"/> when a task produced an uninitialized validation, and
    /// <see cref="InvalidOperationException"/> when <paramref name="combine"/> returned <see langword="null"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="TResult">The type of the combined value.</typeparam>
    /// <typeparam name="TError">The type of the errors.</typeparam>
    /// <param name="first">The first validation, still running or already done.</param>
    /// <param name="second">The second validation, still running or already done.</param>
    /// <param name="third">The third validation, still running or already done.</param>
    /// <param name="combine">Combines the values.</param>
    /// <returns>A valid validation with the combined value, or every error in argument order.</returns>
    /// <exception cref="ArgumentNullException">A task or <paramref name="combine"/> is null.</exception>
    public static ValueTask<Validated<TResult, TError>> CombineAsync<T1, T2, T3, TResult, TError>(
        Task<Validated<T1, TError>> first,
        Task<Validated<T2, TError>> second,
        Task<Validated<T3, TError>> third,
        Func<T1, T2, T3, TResult> combine)
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where TResult : notnull
        where TError : notnull
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        ArgumentNullException.ThrowIfNull(third);
        ArgumentNullException.ThrowIfNull(combine);
        return CombineAfter(first, second, third, combine);

        static async ValueTask<Validated<TResult, TError>> CombineAfter(
            Task<Validated<T1, TError>> first,
            Task<Validated<T2, TError>> second,
            Task<Validated<T3, TError>> third,
            Func<T1, T2, T3, TResult> combine)
        {
            if (!(first.IsCompletedSuccessfully
                && second.IsCompletedSuccessfully
                && third.IsCompletedSuccessfully))
            {
                await Task.WhenAll(first, second, third).ConfigureAwait(false);
            }

            return Completed(first, second, third, combine);
        }

        // Every task has succeeded here, so reading the results does not block.
        static Validated<TResult, TError> Completed(
            Task<Validated<T1, TError>> first,
            Task<Validated<T2, TError>> second,
            Task<Validated<T3, TError>> third,
            Func<T1, T2, T3, TResult> combine) =>
            Combine(first.Result, second.Result, third.Result, combine);
    }

    /// <summary>
    /// Waits for 4 independent validations and combines them like <c>Combine</c>: <paramref name="combine"/> receives
    /// every value when all of them are valid; otherwise the result holds every error, in argument order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The tasks run concurrently (they are started by the caller). A synchronous validation goes in as
    /// <c>Task.FromResult(validated)</c>, or combine the synchronous ones with <c>Combine</c> and join both results with
    /// <c>Zip</c>. When every task has already succeeded, the result is complete at once and nothing is allocated.
    /// </para>
    /// <para>
    /// When awaited: the exception of the first task that failed (every task is observed), a canceled result when one
    /// was canceled, <see cref="ArgumentException"/> when a task produced an uninitialized validation, and
    /// <see cref="InvalidOperationException"/> when <paramref name="combine"/> returned <see langword="null"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="TResult">The type of the combined value.</typeparam>
    /// <typeparam name="TError">The type of the errors.</typeparam>
    /// <param name="first">The first validation, still running or already done.</param>
    /// <param name="second">The second validation, still running or already done.</param>
    /// <param name="third">The third validation, still running or already done.</param>
    /// <param name="fourth">The fourth validation, still running or already done.</param>
    /// <param name="combine">Combines the values.</param>
    /// <returns>A valid validation with the combined value, or every error in argument order.</returns>
    /// <exception cref="ArgumentNullException">A task or <paramref name="combine"/> is null.</exception>
    public static ValueTask<Validated<TResult, TError>> CombineAsync<T1, T2, T3, T4, TResult, TError>(
        Task<Validated<T1, TError>> first,
        Task<Validated<T2, TError>> second,
        Task<Validated<T3, TError>> third,
        Task<Validated<T4, TError>> fourth,
        Func<T1, T2, T3, T4, TResult> combine)
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
        where TResult : notnull
        where TError : notnull
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        ArgumentNullException.ThrowIfNull(third);
        ArgumentNullException.ThrowIfNull(fourth);
        ArgumentNullException.ThrowIfNull(combine);
        return CombineAfter(first, second, third, fourth, combine);

        static async ValueTask<Validated<TResult, TError>> CombineAfter(
            Task<Validated<T1, TError>> first,
            Task<Validated<T2, TError>> second,
            Task<Validated<T3, TError>> third,
            Task<Validated<T4, TError>> fourth,
            Func<T1, T2, T3, T4, TResult> combine)
        {
            if (!(first.IsCompletedSuccessfully
                && second.IsCompletedSuccessfully
                && third.IsCompletedSuccessfully
                && fourth.IsCompletedSuccessfully))
            {
                await Task.WhenAll(first, second, third, fourth).ConfigureAwait(false);
            }

            return Completed(first, second, third, fourth, combine);
        }

        // Every task has succeeded here, so reading the results does not block.
        static Validated<TResult, TError> Completed(
            Task<Validated<T1, TError>> first,
            Task<Validated<T2, TError>> second,
            Task<Validated<T3, TError>> third,
            Task<Validated<T4, TError>> fourth,
            Func<T1, T2, T3, T4, TResult> combine) =>
            Combine(first.Result, second.Result, third.Result, fourth.Result, combine);
    }

    /// <summary>
    /// Waits for 5 independent validations and combines them like <c>Combine</c>: <paramref name="combine"/> receives
    /// every value when all of them are valid; otherwise the result holds every error, in argument order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The tasks run concurrently (they are started by the caller). A synchronous validation goes in as
    /// <c>Task.FromResult(validated)</c>, or combine the synchronous ones with <c>Combine</c> and join both results with
    /// <c>Zip</c>. When every task has already succeeded, the result is complete at once and nothing is allocated.
    /// </para>
    /// <para>
    /// When awaited: the exception of the first task that failed (every task is observed), a canceled result when one
    /// was canceled, <see cref="ArgumentException"/> when a task produced an uninitialized validation, and
    /// <see cref="InvalidOperationException"/> when <paramref name="combine"/> returned <see langword="null"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="T5">The type of the fifth value.</typeparam>
    /// <typeparam name="TResult">The type of the combined value.</typeparam>
    /// <typeparam name="TError">The type of the errors.</typeparam>
    /// <param name="first">The first validation, still running or already done.</param>
    /// <param name="second">The second validation, still running or already done.</param>
    /// <param name="third">The third validation, still running or already done.</param>
    /// <param name="fourth">The fourth validation, still running or already done.</param>
    /// <param name="fifth">The fifth validation, still running or already done.</param>
    /// <param name="combine">Combines the values.</param>
    /// <returns>A valid validation with the combined value, or every error in argument order.</returns>
    /// <exception cref="ArgumentNullException">A task or <paramref name="combine"/> is null.</exception>
    public static ValueTask<Validated<TResult, TError>> CombineAsync<T1, T2, T3, T4, T5, TResult, TError>(
        Task<Validated<T1, TError>> first,
        Task<Validated<T2, TError>> second,
        Task<Validated<T3, TError>> third,
        Task<Validated<T4, TError>> fourth,
        Task<Validated<T5, TError>> fifth,
        Func<T1, T2, T3, T4, T5, TResult> combine)
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
        where T5 : notnull
        where TResult : notnull
        where TError : notnull
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        ArgumentNullException.ThrowIfNull(third);
        ArgumentNullException.ThrowIfNull(fourth);
        ArgumentNullException.ThrowIfNull(fifth);
        ArgumentNullException.ThrowIfNull(combine);
        return CombineAfter(first, second, third, fourth, fifth, combine);

        static async ValueTask<Validated<TResult, TError>> CombineAfter(
            Task<Validated<T1, TError>> first,
            Task<Validated<T2, TError>> second,
            Task<Validated<T3, TError>> third,
            Task<Validated<T4, TError>> fourth,
            Task<Validated<T5, TError>> fifth,
            Func<T1, T2, T3, T4, T5, TResult> combine)
        {
            if (!(first.IsCompletedSuccessfully
                && second.IsCompletedSuccessfully
                && third.IsCompletedSuccessfully
                && fourth.IsCompletedSuccessfully
                && fifth.IsCompletedSuccessfully))
            {
                await Task.WhenAll(first, second, third, fourth, fifth).ConfigureAwait(false);
            }

            return Completed(first, second, third, fourth, fifth, combine);
        }

        // Every task has succeeded here, so reading the results does not block.
        static Validated<TResult, TError> Completed(
            Task<Validated<T1, TError>> first,
            Task<Validated<T2, TError>> second,
            Task<Validated<T3, TError>> third,
            Task<Validated<T4, TError>> fourth,
            Task<Validated<T5, TError>> fifth,
            Func<T1, T2, T3, T4, T5, TResult> combine) =>
            Combine(first.Result, second.Result, third.Result, fourth.Result, fifth.Result, combine);
    }

    /// <summary>
    /// Waits for 6 independent validations and combines them like <c>Combine</c>: <paramref name="combine"/> receives
    /// every value when all of them are valid; otherwise the result holds every error, in argument order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The tasks run concurrently (they are started by the caller). A synchronous validation goes in as
    /// <c>Task.FromResult(validated)</c>, or combine the synchronous ones with <c>Combine</c> and join both results with
    /// <c>Zip</c>. When every task has already succeeded, the result is complete at once and nothing is allocated.
    /// </para>
    /// <para>
    /// When awaited: the exception of the first task that failed (every task is observed), a canceled result when one
    /// was canceled, <see cref="ArgumentException"/> when a task produced an uninitialized validation, and
    /// <see cref="InvalidOperationException"/> when <paramref name="combine"/> returned <see langword="null"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="T5">The type of the fifth value.</typeparam>
    /// <typeparam name="T6">The type of the sixth value.</typeparam>
    /// <typeparam name="TResult">The type of the combined value.</typeparam>
    /// <typeparam name="TError">The type of the errors.</typeparam>
    /// <param name="first">The first validation, still running or already done.</param>
    /// <param name="second">The second validation, still running or already done.</param>
    /// <param name="third">The third validation, still running or already done.</param>
    /// <param name="fourth">The fourth validation, still running or already done.</param>
    /// <param name="fifth">The fifth validation, still running or already done.</param>
    /// <param name="sixth">The sixth validation, still running or already done.</param>
    /// <param name="combine">Combines the values.</param>
    /// <returns>A valid validation with the combined value, or every error in argument order.</returns>
    /// <exception cref="ArgumentNullException">A task or <paramref name="combine"/> is null.</exception>
    public static ValueTask<Validated<TResult, TError>> CombineAsync<T1, T2, T3, T4, T5, T6, TResult, TError>(
        Task<Validated<T1, TError>> first,
        Task<Validated<T2, TError>> second,
        Task<Validated<T3, TError>> third,
        Task<Validated<T4, TError>> fourth,
        Task<Validated<T5, TError>> fifth,
        Task<Validated<T6, TError>> sixth,
        Func<T1, T2, T3, T4, T5, T6, TResult> combine)
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
        where T5 : notnull
        where T6 : notnull
        where TResult : notnull
        where TError : notnull
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        ArgumentNullException.ThrowIfNull(third);
        ArgumentNullException.ThrowIfNull(fourth);
        ArgumentNullException.ThrowIfNull(fifth);
        ArgumentNullException.ThrowIfNull(sixth);
        ArgumentNullException.ThrowIfNull(combine);
        return CombineAfter(first, second, third, fourth, fifth, sixth, combine);

        static async ValueTask<Validated<TResult, TError>> CombineAfter(
            Task<Validated<T1, TError>> first,
            Task<Validated<T2, TError>> second,
            Task<Validated<T3, TError>> third,
            Task<Validated<T4, TError>> fourth,
            Task<Validated<T5, TError>> fifth,
            Task<Validated<T6, TError>> sixth,
            Func<T1, T2, T3, T4, T5, T6, TResult> combine)
        {
            if (!(first.IsCompletedSuccessfully
                && second.IsCompletedSuccessfully
                && third.IsCompletedSuccessfully
                && fourth.IsCompletedSuccessfully
                && fifth.IsCompletedSuccessfully
                && sixth.IsCompletedSuccessfully))
            {
                await Task.WhenAll(first, second, third, fourth, fifth, sixth).ConfigureAwait(false);
            }

            return Completed(first, second, third, fourth, fifth, sixth, combine);
        }

        // Every task has succeeded here, so reading the results does not block.
        static Validated<TResult, TError> Completed(
            Task<Validated<T1, TError>> first,
            Task<Validated<T2, TError>> second,
            Task<Validated<T3, TError>> third,
            Task<Validated<T4, TError>> fourth,
            Task<Validated<T5, TError>> fifth,
            Task<Validated<T6, TError>> sixth,
            Func<T1, T2, T3, T4, T5, T6, TResult> combine) =>
            Combine(first.Result, second.Result, third.Result, fourth.Result, fifth.Result, sixth.Result, combine);
    }

    /// <summary>
    /// Waits for 7 independent validations and combines them like <c>Combine</c>: <paramref name="combine"/> receives
    /// every value when all of them are valid; otherwise the result holds every error, in argument order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The tasks run concurrently (they are started by the caller). A synchronous validation goes in as
    /// <c>Task.FromResult(validated)</c>, or combine the synchronous ones with <c>Combine</c> and join both results with
    /// <c>Zip</c>. When every task has already succeeded, the result is complete at once and nothing is allocated.
    /// </para>
    /// <para>
    /// When awaited: the exception of the first task that failed (every task is observed), a canceled result when one
    /// was canceled, <see cref="ArgumentException"/> when a task produced an uninitialized validation, and
    /// <see cref="InvalidOperationException"/> when <paramref name="combine"/> returned <see langword="null"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="T5">The type of the fifth value.</typeparam>
    /// <typeparam name="T6">The type of the sixth value.</typeparam>
    /// <typeparam name="T7">The type of the seventh value.</typeparam>
    /// <typeparam name="TResult">The type of the combined value.</typeparam>
    /// <typeparam name="TError">The type of the errors.</typeparam>
    /// <param name="first">The first validation, still running or already done.</param>
    /// <param name="second">The second validation, still running or already done.</param>
    /// <param name="third">The third validation, still running or already done.</param>
    /// <param name="fourth">The fourth validation, still running or already done.</param>
    /// <param name="fifth">The fifth validation, still running or already done.</param>
    /// <param name="sixth">The sixth validation, still running or already done.</param>
    /// <param name="seventh">The seventh validation, still running or already done.</param>
    /// <param name="combine">Combines the values.</param>
    /// <returns>A valid validation with the combined value, or every error in argument order.</returns>
    /// <exception cref="ArgumentNullException">A task or <paramref name="combine"/> is null.</exception>
    public static ValueTask<Validated<TResult, TError>> CombineAsync<T1, T2, T3, T4, T5, T6, T7, TResult, TError>(
        Task<Validated<T1, TError>> first,
        Task<Validated<T2, TError>> second,
        Task<Validated<T3, TError>> third,
        Task<Validated<T4, TError>> fourth,
        Task<Validated<T5, TError>> fifth,
        Task<Validated<T6, TError>> sixth,
        Task<Validated<T7, TError>> seventh,
        Func<T1, T2, T3, T4, T5, T6, T7, TResult> combine)
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
        where T5 : notnull
        where T6 : notnull
        where T7 : notnull
        where TResult : notnull
        where TError : notnull
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        ArgumentNullException.ThrowIfNull(third);
        ArgumentNullException.ThrowIfNull(fourth);
        ArgumentNullException.ThrowIfNull(fifth);
        ArgumentNullException.ThrowIfNull(sixth);
        ArgumentNullException.ThrowIfNull(seventh);
        ArgumentNullException.ThrowIfNull(combine);
        return CombineAfter(first, second, third, fourth, fifth, sixth, seventh, combine);

        static async ValueTask<Validated<TResult, TError>> CombineAfter(
            Task<Validated<T1, TError>> first,
            Task<Validated<T2, TError>> second,
            Task<Validated<T3, TError>> third,
            Task<Validated<T4, TError>> fourth,
            Task<Validated<T5, TError>> fifth,
            Task<Validated<T6, TError>> sixth,
            Task<Validated<T7, TError>> seventh,
            Func<T1, T2, T3, T4, T5, T6, T7, TResult> combine)
        {
            if (!(first.IsCompletedSuccessfully
                && second.IsCompletedSuccessfully
                && third.IsCompletedSuccessfully
                && fourth.IsCompletedSuccessfully
                && fifth.IsCompletedSuccessfully
                && sixth.IsCompletedSuccessfully
                && seventh.IsCompletedSuccessfully))
            {
                await Task.WhenAll(first, second, third, fourth, fifth, sixth, seventh).ConfigureAwait(false);
            }

            return Completed(first, second, third, fourth, fifth, sixth, seventh, combine);
        }

        // Every task has succeeded here, so reading the results does not block.
        static Validated<TResult, TError> Completed(
            Task<Validated<T1, TError>> first,
            Task<Validated<T2, TError>> second,
            Task<Validated<T3, TError>> third,
            Task<Validated<T4, TError>> fourth,
            Task<Validated<T5, TError>> fifth,
            Task<Validated<T6, TError>> sixth,
            Task<Validated<T7, TError>> seventh,
            Func<T1, T2, T3, T4, T5, T6, T7, TResult> combine) =>
            Combine(first.Result, second.Result, third.Result, fourth.Result, fifth.Result, sixth.Result, seventh.Result, combine);
    }

    /// <summary>
    /// Waits for 8 independent validations and combines them like <c>Combine</c>: <paramref name="combine"/> receives
    /// every value when all of them are valid; otherwise the result holds every error, in argument order.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The tasks run concurrently (they are started by the caller). A synchronous validation goes in as
    /// <c>Task.FromResult(validated)</c>, or combine the synchronous ones with <c>Combine</c> and join both results with
    /// <c>Zip</c>. When every task has already succeeded, the result is complete at once and nothing is allocated.
    /// </para>
    /// <para>
    /// When awaited: the exception of the first task that failed (every task is observed), a canceled result when one
    /// was canceled, <see cref="ArgumentException"/> when a task produced an uninitialized validation, and
    /// <see cref="InvalidOperationException"/> when <paramref name="combine"/> returned <see langword="null"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="T5">The type of the fifth value.</typeparam>
    /// <typeparam name="T6">The type of the sixth value.</typeparam>
    /// <typeparam name="T7">The type of the seventh value.</typeparam>
    /// <typeparam name="T8">The type of the eighth value.</typeparam>
    /// <typeparam name="TResult">The type of the combined value.</typeparam>
    /// <typeparam name="TError">The type of the errors.</typeparam>
    /// <param name="first">The first validation, still running or already done.</param>
    /// <param name="second">The second validation, still running or already done.</param>
    /// <param name="third">The third validation, still running or already done.</param>
    /// <param name="fourth">The fourth validation, still running or already done.</param>
    /// <param name="fifth">The fifth validation, still running or already done.</param>
    /// <param name="sixth">The sixth validation, still running or already done.</param>
    /// <param name="seventh">The seventh validation, still running or already done.</param>
    /// <param name="eighth">The eighth validation, still running or already done.</param>
    /// <param name="combine">Combines the values.</param>
    /// <returns>A valid validation with the combined value, or every error in argument order.</returns>
    /// <exception cref="ArgumentNullException">A task or <paramref name="combine"/> is null.</exception>
    public static ValueTask<Validated<TResult, TError>> CombineAsync<T1, T2, T3, T4, T5, T6, T7, T8, TResult, TError>(
        Task<Validated<T1, TError>> first,
        Task<Validated<T2, TError>> second,
        Task<Validated<T3, TError>> third,
        Task<Validated<T4, TError>> fourth,
        Task<Validated<T5, TError>> fifth,
        Task<Validated<T6, TError>> sixth,
        Task<Validated<T7, TError>> seventh,
        Task<Validated<T8, TError>> eighth,
        Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> combine)
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
        where T5 : notnull
        where T6 : notnull
        where T7 : notnull
        where T8 : notnull
        where TResult : notnull
        where TError : notnull
    {
        ArgumentNullException.ThrowIfNull(first);
        ArgumentNullException.ThrowIfNull(second);
        ArgumentNullException.ThrowIfNull(third);
        ArgumentNullException.ThrowIfNull(fourth);
        ArgumentNullException.ThrowIfNull(fifth);
        ArgumentNullException.ThrowIfNull(sixth);
        ArgumentNullException.ThrowIfNull(seventh);
        ArgumentNullException.ThrowIfNull(eighth);
        ArgumentNullException.ThrowIfNull(combine);
        return CombineAfter(first, second, third, fourth, fifth, sixth, seventh, eighth, combine);

        static async ValueTask<Validated<TResult, TError>> CombineAfter(
            Task<Validated<T1, TError>> first,
            Task<Validated<T2, TError>> second,
            Task<Validated<T3, TError>> third,
            Task<Validated<T4, TError>> fourth,
            Task<Validated<T5, TError>> fifth,
            Task<Validated<T6, TError>> sixth,
            Task<Validated<T7, TError>> seventh,
            Task<Validated<T8, TError>> eighth,
            Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> combine)
        {
            if (!(first.IsCompletedSuccessfully
                && second.IsCompletedSuccessfully
                && third.IsCompletedSuccessfully
                && fourth.IsCompletedSuccessfully
                && fifth.IsCompletedSuccessfully
                && sixth.IsCompletedSuccessfully
                && seventh.IsCompletedSuccessfully
                && eighth.IsCompletedSuccessfully))
            {
                await Task.WhenAll(first, second, third, fourth, fifth, sixth, seventh, eighth).ConfigureAwait(false);
            }

            return Completed(first, second, third, fourth, fifth, sixth, seventh, eighth, combine);
        }

        // Every task has succeeded here, so reading the results does not block.
        static Validated<TResult, TError> Completed(
            Task<Validated<T1, TError>> first,
            Task<Validated<T2, TError>> second,
            Task<Validated<T3, TError>> third,
            Task<Validated<T4, TError>> fourth,
            Task<Validated<T5, TError>> fifth,
            Task<Validated<T6, TError>> sixth,
            Task<Validated<T7, TError>> seventh,
            Task<Validated<T8, TError>> eighth,
            Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> combine) =>
            Combine(first.Result, second.Result, third.Result, fourth.Result, fifth.Result, sixth.Result, seventh.Result, eighth.Result, combine);
    }
}
