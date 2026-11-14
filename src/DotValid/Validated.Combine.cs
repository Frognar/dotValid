// Generated from one template for 2..8 validations (a script kept outside the repository); change all overloads alike.
namespace DotValid;

/// <content>
/// <c>Combine</c>: two to eight independent validations, every error accumulated (V7).
/// </content>
public static partial class Validated
{
    /// <summary>
    /// Combines 2 independent validations: <paramref name="combine"/> receives every value when all of them are valid;
    /// otherwise the result holds the errors of every invalid one, in argument order.
    /// </summary>
    /// <remarks>
    /// Every validation is read, none is skipped: this is how independent checks report every problem at once.
    /// <paramref name="combine"/> runs only when all of them are valid, exactly once.
    /// </remarks>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="TResult">The type of the combined value.</typeparam>
    /// <typeparam name="TError">The type of the errors.</typeparam>
    /// <param name="first">The first validation.</param>
    /// <param name="second">The second validation.</param>
    /// <param name="combine">Combines the values.</param>
    /// <returns>A valid validation with the combined value, or every error in argument order.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="combine"/> is null.</exception>
    /// <exception cref="ArgumentException">One of <paramref name="first"/>, <paramref name="second"/> is uninitialized (default).</exception>
    /// <exception cref="InvalidOperationException"><paramref name="combine"/> returned <see langword="null"/>.</exception>
    public static Validated<TResult, TError> Combine<T1, T2, TResult, TError>(
        Validated<T1, TError> first,
        Validated<T2, TError> second,
        Func<T1, T2, TResult> combine)
        where T1 : notnull
        where T2 : notnull
        where TResult : notnull
        where TError : notnull
    {
        Validated<T1, TError>.RejectUninitialized(first, nameof(first));
        Validated<T2, TError>.RejectUninitialized(second, nameof(second));
        ArgumentNullException.ThrowIfNull(combine);
        return first.TryGetValid(out var firstValue)
            && second.TryGetValid(out var secondValue)
            ? Validated<TResult, TError>.FromValue(Validated<TResult, TError>.Produced(combine(firstValue, secondValue), nameof(combine)))
            : Validated<TResult, TError>.FromErrors(first.InvalidErrors, second.InvalidErrors);
    }

    /// <summary>
    /// Combines 3 independent validations: <paramref name="combine"/> receives every value when all of them are valid;
    /// otherwise the result holds the errors of every invalid one, in argument order.
    /// </summary>
    /// <remarks>
    /// Every validation is read, none is skipped: this is how independent checks report every problem at once.
    /// <paramref name="combine"/> runs only when all of them are valid, exactly once.
    /// </remarks>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="TResult">The type of the combined value.</typeparam>
    /// <typeparam name="TError">The type of the errors.</typeparam>
    /// <param name="first">The first validation.</param>
    /// <param name="second">The second validation.</param>
    /// <param name="third">The third validation.</param>
    /// <param name="combine">Combines the values.</param>
    /// <returns>A valid validation with the combined value, or every error in argument order.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="combine"/> is null.</exception>
    /// <exception cref="ArgumentException">One of <paramref name="first"/>, <paramref name="second"/>, <paramref name="third"/> is uninitialized (default).</exception>
    /// <exception cref="InvalidOperationException"><paramref name="combine"/> returned <see langword="null"/>.</exception>
    public static Validated<TResult, TError> Combine<T1, T2, T3, TResult, TError>(
        Validated<T1, TError> first,
        Validated<T2, TError> second,
        Validated<T3, TError> third,
        Func<T1, T2, T3, TResult> combine)
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where TResult : notnull
        where TError : notnull
    {
        Validated<T1, TError>.RejectUninitialized(first, nameof(first));
        Validated<T2, TError>.RejectUninitialized(second, nameof(second));
        Validated<T3, TError>.RejectUninitialized(third, nameof(third));
        ArgumentNullException.ThrowIfNull(combine);
        return first.TryGetValid(out var firstValue)
            && second.TryGetValid(out var secondValue)
            && third.TryGetValid(out var thirdValue)
            ? Validated<TResult, TError>.FromValue(Validated<TResult, TError>.Produced(combine(firstValue, secondValue, thirdValue), nameof(combine)))
            : Validated<TResult, TError>.FromErrors(first.InvalidErrors, second.InvalidErrors, third.InvalidErrors);
    }

