using System.Collections.Generic;

namespace DotValid;

/// <summary>
/// Provides helper methods for creating instances of <see cref="Validated{T}"/>,
/// simplifying the construction of successful or failed validation results.
/// </summary>
public static class Validation
{
    /// <summary>
    /// Creates a validated instance that represents a successful validation result
    /// containing the specified value.
    /// </summary>
    /// <typeparam name="T">The type of the value representing a successful validation result.</typeparam>
    /// <param name="value">The value representing the successful validation result.</param>
    /// <returns>A validated instance containing the successful value.</returns>
    public static Validated<T> Succeed<T>(T value) => Validated<T>.Succeed(value);

    /// <summary>
    /// Creates a validated instance that represents a failure
    /// containing the specified collection of validation errors.
    /// </summary>
    /// <typeparam name="T">The type of the value being validated.</typeparam>
    /// <param name="errors">The collection of validation errors associated with the failure.</param>
    /// <returns>A validated instance containing the failure and its associated validation errors.</returns>
    public static Validated<T> Failed<T>(IEnumerable<ValidationError> errors) => Validated<T>.Failed(errors);
}
