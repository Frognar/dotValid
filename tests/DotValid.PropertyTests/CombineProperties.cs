using FsCheck;
using FsCheck.Xunit;

namespace DotValid.PropertyTests;

/// <summary>
/// <c>Validated.Combine</c> reads two to eight independent validations and accumulates (V7): the combiner gets every
/// value when all are valid; otherwise every error of every invalid input comes out, in argument order.
/// </summary>
public sealed class CombineProperties
{
    [Property]
    public void Combining_two_is_Zip_with_a_combiner(Validated<int, string> first, Validated<bool, string> second, Func<int, bool, string> combine) =>
        Assert.Equal(first.Zip(second, combine), Validated.Combine(first, second, combine));

    [Property]
    public void Combining_three_is_nested_Zip_then_Map(Validated<int, string> first, Validated<bool, string> second, Validated<long, string> third) =>
        Assert.Equal(
            first.Zip(second).Zip(third).Map(all => $"{all.Item1.Item1} {all.Item1.Item2} {all.Item2}"),
            Validated.Combine(first, second, third, (a, b, c) => $"{a} {b} {c}"));

    [Property]
    public void Combining_eight_gives_every_error_in_argument_order(
        Validated<int, string> v1,
        Validated<int, string> v2,
        Validated<int, string> v3,
        Validated<int, string> v4,
        Validated<int, string> v5,
        Validated<int, string> v6,
        Validated<int, string> v7,
        Validated<int, string> v8)
    {
        Validated<int, string>[] all = [v1, v2, v3, v4, v5, v6, v7, v8];
        var expected = all.SelectMany(ZipProperties.Errors).ToArray();

        var combined = Validated.Combine(v1, v2, v3, v4, v5, v6, v7, v8, (a, b, c, d, e, f, g, h) => a + b + c + d + e + f + g + h);

        Assert.Equal(expected, ZipProperties.Errors(combined));
        if (expected.Length == 0)
        {
            ValidatedAssert.IsValid(all.Sum(validated => validated.OrDefault(0)), combined);
        }
    }

    [Property]
    public void Every_arity_gives_every_error_of_its_inputs_in_order(
        Validated<int, string> v1,
        Validated<int, string> v2,
        Validated<int, string> v3,
        Validated<int, string> v4,
        Validated<int, string> v5,
        Validated<int, string> v6,
        Validated<int, string> v7,
        Validated<int, string> v8)
    {
        Validated<int, string>[] all = [v1, v2, v3, v4, v5, v6, v7, v8];
        for (var arity = 2; arity <= 8; arity++)
        {
            var inputs = all[..arity];
            var expected = inputs.SelectMany(ZipProperties.Errors).ToArray();

            var combined = CombineAll(inputs, values => values.Sum());

            Assert.Equal(expected, ZipProperties.Errors(combined));
            Assert.Equal(expected.Length == 0, combined is Valid<int>);
        }
    }

    [Property]
    public void Every_arity_combines_valid_values_once(int value)
    {
        var calls = 0;
        var v = Validations.Valid<int, string>(value);

        _ = Validated.Combine(v, v, (a, b) => ++calls);
        _ = Validated.Combine(v, v, v, (a, b, c) => ++calls);
        _ = Validated.Combine(v, v, v, v, (a, b, c, d) => ++calls);
        _ = Validated.Combine(v, v, v, v, v, (a, b, c, d, e) => ++calls);
        _ = Validated.Combine(v, v, v, v, v, v, (a, b, c, d, e, f) => ++calls);
        _ = Validated.Combine(v, v, v, v, v, v, v, (a, b, c, d, e, f, g) => ++calls);
        _ = Validated.Combine(v, v, v, v, v, v, v, v, (a, b, c, d, e, f, g, h) => ++calls);

        Assert.Equal(7, calls);
        ValidatedAssert.IsValid(value * 8, Validated.Combine(v, v, v, v, v, v, v, v, (a, b, c, d, e, f, g, h) => a + b + c + d + e + f + g + h));
    }

    [Property]
    public void The_combiner_never_runs_when_one_input_is_invalid(NonNull<string> error, int value)
    {
        var calls = 0;
        var good = Validations.Valid<int, string>(value);
        var bad = Validations.Invalid<int, string>(error.Get);

        ValidatedAssert.IsInvalid([error.Get], Validated.Combine(good, good, good, bad, (a, b, c, d) => ++calls));
        Assert.Equal(0, calls);
    }

    [Property]
    public void A_null_from_the_combiner_is_rejected(int value) =>
        Assert.Throws<InvalidOperationException>(
            () => Validated.Combine(Validations.Valid<int, string>(value), Validations.Valid<int, string>(value), (_, _) => (string)null!));

    [Fact]
    public void Every_arity_rejects_an_uninitialized_argument_by_its_name_and_a_missing_combiner()
    {
        string[] names = ["first", "second", "third", "fourth", "fifth", "sixth", "seventh", "eighth"];
        var calls = 0;
        for (var arity = 2; arity <= 8; arity++)
        {
            for (var position = 0; position < arity; position++)
            {
                var inputs = Enumerable.Repeat(Validations.Valid<int, string>(1), arity).ToArray();
                inputs[position] = default;

                var rejected = Assert.Throws<ArgumentException>(() => CombineAll(inputs, _ => ++calls));

                Assert.Equal(names[position], rejected.ParamName);
            }

            var valid = Enumerable.Repeat(Validations.Valid<int, string>(1), arity).ToArray();
            Assert.Equal("combine", Assert.Throws<ArgumentNullException>(() => CombineAll(valid, null)).ParamName);
        }

        Assert.Equal(0, calls);
    }

    // Calls the Combine overload for inputs.Length (2..8); a null combine passes null to that overload.
    private static Validated<int, string> CombineAll(Validated<int, string>[] v, Func<int[], int>? combine) =>
        v.Length switch
        {
            2 => Validated.Combine(v[0], v[1], combine is null ? null! : (Func<int, int, int>)((a, b) => combine([a, b]))),
            3 => Validated.Combine(v[0], v[1], v[2], combine is null ? null! : (Func<int, int, int, int>)((a, b, c) => combine([a, b, c]))),
            4 => Validated.Combine(v[0], v[1], v[2], v[3], combine is null ? null! : (Func<int, int, int, int, int>)((a, b, c, d) => combine([a, b, c, d]))),
            5 => Validated.Combine(
                v[0], v[1], v[2], v[3], v[4], combine is null ? null! : (Func<int, int, int, int, int, int>)((a, b, c, d, e) => combine([a, b, c, d, e]))),
            6 => Validated.Combine(
                v[0], v[1], v[2], v[3], v[4], v[5], combine is null ? null! : (Func<int, int, int, int, int, int, int>)((a, b, c, d, e, f) => combine([a, b, c, d, e, f]))),
            7 => Validated.Combine(
                v[0], v[1], v[2], v[3], v[4], v[5], v[6],
                combine is null ? null! : (Func<int, int, int, int, int, int, int, int>)((a, b, c, d, e, f, g) => combine([a, b, c, d, e, f, g]))),
            8 => Validated.Combine(
                v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7],
                combine is null ? null! : (Func<int, int, int, int, int, int, int, int, int>)((a, b, c, d, e, f, g, h) => combine([a, b, c, d, e, f, g, h]))),
            _ => throw new ArgumentOutOfRangeException(nameof(v)),
        };
}