    /// <summary>
    /// Combines 4 independent validations: <paramref name="combine"/> receives every value when all of them are valid;
    /// otherwise the result holds the errors of every invalid one, in argument order.
    /// </summary>
    /// <remarks>
    /// Every validation is read, none is skipped: this is how independent checks report every problem at once.
    /// <paramref name="combine"/> runs only when all of them are valid, exactly once.
    /// </remarks>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="TResult">The type of the combined value.</typeparam>
    /// <typeparam name="TError">The type of the errors.</typeparam>
    /// <param name="first">The first validation.</param>
    /// <param name="second">The second validation.</param>
    /// <param name="third">The third validation.</param>
    /// <param name="fourth">The fourth validation.</param>
    /// <param name="combine">Combines the values.</param>
    /// <returns>A valid validation with the combined value, or every error in argument order.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="combine"/> is null.</exception>
    /// <exception cref="ArgumentException">One of <paramref name="first"/>, <paramref name="second"/>, <paramref name="third"/>, <paramref name="fourth"/> is uninitialized (default).</exception>
    /// <exception cref="InvalidOperationException"><paramref name="combine"/> returned <see langword="null"/>.</exception>
    public static Validated<TResult, TError> Combine<T1, T2, T3, T4, TResult, TError>(
        Validated<T1, TError> first,
        Validated<T2, TError> second,
        Validated<T3, TError> third,
        Validated<T4, TError> fourth,
        Func<T1, T2, T3, T4, TResult> combine)
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
        where TResult : notnull
        where TError : notnull
    {
        Validated<T1, TError>.RejectUninitialized(first, nameof(first));
        Validated<T2, TError>.RejectUninitialized(second, nameof(second));
        Validated<T3, TError>.RejectUninitialized(third, nameof(third));
        Validated<T4, TError>.RejectUninitialized(fourth, nameof(fourth));
        ArgumentNullException.ThrowIfNull(combine);
        return first.TryGetValid(out var firstValue)
            && second.TryGetValid(out var secondValue)
            && third.TryGetValid(out var thirdValue)
            && fourth.TryGetValid(out var fourthValue)
            ? Validated<TResult, TError>.FromValue(Validated<TResult, TError>.Produced(combine(firstValue, secondValue, thirdValue, fourthValue), nameof(combine)))
            : Validated<TResult, TError>.FromErrors(first.InvalidErrors, second.InvalidErrors, third.InvalidErrors, fourth.InvalidErrors);
    }

