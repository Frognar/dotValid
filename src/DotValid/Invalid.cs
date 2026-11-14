using System.Collections.Immutable;

namespace DotValid;

/// <summary>
/// The invalid case of a <see cref="Validated{T, TError}"/>: holds one or more errors, never none. Converts implicitly
/// to any <see cref="Validated{T, TError}"/> with the same <typeparamref name="TError"/>.
/// </summary>
/// <typeparam name="TError">The type of the errors. Never <see langword="null"/>.</typeparam>
public readonly struct Invalid<TError> : IEquatable<Invalid<TError>>
    where TError : notnull
{
    private readonly ImmutableArray<TError> _errors;

    /// <summary>
    /// Initializes the invalid case with a single <paramref name="error"/>.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
    public Invalid(TError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        _errors = [error];
    }

    /// <summary>
    /// Initializes the invalid case with <paramref name="errors"/>, in order.
    /// </summary>
    /// <param name="errors">The errors; at least one, none of them null.</param>
    /// <exception cref="ArgumentException"><paramref name="errors"/> is empty or holds a null error.</exception>
    public Invalid(params ReadOnlySpan<TError> errors)
    {
        if (errors.IsEmpty)
        {
            throw new ArgumentException("An invalid validation needs at least one error.", nameof(errors));
        }

        foreach (var error in errors)
        {
            if (error is null)
            {
                throw new ArgumentException("An error cannot be null.", nameof(errors));
            }
        }

        _errors = [.. errors];
    }

    /// <summary>
    /// Gets the errors, in order. Never empty.
    /// </summary>
    public ImmutableArray<TError> Errors => _errors;

    /// <summary>
    /// Compares two invalid cases by their errors, element by element.
    /// </summary>
    /// <param name="left">The first case.</param>
    /// <param name="right">The second case.</param>
    /// <returns><see langword="true"/> when the errors are equal; otherwise <see langword="false"/>.</returns>
    public static bool operator ==(Invalid<TError> left, Invalid<TError> right) => left.Equals(right);

    /// <summary>
    /// Compares two invalid cases for inequality. Always the opposite of <c>==</c>.
    /// </summary>
    /// <param name="left">The first case.</param>
    /// <param name="right">The second case.</param>
    /// <returns><see langword="true"/> when the errors differ; otherwise <see langword="false"/>.</returns>
    public static bool operator !=(Invalid<TError> left, Invalid<TError> right) => !left.Equals(right);

    /// <summary>
    /// Deconstructs the case, for positional patterns: <c>Invalid&lt;string&gt;(var errors)</c>.
    /// </summary>
    /// <param name="errors">The errors.</param>
    public void Deconstruct(out ImmutableArray<TError> errors) => errors = _errors;

    /// <summary>
    /// Determines whether this case holds the same errors as <paramref name="other"/>, in the same order, each compared
    /// with <see cref="EqualityComparer{T}.Default"/>.
    /// </summary>
    /// <param name="other">The case to compare with.</param>
    /// <returns><see langword="true"/> when the errors are equal; otherwise <see langword="false"/>.</returns>
    public bool Equals(Invalid<TError> other) =>
        // A default case reads as no errors, which a real case never has: it equals only another default case.
        _errors.AsSpan().SequenceEqual(other._errors.AsSpan(), EqualityComparer<TError>.Default);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Invalid<TError> other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        if (_errors.IsDefault)
        {
            return 0;
        }

        var hash = default(HashCode);
        foreach (var error in _errors)
        {
            hash.Add(error);
        }

        return hash.ToHashCode();
    }
}
