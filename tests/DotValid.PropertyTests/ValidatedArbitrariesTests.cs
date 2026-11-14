using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;

namespace DotValid.PropertyTests;

/// <summary>
/// The generators behind every property: both cases, never the uninitialized default, never null, one to five errors,
/// and shrinking that keeps the case.
/// </summary>
public sealed class ValidatedArbitrariesTests
{
    [Fact]
    public void Generates_both_cases()
    {
        var samples = Gen.Sample(Numbers().Generator, 200);

        Assert.Contains(samples, validated => validated is Valid<int>);
        Assert.Contains(samples, validated => validated is Invalid<int>);
    }

    [Fact]
    public void Never_generates_an_uninitialized_validation() =>
        Assert.DoesNotContain(Gen.Sample(Numbers().Generator, 200), validated => validated is null);

    [Fact]
    public void Generates_one_to_five_errors_and_every_count_in_between()
    {
        var counts = Gen.Sample(Numbers().Generator, 500)
            .Select(validated => validated is Invalid<int>(var errors) ? errors.Length : 0)
            .Where(count => count > 0)
            .Distinct()
            .Order();

        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, counts);
    }

    [Fact]
    public void Generates_reference_contents_without_nulls()
    {
        var texts = ValidatedArbitraries.ValidatedOf(ArbMap.Default.ArbFor<string>(), ArbMap.Default.ArbFor<string>());
        var samples = Gen.Sample(texts.Generator, 500);

        Assert.All(
            samples,
            validated => Assert.True(validated is Valid<string>(not null) || (validated is Invalid<string>(var errors) && errors.All(error => error is not null))));
    }

    [Property]
    public void A_valid_validation_shrinks_to_valid_ones_with_smaller_values(int value)
    {
        var expected = ArbMap.Default.ArbFor<int>()
            .Shrinker(value)
            .Select(smaller => Validations.Valid<int, int>(smaller));

        Assert.Equal(expected, Numbers().Shrinker(Validations.Valid<int, int>(value)));
    }

    [Property]
    public void An_invalid_validation_shrinks_to_invalid_ones_with_fewer_or_smaller_errors(NonEmptyArray<int> errors)
    {
        var shrunk = Numbers().Shrinker(Validations.Invalid<int, int>(errors.Get)).ToArray();

        Assert.All(shrunk, smaller => Assert.True(smaller is Invalid<int>(var fewer) && fewer.Length <= errors.Get.Length));
        if (errors.Get.Length > 1)
        {
            Assert.Contains(Validations.Invalid<int, int>(errors.Get[1..]), shrunk);
        }
    }

    private static Arbitrary<Validated<int, int>> Numbers() =>
        ValidatedArbitraries.ValidatedOf(ArbMap.Default.ArbFor<int>(), ArbMap.Default.ArbFor<int>());
}
