using System.Diagnostics;

namespace DotValid;

/// <summary>
/// The valid case of a <see cref="Validated{T, TError}"/>: holds the value. Converts implicitly to any
/// <see cref="Validated{T, TError}"/> with the same <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the value. Never <see langword="null"/>.</typeparam>
[DebuggerDisplay("{ToString(),nq}")]
public readonly struct Valid<T> : IEquatable<Valid<T>>
    where T : notnull
{
    private readonly T _value;

    /// <summary>
    /// Initializes the valid case with <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
    public Valid(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        _value = value;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public T Value => _value;

    /// <summary>
    /// Compares two valid cases by their values.
    /// </summary>
    /// <param name="left">The first case.</param>
    /// <param name="right">The second case.</param>
    /// <returns><see langword="true"/> when the values are equal; otherwise <see langword="false"/>.</returns>
    public static bool operator ==(Valid<T> left, Valid<T> right) => left.Equals(right);

    /// <summary>
    /// Compares two valid cases for inequality. Always the opposite of <c>==</c>.
    /// </summary>
    /// <param name="left">The first case.</param>
    /// <param name="right">The second case.</param>
    /// <returns><see langword="true"/> when the values differ; otherwise <see langword="false"/>.</returns>
    public static bool operator !=(Valid<T> left, Valid<T> right) => !left.Equals(right);

    /// <summary>
    /// Deconstructs the case, for positional patterns: <c>Valid&lt;int&gt;(var value)</c>.
    /// </summary>
    /// <param name="value">The value.</param>
    public void Deconstruct(out T value) => value = _value;

    /// <summary>
    /// Determines whether this case holds a value equal to the value of <paramref name="other"/>, according to
    /// <see cref="EqualityComparer{T}.Default"/>.
    /// </summary>
    /// <param name="other">The case to compare with.</param>
    /// <returns><see langword="true"/> when the values are equal; otherwise <see langword="false"/>.</returns>
    public bool Equals(Valid<T> other) => EqualityComparer<T>.Default.Equals(_value, other._value);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is Valid<T> other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(_value);

    /// <summary>
    /// Returns <c>Valid(value)</c>.
    /// </summary>
    /// <returns>A text representation of this case.</returns>
    public override string ToString() => $"Valid({_value})";
}
