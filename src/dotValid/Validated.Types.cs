using System.Collections.Generic;

namespace DotValid;

public readonly partial record struct Validated<T>
{
    /// <summary>
    /// Represents the base contract for a validated result, which can either be a success
    /// or a failure containing validation errors.
    /// </summary>
    private interface IValidated;

    private readonly record struct SuccessType(T Value) : IValidated
    {
        public T Value { get; } = Value;
    }

    private readonly record struct FailureType(IEnumerable<ValidationError> Errors) : IValidated
    {
        public IEnumerable<ValidationError> Errors { get; } = Errors;
    }
}
