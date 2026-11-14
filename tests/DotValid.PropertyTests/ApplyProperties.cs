using FsCheck;
using FsCheck.Xunit;

namespace DotValid.PropertyTests;

/// <summary>
/// <c>Apply</c> is the applicative core (V7): the four applicative laws hold, and the errors of the function and of the
/// argument accumulate, the function's first.
/// </summary>
public sealed class ApplyProperties
{
    [Property]
    public void Identity_applying_the_identity_changes_nothing(Validated<int, string> validated) =>
        Assert.Equal(validated, Validations.Valid<Func<int, int>, string>(value => value).Apply(validated));

    [Property]
    public void Homomorphism_applying_a_valid_function_to_a_valid_value_is_a_valid_result(Func<int, string> function, int value) =>
        Assert.Equal(
            Validations.Valid<string, string>(function(value)),
            Validations.Valid<Func<int, string>, string>(function).Apply(Validations.Valid<int, string>(value)));

    [Property]
    public void Interchange_the_function_and_a_valid_argument_can_swap_sides(Validated<Func<int, string>, string> function, int value) =>
        Assert.Equal(
            function.Apply(Validations.Valid<int, string>(value)),
            Validations.Valid<Func<Func<int, string>, string>, string>(f => f(value)).Apply(function));

    [Property]
    public void Composition_applying_a_composition_is_applying_twice(
        Validated<Func<long, string>, string> outer,
        Validated<Func<int, long>, string> inner,
        Validated<int, string> argument)
    {
        Func<Func<long, string>, Func<Func<int, long>, Func<int, string>>> compose = f => g => x => f(g(x));

        Assert.Equal(
            Validations.Valid<Func<Func<long, string>, Func<Func<int, long>, Func<int, string>>>, string>(compose)
                .Apply(outer)
                .Apply(inner)
                .Apply(argument),
            outer.Apply(inner.Apply(argument)));
    }

    [Property]
    public void The_errors_of_the_function_and_the_argument_accumulate(NonEmptyArray<int> functionErrors, NonEmptyArray<int> argumentErrors) =>
        ValidatedAssert.IsInvalid(
            functionErrors.Get.Concat(argumentErrors.Get),
            Validations.Invalid<Func<string, bool>, int>(functionErrors.Get).Apply(Validations.Invalid<string, int>(argumentErrors.Get)));

    [Property]
    public void Apply_is_Zip_then_calling_the_function(Validated<Func<int, string>, string> function, Validated<int, string> argument) =>
        Assert.Equal(function.Zip(argument, (f, value) => f(value)), function.Apply(argument));

    [Fact]
    public void A_curried_constructor_collects_every_error()
    {
        Func<string, Func<int, string>> person = name => age => $"{name} ({age})";

        var built = Validations.Valid<Func<string, Func<int, string>>, string>(person)
            .Apply(Validations.Invalid<string, string>("name is empty"))
            .Apply(Validations.Invalid<int, string>("age is negative"));

        ValidatedAssert.IsInvalid(["name is empty", "age is negative"], built);
    }

    [Property]
    public void A_null_from_the_function_is_rejected_and_named(int value)
    {
        var exception = Assert.Throws<InvalidOperationException>(
            () => Validations.Valid<Func<int, string>, string>(_ => null!).Apply(Validations.Valid<int, string>(value)));

        Assert.StartsWith("'function' returned null", exception.Message, StringComparison.Ordinal);
    }

    [Property]
    public void An_uninitialized_argument_is_rejected_whatever_the_function(Validated<Func<int, string>, string> function) =>
        Assert.Equal(
            "argument",
            Assert.Throws<ArgumentException>(() => function.Apply(default(Validated<int, string>))).ParamName);
}
