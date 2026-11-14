using FsCheck;
using FsCheck.Fluent;

namespace DotValid.PropertyTests;

/// <summary>
/// FsCheck generators for DotValid types, registered for the whole assembly in <c>AssemblyInfo.cs</c>.
/// </summary>
public static class ValidatedArbitraries
{
    private const int MaxErrors = 5;

    /// <summary>
    /// <c>Validated&lt;T, TError&gt;</c> for any <c>T</c> and <c>TError</c> FsCheck can generate: valid and invalid in
    /// equal proportion; an invalid one holds one to five errors. FsCheck injects the arbitraries for <c>T</c> and
    /// <c>TError</c>, so nested validations work too.
    /// </summary>
    /// <remarks>
    /// <see langword="null"/> is never a value or an error (V4), so generated nulls are skipped. The uninitialized
    /// <c>default</c> is never generated: properties about it build it explicitly. A valid validation shrinks to smaller
    /// values; an invalid one to fewer errors, then to smaller errors.
    /// </remarks>
    public static Arbitrary<Validated<T, TError>> ValidatedOf<T, TError>(Arbitrary<T> values, Arbitrary<TError> errors)
        where T : notnull
        where TError : notnull
    {
        var nonNullErrors = errors.Generator.Where(error => error is not null);
        var validations = Gen.OneOf(
            values.Generator.Where(value => value is not null).Select(value => Validations.Valid<T, TError>(value)),
            Gen.Choose(1, MaxErrors)
                .SelectMany(count => nonNullErrors.ArrayOf(count))
                .Select(list => Validations.Invalid<T, TError>(list)));

        return Arb.From(validations, validated => Shrink(validated, values, errors));
    }

    private static IEnumerable<Validated<T, TError>> Shrink<T, TError>(
        Validated<T, TError> validated,
        Arbitrary<T> values,
        Arbitrary<TError> errors)
        where T : notnull
        where TError : notnull
    {
        if (validated is Valid<T>(var value))
        {
            return values.Shrinker(value)
                .Where(smaller => smaller is not null)
                .Select(smaller => Validations.Valid<T, TError>(smaller));
        }

        if (validated is Invalid<TError>(var list))
        {
            var fewer = list.Length > 1
                ? Enumerable.Range(0, list.Length).Select(skip => Validations.Invalid<T, TError>([.. list.RemoveAt(skip)]))
                : [];
            var smaller = errors.Shrinker(list[0])
                .Where(error => error is not null)
                .Select(error => Validations.Invalid<T, TError>([.. list.SetItem(0, error)]));
            return fewer.Concat(smaller);
        }

        return [];
    }
}
