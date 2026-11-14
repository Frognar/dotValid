using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DotValid;

/// <summary>
/// The outcome of a validation: either a valid value of type <typeparamref name="T"/> (<see cref="Valid{T}"/>) or one
/// or more errors of type <typeparamref name="TError"/> (<see cref="Invalid{TError}"/>).
/// </summary>
/// <typeparam name="T">The type of the value. Never <see langword="null"/>.</typeparam>
/// <typeparam name="TError">The type of the errors. Never <see langword="null"/>.</typeparam>
/// <remarks>
/// <para>
/// <see cref="Validated{T, TError}"/> is a C# union with exactly two cases, <see cref="Valid{T}"/> and
/// <see cref="Invalid{TError}"/>. Both convert implicitly to <see cref="Validated{T, TError}"/>, and a <c>switch</c> that
/// handles both cases is exhaustive. Unlike a result, independent validations combine by accumulating every error.
/// </para>
/// <para>
/// <c>default(Validated&lt;T, TError&gt;)</c> is neither case: it is an uninitialized validation. Every operation that
/// reads it throws <see cref="InvalidOperationException"/>, and a <c>switch</c> over it throws
/// <see cref="SwitchExpressionException"/>.
/// </para>
/// </remarks>
[Union]
[DebuggerDisplay("{ToString(),nq}")]
public readonly partial struct Validated<T, TError> : Validated<T, TError>.IUnionMembers, IEquatable<Validated<T, TError>>
    where T : notnull
    where TError : notnull
{
    private readonly Valid<T> _valid;
    private readonly Invalid<TError> _invalid;
    private readonly ValidatedCase _case;

    private Validated(Valid<T> valid)
    {
        _valid = valid;
        _invalid = default;
        _case = ValidatedCase.Valid;
    }

    private Validated(Invalid<TError> invalid)
    {
        _valid = default;
        _invalid = invalid;
        _case = ValidatedCase.Invalid;
    }

    /// <summary>
    /// The union members the C# compiler uses for union conversions and pattern matching.
    /// </summary>
    /// <remarks>
    /// <see cref="Validated{T, TError}"/> implements the instance members explicitly, so they stay out of its public
    /// API. Use pattern matching instead of calling them.
    /// </remarks>
    public interface IUnionMembers
    {
        /// <summary>
        /// Creates a valid <see cref="Validated{T, TError}"/>.
        /// </summary>
        /// <param name="value">The valid case.</param>
        /// <returns>The <see cref="Valid{T}"/> case.</returns>
        /// <exception cref="ArgumentException"><paramref name="value"/> is a default <see cref="Valid{T}"/>.</exception>
        static Validated<T, TError> Create(Valid<T> value) =>
            value.Value is null
                ? throw new ArgumentException("A default Valid<T> holds no value; create it with valid(value).", nameof(value))
                : new Validated<T, TError>(value);

        /// <summary>
        /// Creates an invalid <see cref="Validated{T, TError}"/>.
        /// </summary>
        /// <param name="value">The invalid case.</param>
        /// <returns>The <see cref="Invalid{TError}"/> case.</returns>
        /// <exception cref="ArgumentException"><paramref name="value"/> is a default <see cref="Invalid{TError}"/>.</exception>
        static Validated<T, TError> Create(Invalid<TError> value) =>
            value.Errors.IsDefault
                ? throw new ArgumentException(
                    "A default Invalid<TError> holds no errors; create it with invalid(error).",
                    nameof(value))
                : new Validated<T, TError>(value);

        /// <summary>
        /// Gets the current case, boxed: a <see cref="Valid{T}"/> or an <see cref="Invalid{TError}"/>, or
        /// <see langword="null"/> for an uninitialized (default) validation.
        /// </summary>
        object? Value { get; }

        /// <summary>
        /// Gets the valid case when this validation is valid.
        /// </summary>
        /// <param name="value">The valid case when the method returns <see langword="true"/>; otherwise the default.</param>
        /// <returns><see langword="true"/> for the <see cref="Valid{T}"/> case; otherwise <see langword="false"/>.</returns>
        bool TryGetValue(out Valid<T> value);

        /// <summary>
        /// Gets the invalid case when this validation is invalid.
        /// </summary>
        /// <param name="value">The invalid case when the method returns <see langword="true"/>; otherwise the default.</param>
        /// <returns><see langword="true"/> for the <see cref="Invalid{TError}"/> case; otherwise <see langword="false"/>.</returns>
        bool TryGetValue(out Invalid<TError> value);
    }

    object? IUnionMembers.Value =>
        _case switch
        {
            ValidatedCase.Valid => _valid,
            ValidatedCase.Invalid => _invalid,
            _ => null,
        };

    /// <summary>
    /// Reduces this validation to a single value: <paramref name="valid"/> runs on the value of a valid validation,
    /// <paramref name="invalid"/> runs on the errors of an invalid one. Exactly one of them runs, exactly once.
    /// </summary>
    /// <remarks>
    /// The expression form of <c>validated switch { Valid&lt;T&gt;(var value) => ..., Invalid&lt;TError&gt;(var errors) => ... }</c>,
    /// convenient in lambdas and chains.
    /// </remarks>
    /// <typeparam name="TResult">The type of the value both branches produce.</typeparam>
    /// <param name="valid">Produces the value from the value of a valid validation.</param>
    /// <param name="invalid">Produces the value from the errors, in order (never empty).</param>
    /// <returns>The value produced by the branch that ran.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="valid"/> or <paramref name="invalid"/> is null.</exception>
    /// <exception cref="InvalidOperationException">The validation is uninitialized (default).</exception>
    public TResult Match<TResult>(Func<T, TResult> valid, Func<ImmutableArray<TError>, TResult> invalid)
    {
        ArgumentNullException.ThrowIfNull(valid);
        ArgumentNullException.ThrowIfNull(invalid);
        return _case switch
        {
            ValidatedCase.Valid => valid(_valid.Value),
            ValidatedCase.Invalid => invalid(_invalid.Errors),
            _ => throw Uninitialized(),
        };
    }

    /// <summary>
    /// Returns the value of a valid validation, or <paramref name="fallback"/> for an invalid one.
    /// </summary>
    /// <param name="fallback">The value to use for an invalid validation.</param>
    /// <returns>The value or the fallback.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fallback"/> is null.</exception>
    /// <exception cref="InvalidOperationException">The validation is uninitialized (default).</exception>
    public T OrDefault(T fallback)
    {
        ArgumentNullException.ThrowIfNull(fallback);
        return _case switch
        {
            ValidatedCase.Valid => _valid.Value,
            ValidatedCase.Invalid => fallback,
            _ => throw Uninitialized(),
        };
    }

    /// <summary>
    /// Returns the value of a valid validation, or the value <paramref name="fallback"/> produces from the errors.
    /// <paramref name="fallback"/> runs only for an invalid validation.
    /// </summary>
    /// <param name="fallback">Produces the value to use from the errors.</param>
    /// <returns>The value or the produced fallback.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="fallback"/> is null.</exception>
    /// <exception cref="InvalidOperationException">The validation is uninitialized (default).</exception>
    public T OrDefault(Func<ImmutableArray<TError>, T> fallback)
    {
        ArgumentNullException.ThrowIfNull(fallback);
        return _case switch
        {
            ValidatedCase.Valid => _valid.Value,
            ValidatedCase.Invalid => fallback(_invalid.Errors),
            _ => throw Uninitialized(),
        };
    }

    /// <summary>
    /// Transforms the value of a valid validation with <paramref name="map"/>; the errors of an invalid one pass through
    /// unchanged.
    /// </summary>
    /// <remarks><paramref name="map"/> runs only for a valid validation, exactly once.</remarks>
    /// <typeparam name="TResult">The type of the transformed value.</typeparam>
    /// <param name="map">Transforms the value.</param>
    /// <returns>A valid validation with the transformed value, or the same errors.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="map"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// The validation is uninitialized (default), or <paramref name="map"/> returned <see langword="null"/>.
    /// </exception>
    public Validated<TResult, TError> Map<TResult>(Func<T, TResult> map)
        where TResult : notnull
    {
        ArgumentNullException.ThrowIfNull(map);
        return _case switch
        {
            ValidatedCase.Valid => new Validated<TResult, TError>(new Valid<TResult>(Produced(map(_valid.Value), nameof(map)))),
            ValidatedCase.Invalid => new Validated<TResult, TError>(_invalid),
            _ => throw Uninitialized(),
        };
    }

    /// <summary>
    /// Transforms every error with <paramref name="map"/>, possibly into another type, keeping their order; the value of
    /// a valid validation passes through unchanged.
    /// </summary>
    /// <remarks>
    /// <paramref name="map"/> runs once per error and never for a valid validation. Use it to translate errors between
    /// layers, for example from field codes to messages.
    /// </remarks>
    /// <typeparam name="TNewError">The type of the transformed errors.</typeparam>
    /// <param name="map">Transforms one error.</param>
    /// <returns>The same valid validation, or an invalid one with every error transformed.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="map"/> is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// The validation is uninitialized (default), or <paramref name="map"/> returned <see langword="null"/>.
    /// </exception>
    public Validated<T, TNewError> MapError<TNewError>(Func<TError, TNewError> map)
        where TNewError : notnull
    {
        ArgumentNullException.ThrowIfNull(map);
        return _case switch
        {
            ValidatedCase.Valid => new Validated<T, TNewError>(_valid),
            ValidatedCase.Invalid => new Validated<T, TNewError>(Invalid<TNewError>.FromTrusted(MapErrors(_invalid.Errors, map))),
            _ => throw Uninitialized(),
        };

        static ImmutableArray<TNewError> MapErrors(ImmutableArray<TError> errors, Func<TError, TNewError> map)
        {
            var mapped = ImmutableArray.CreateBuilder<TNewError>(errors.Length);
            foreach (var error in errors)
            {
                mapped.Add(Produced(map(error), nameof(map)));
            }

            return mapped.MoveToImmutable();
        }
    }

    /// <summary>
    /// Compares two validations: both valid with equal values, both invalid with equal errors, or both uninitialized.
    /// </summary>
    /// <param name="left">The first validation.</param>
    /// <param name="right">The second validation.</param>
    /// <returns><see langword="true"/> when the validations are equal; otherwise <see langword="false"/>.</returns>
    public static bool operator ==(Validated<T, TError> left, Validated<T, TError> right) => left.Equals(right);

    /// <summary>
    /// Compares two validations for inequality. Always the opposite of <c>==</c>.
    /// </summary>
    /// <param name="left">The first validation.</param>
    /// <param name="right">The second validation.</param>
    /// <returns><see langword="true"/> when the validations differ; otherwise <see langword="false"/>.</returns>
    public static bool operator !=(Validated<T, TError> left, Validated<T, TError> right) => !left.Equals(right);

    bool IUnionMembers.TryGetValue(out Valid<T> value)
    {
        value = _valid;
        return _case == ValidatedCase.Valid;
    }

    bool IUnionMembers.TryGetValue(out Invalid<TError> value)
    {
        value = _invalid;
        return _case == ValidatedCase.Invalid;
    }

    /// <summary>
    /// Determines whether this validation equals <paramref name="other"/>: both valid with equal values, both invalid
    /// with equal errors in the same order, or both uninitialized. A valid validation never equals an invalid one.
    /// </summary>
    /// <param name="other">The validation to compare with.</param>
    /// <returns><see langword="true"/> when the validations are equal; otherwise <see langword="false"/>.</returns>
    public bool Equals(Validated<T, TError> other) =>
        _case == other._case
        && _case switch
        {
            ValidatedCase.Valid => _valid.Equals(other._valid),
            ValidatedCase.Invalid => _invalid.Equals(other._invalid),
            _ => true,
        };

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Validated<T, TError> validated && Equals(validated);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        _case switch
        {
            ValidatedCase.Valid => HashCode.Combine(ValidatedCase.Valid, _valid),
            ValidatedCase.Invalid => HashCode.Combine(ValidatedCase.Invalid, _invalid),
            _ => 0,
        };

    // V4: a delegate never smuggles a null into a validation.
    internal static TValue Produced<TValue>(TValue value, string producer) =>
        value is null
            ? throw new InvalidOperationException($"'{producer}' returned null, which a validation cannot hold.")
            : value;

    // V3: every consuming member rejects the uninitialized default the same way.
    private static InvalidOperationException Uninitialized() =>
        new("The validation is uninitialized (default). Create validations with valid(value) or invalid(error).");

    /// <summary>
    /// Returns <c>Valid(value)</c> for a valid validation, <c>Invalid([error, ...])</c> for an invalid one, and
    /// <c>Uninitialized</c> for <c>default</c>.
    /// </summary>
    /// <returns>A text representation of this validation.</returns>
    public override string ToString() =>
        _case switch
        {
            ValidatedCase.Valid => _valid.ToString(),
            ValidatedCase.Invalid => _invalid.ToString(),
            _ => "Uninitialized",
        };
}