    /// <summary>
    /// Combines 5 independent validations: <paramref name="combine"/> receives every value when all of them are valid;
    /// otherwise the result holds the errors of every invalid one, in argument order.
    /// </summary>
    /// <remarks>
    /// Every validation is read, none is skipped: this is how independent checks report every problem at once.
    /// <paramref name="combine"/> runs only when all of them are valid, exactly once.
    /// </remarks>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="T5">The type of the fifth value.</typeparam>
    /// <typeparam name="TResult">The type of the combined value.</typeparam>
    /// <typeparam name="TError">The type of the errors.</typeparam>
    /// <param name="first">The first validation.</param>
    /// <param name="second">The second validation.</param>
    /// <param name="third">The third validation.</param>
    /// <param name="fourth">The fourth validation.</param>
    /// <param name="fifth">The fifth validation.</param>
    /// <param name="combine">Combines the values.</param>
    /// <returns>A valid validation with the combined value, or every error in argument order.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="combine"/> is null.</exception>
    /// <exception cref="ArgumentException">One of <paramref name="first"/>, <paramref name="second"/>, <paramref name="third"/>, <paramref name="fourth"/>, <paramref name="fifth"/> is uninitialized (default).</exception>
    /// <exception cref="InvalidOperationException"><paramref name="combine"/> returned <see langword="null"/>.</exception>
    public static Validated<TResult, TError> Combine<T1, T2, T3, T4, T5, TResult, TError>(
        Validated<T1, TError> first,
        Validated<T2, TError> second,
        Validated<T3, TError> third,
        Validated<T4, TError> fourth,
        Validated<T5, TError> fifth,
        Func<T1, T2, T3, T4, T5, TResult> combine)
        where T1 : notnull
        where T2 : notnull
        where T3 : notnull
        where T4 : notnull
        where T5 : notnull
        where TResult : notnull
        where TError : notnull
    {
        Validated<T1, TError>.RejectUninitialized(first, nameof(first));
        Validated<T2, TError>.RejectUninitialized(second, nameof(second));
        Validated<T3, TError>.RejectUninitialized(third, nameof(third));
        Validated<T4, TError>.RejectUninitialized(fourth, nameof(fourth));
        Validated<T5, TError>.RejectUninitialized(fifth, nameof(fifth));
        ArgumentNullException.ThrowIfNull(combine);
        return first.TryGetValid(out var firstValue)
            && second.TryGetValid(out var secondValue)
            && third.TryGetValid(out var thirdValue)
            && fourth.TryGetValid(out var fourthValue)
            && fifth.TryGetValid(out var fifthValue)
            ? Validated<TResult, TError>.FromValue(Validated<TResult, TError>.Produced(combine(firstValue, secondValue, thirdValue, fourthValue, fifthValue), nameof(combine)))
            : Validated<TResult, TError>.FromErrors(first.InvalidErrors, second.InvalidErrors, third.InvalidErrors, fourth.InvalidErrors, fifth.InvalidErrors);
    }

    /// <summary>
    /// Combines 6 independent validations: <paramref name="combine"/> receives every value when all of them are valid;
    /// otherwise the result holds the errors of every invalid one, in argument order.
    /// </summary>
    /// <remarks>
    /// Every validation is read, none is skipped: this is how independent checks report every problem at once.
    /// <paramref name="combine"/> runs only when all of them are valid, exactly once.
    /// </remarks>
    /// <typeparam name="T1">The type of the first value.</typeparam>
    /// <typeparam name="T2">The type of the second value.</typeparam>
    /// <typeparam name="T3">The type of the third value.</typeparam>
    /// <typeparam name="T4">The type of the fourth value.</typeparam>
    /// <typeparam name="T5">The type of the fifth value.</typeparam>
    /// <typeparam name="T6">The type of the sixth value.</typeparam>
    /// <typeparam name="TResult">The type of the combined value.</typeparam>
    /// <typeparam name="TError">The type of the errors.</typeparam>
    /// <param name="first">The first validation.</param>
    /// <param name="second">The second validation.</param>
    /// <param name="third">The third validation.</param>
    /// <param name="fourth">The fourth validation.</param>
    /// <param name="fifth">The fifth validation.</param>
    /// <param name="sixth">The sixth validation.</param>
    /// <param name="combine">Combines the values.</param>
    /// <returns>A valid validation with the combined value, or every error in argument order.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="combine"/> is null.</exception>
    /// <exception cref="ArgumentException">One of <paramref name="first"/>, <paramref name="second"/>, <paramref name="third"/>, <paramref name="fourth"/>, <paramref name="fifth"/>, <paramref name="sixth"/> is uninitialized (default).</exception>
    /// <exception cref="InvalidOperationException"><paramref name="combine"/> returned <see langword="null"/>.</exception>
    public static Validated<TResult, TError> Combine<T1, T2, T3, T4, T5, T6, TResult, TError>(
        Validated<T1, TError> first,
        Validated<T2, TError> second,
        Validated<T3, TError> third,
        Validated<T4, TError> fourth,
        Validated<T5, TError> fifth,
        Validated<T6, TError> sixth,
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
        Validated<T1, TError>.RejectUninitialized(first, nameof(first));
        Validated<T2, TError>.RejectUninitialized(second, nameof(second));
        Validated<T3, TError>.RejectUninitialized(third, nameof(third));
        Validated<T4, TError>.RejectUninitialized(fourth, nameof(fourth));
        Validated<T5, TError>.RejectUninitialized(fifth, nameof(fifth));
        Validated<T6, TError>.RejectUninitialized(sixth, nameof(sixth));
        ArgumentNullException.ThrowIfNull(combine);
        return first.TryGetValid(out var firstValue)
            && second.TryGetValid(out var secondValue)
            && third.TryGetValid(out var thirdValue)
            && fourth.TryGetValid(out var fourthValue)
            && fifth.TryGetValid(out var fifthValue)
            && sixth.TryGetValid(out var sixthValue)
            ? Validated<TResult, TError>.FromValue(Validated<TResult, TError>.Produced(combine(firstValue, secondValue, thirdValue, fourthValue, fifthValue, sixthValue), nameof(combine)))
            : Validated<TResult, TError>.FromErrors(first.InvalidErrors, second.InvalidErrors, third.InvalidErrors, fourth.InvalidErrors, fifth.InvalidErrors, sixth.InvalidErrors);
    }

