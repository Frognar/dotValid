namespace DotValid;

/// <summary>
/// Represents an error that occurred during validation of a property.
/// </summary>
public readonly record struct ValidationError(string PropertyName, string Error)
{
    /// <summary>
    /// Gets the name of the property that caused the validation error.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> representing the name of the property.
    /// </value>
    public string PropertyName { get; } = PropertyName;

    /// <summary>
    /// Gets the error message associated with the validation failure.
    /// </summary>
    /// <value>
    /// A string representing the error message that describes the validation issue.
    /// </value>
    public string Error { get; } = Error;
}
