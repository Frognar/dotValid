using System;
using System.Collections.Generic;

namespace DotValid;

/// <summary>
/// Represents the result of a validation process, which can either be a success containing
/// a valid value or a failure containing a collection of validation errors.
/// </summary>
/// <typeparam name="T">The type of the value being validated.</typeparam>
public readonly partial record struct Validated<T>
{
    private readonly IValidated _validated;

    private Validated(IValidated validated)
    {
        _validated = validated;
    }

    /// <summary>
    /// Gets a value indicating whether the validation result represents a successful validation
    /// state without any errors.
    /// </summary>
    /// <value>
    /// Returns <see langword="true"/> if the validation result is successful; otherwise,
    /// returns <see langword="false"/>.
    /// </value>
    public bool IsValid => _validated is SuccessType;

    /// <summary>
    /// Executes one of two provided functions based on whether the current instance represents
    /// a success or a failure.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the provided functions.</typeparam>
    /// <param name="errors">A function to invoke when the instance represents a failure,
    /// receiving the collection of validation errors as an argument.</param>
    /// <param name="value">A function to invoke when the instance represents a success,
    /// receiving the successful value as an argument.</param>
    /// <returns>The result produced by invoking the appropriate function.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the internal state is invalid or unrecognized.</exception>
    public TResult Match<TResult>(
        Func<IEnumerable<ValidationError>, TResult> errors,
        Func<T, TResult> value)
    {
        return _validated switch
        {
            FailureType f => errors(f.Errors),
            SuccessType s => value(s.Value),
            _ => throw new InvalidOperationException(
                "Impossible state: This part of the code should never be reached."),
        };
    }

    /// <summary>
    /// Creates a validated instance that represents a successful validation result
    /// containing the specified value.
    /// </summary>
    /// <param name="value">The value representing the successful validation result.</param>
    /// <returns>A validated instance containing the successful value.</returns>
    internal static Validated<T> Succeed(T value) => new(new SuccessType(value));

    /// <summary>
    /// Creates a validated instance that represents a failure
    /// containing the specified collection of validation errors.
    /// </summary>
    /// <param name="errors">The collection of validation errors associated with the failure.</param>
    /// <returns>A validated instance containing the failure and its associated validation errors.</returns>
    internal static Validated<T> Failed(IEnumerable<ValidationError> errors) => new(new FailureType([..errors]));
}
