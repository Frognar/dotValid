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

    private Validated(ValidatedCase @case, Valid<T> valid, Invalid<TError> invalid) =>
        (_case, _valid, _invalid) = (@case, valid, invalid);

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
        static Validated<T, TError> Create(Valid<T> value) => throw new NotImplementedException();

        /// <summary>
        /// Creates an invalid <see cref="Validated{T, TError}"/>.
        /// </summary>
        /// <param name="value">The invalid case.</param>
        /// <returns>The <see cref="Invalid{TError}"/> case.</returns>
        /// <exception cref="ArgumentException"><paramref name="value"/> is a default <see cref="Invalid{TError}"/>.</exception>
        static Validated<T, TError> Create(Invalid<TError> value) => throw new NotImplementedException();

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

    object? IUnionMembers.Value => throw new NotImplementedException();

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

    bool IUnionMembers.TryGetValue(out Valid<T> value) => throw new NotImplementedException();

    bool IUnionMembers.TryGetValue(out Invalid<TError> value) => throw new NotImplementedException();

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
}
