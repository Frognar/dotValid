namespace DotValid;

/// <summary>
/// A ready-made error type: which <paramref name="Property"/> failed and why (<paramref name="Message"/>). Any type can be
/// the error of a <see cref="Validated{T, TError}"/>; this one is there for the common case.
/// </summary>
/// <param name="Property">The name or path of the property that failed, for example <c>Address.City</c>.</param>
/// <param name="Message">Why it failed.</param>
/// <exception cref="ArgumentNullException"><paramref name="Property"/> or <paramref name="Message"/> is null.</exception>
public readonly record struct ValidationError(string Property, string Message)
{
    /// <summary>Gets the name or path of the property that failed.</summary>
    public string Property { get; } = Property ?? throw new ArgumentNullException(nameof(Property));

    /// <summary>Gets why the property failed.</summary>
    public string Message { get; } = Message ?? throw new ArgumentNullException(nameof(Message));

    /// <summary>Returns <c>Property: Message</c>.</summary>
    /// <returns>A text representation of this error.</returns>
    public override string ToString() => $"{Property}: {Message}";
}