    /// <summary>
    /// Combines 7 independent validations: <paramref name="combine"/> receives every value when all of them are valid;
    /// otherwise the result holds the errors of every invalid one, in argument order.
    /// </summary>
    /// <remarks>
    /// Every validation is read, none is skipped: this is how independent checks report every problem at once.
    /// <paramref name="combine"/> runs only when all of them are valid, exactly once.
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
    /// <param name="first">The first validation.</param>
    /// <param name="second">The second validation.</param>
    /// <param name="third">The third validation.</param>
    /// <param name="fourth">The fourth validation.</param>
    /// <param name="fifth">The fifth validation.</param>
    /// <param name="sixth">The sixth validation.</param>
    /// <param name="seventh">The seventh validation.</param>
    /// <param name="combine">Combines the values.</param>
    /// <returns>A valid validation with the combined value, or every error in argument order.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="combine"/> is null.</exception>
    /// <exception cref="ArgumentException">One of <paramref name="first"/>, <paramref name="second"/>, <paramref name="third"/>, <paramref name="fourth"/>, <paramref name="fifth"/>, <paramref name="sixth"/>, <paramref name="seventh"/> is uninitialized (default).</exception>
    /// <exception cref="InvalidOperationException"><paramref name="combine"/> returned <see langword="null"/>.</exception>
    public static Validated<TResult, TError> Combine<T1, T2, T3, T4, T5, T6, T7, TResult, TError>(
        Validated<T1, TError> first,
        Validated<T2, TError> second,
        Validated<T3, TError> third,
        Validated<T4, TError> fourth,
        Validated<T5, TError> fifth,
        Validated<T6, TError> sixth,
        Validated<T7, TError> seventh,
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
        Validated<T1, TError>.RejectUninitialized(first, nameof(first));
        Validated<T2, TError>.RejectUninitialized(second, nameof(second));
        Validated<T3, TError>.RejectUninitialized(third, nameof(third));
        Validated<T4, TError>.RejectUninitialized(fourth, nameof(fourth));
        Validated<T5, TError>.RejectUninitialized(fifth, nameof(fifth));
        Validated<T6, TError>.RejectUninitialized(sixth, nameof(sixth));
        Validated<T7, TError>.RejectUninitialized(seventh, nameof(seventh));
        ArgumentNullException.ThrowIfNull(combine);
        return first.TryGetValid(out var firstValue)
            && second.TryGetValid(out var secondValue)
            && third.TryGetValid(out var thirdValue)
            && fourth.TryGetValid(out var fourthValue)
            && fifth.TryGetValid(out var fifthValue)
            && sixth.TryGetValid(out var sixthValue)
            && seventh.TryGetValid(out var seventhValue)
            ? Validated<TResult, TError>.FromValue(Validated<TResult, TError>.Produced(combine(firstValue, secondValue, thirdValue, fourthValue, fifthValue, sixthValue, seventhValue), nameof(combine)))
            : Validated<TResult, TError>.FromErrors(first.InvalidErrors, second.InvalidErrors, third.InvalidErrors, fourth.InvalidErrors, fifth.InvalidErrors, sixth.InvalidErrors, seventh.InvalidErrors);
    }

