namespace DotValid;

/// <summary>
/// Which case a <see cref="Validated{T, TError}"/> is in. The zero value is the uninitialized <c>default</c> validation.
/// </summary>
internal enum ValidatedCase : byte
{
    Uninitialized = 0,
    Valid = 1,
    Invalid = 2,
}