    /// <summary>
    /// Combines 8 independent validations: <paramref name="combine"/> receives every value when all of them are valid;
    /// otherwise the result holds the errors of every invalid one, in argument order.
    /// </summary>
    /// <remarks>
    /// Every validation is read, none is skipped: this is how independent checks report every problem at once.
    /// <paramref name="combine"/> runs only when all of them are valid, exactly once.
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
    /// <param name="first">The first validation.</param>
    /// <param name="second">The second validation.</param>
    /// <param name="third">The third validation.</param>
    /// <param name="fourth">The fourth validation.</param>
    /// <param name="fifth">The fifth validation.</param>
    /// <param name="sixth">The sixth validation.</param>
    /// <param name="seventh">The seventh validation.</param>
    /// <param name="eighth">The eighth validation.</param>
    /// <param name="combine">Combines the values.</param>
    /// <returns>A valid validation with the combined value, or every error in argument order.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="combine"/> is null.</exception>
    /// <exception cref="ArgumentException">One of <paramref name="first"/>, <paramref name="second"/>, <paramref name="third"/>, <paramref name="fourth"/>, <paramref name="fifth"/>, <paramref name="sixth"/>, <paramref name="seventh"/>, <paramref name="eighth"/> is uninitialized (default).</exception>
    /// <exception cref="InvalidOperationException"><paramref name="combine"/> returned <see langword="null"/>.</exception>
    public static Validated<TResult, TError> Combine<T1, T2, T3, T4, T5, T6, T7, T8, TResult, TError>(
        Validated<T1, TError> first,
        Validated<T2, TError> second,
        Validated<T3, TError> third,
        Validated<T4, TError> fourth,
        Validated<T5, TError> fifth,
        Validated<T6, TError> sixth,
        Validated<T7, TError> seventh,
        Validated<T8, TError> eighth,
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
        Validated<T1, TError>.RejectUninitialized(first, nameof(first));
        Validated<T2, TError>.RejectUninitialized(second, nameof(second));
        Validated<T3, TError>.RejectUninitialized(third, nameof(third));
        Validated<T4, TError>.RejectUninitialized(fourth, nameof(fourth));
        Validated<T5, TError>.RejectUninitialized(fifth, nameof(fifth));
        Validated<T6, TError>.RejectUninitialized(sixth, nameof(sixth));
        Validated<T7, TError>.RejectUninitialized(seventh, nameof(seventh));
        Validated<T8, TError>.RejectUninitialized(eighth, nameof(eighth));
        ArgumentNullException.ThrowIfNull(combine);
        return first.TryGetValid(out var firstValue)
            && second.TryGetValid(out var secondValue)
            && third.TryGetValid(out var thirdValue)
            && fourth.TryGetValid(out var fourthValue)
            && fifth.TryGetValid(out var fifthValue)
            && sixth.TryGetValid(out var sixthValue)
            && seventh.TryGetValid(out var seventhValue)
            && eighth.TryGetValid(out var eighthValue)
            ? Validated<TResult, TError>.FromValue(Validated<TResult, TError>.Produced(combine(firstValue, secondValue, thirdValue, fourthValue, fifthValue, sixthValue, seventhValue, eighthValue), nameof(combine)))
            : Validated<TResult, TError>.FromErrors(first.InvalidErrors, second.InvalidErrors, third.InvalidErrors, fourth.InvalidErrors, fifth.InvalidErrors, sixth.InvalidErrors, seventh.InvalidErrors, eighth.InvalidErrors);
    }
}
