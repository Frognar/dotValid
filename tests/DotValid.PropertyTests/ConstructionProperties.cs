using FsCheck;
using FsCheck.Xunit;
using static DotValid.Prelude;

namespace DotValid.PropertyTests;

/// <summary>
/// The two ways in, <c>valid</c> and <c>invalid</c>, produce the case they name and keep its content as it is: the
/// value, or every error in order (V1, V2).
/// </summary>
public sealed class ConstructionProperties
{
    [Property]
    public void Valid_holds_its_value(int value) =>
        ValidatedAssert.IsValid(value, Validations.Valid<int, string>(value));

    [Property]
    public void Invalid_holds_its_errors_in_order(NonEmptyArray<NonNull<string>> errors)
    {
        var expected = errors.Get.Select(error => error.Get).ToArray();

        ValidatedAssert.IsInvalid(expected, Validations.Invalid<int, string>(expected));
    }

    [Property]
    public void Valid_and_invalid_stay_apart_when_value_and_error_have_the_same_type(int content)
    {
        ValidatedAssert.IsValid(content, Validations.Valid<int, int>(content));
        ValidatedAssert.IsInvalid([content], Validations.Invalid<int, int>(content));
    }

    [Property]
    public void A_reference_value_and_reference_errors_are_kept_as_they_are(NonNull<string> value, NonNull<string> error)
    {
        var validated = Validations.Valid<string, int>(value.Get);
        var invalidated = Validations.Invalid<int, string>(error.Get);

        Assert.True(validated is Valid<string>(var kept) && ReferenceEquals(value.Get, kept));
        Assert.True(invalidated is Invalid<string>(var errors) && ReferenceEquals(error.Get, errors[0]));
    }

    [Property]
    public void The_prelude_and_the_case_constructors_agree(int value, NonNull<string> first, NonNull<string> second)
    {
        Assert.Equal(new Valid<int>(value), valid(value));
        Assert.Equal(new Invalid<string>(first.Get), invalid(first.Get));
        Assert.Equal(new Invalid<string>(first.Get, second.Get), invalid(first.Get, second.Get));
        Assert.Equal(invalid(first.Get), invalid([first.Get]));
    }

    [Property]
    public void A_conversion_and_the_union_member_Create_agree(int value, NonNull<string> failure)
    {
        Validated<int, string> converted = valid(value);
        Validated<int, string> failed = invalid(failure.Get);

        Assert.Equal(converted, Validated<int, string>.IUnionMembers.Create(valid(value)));
        Assert.Equal(failed, Validated<int, string>.IUnionMembers.Create(invalid(failure.Get)));
    }

    [Fact]
    public void A_default_valid_case_of_a_value_type_is_a_valid_case()
    {
        Validated<int, string> validated = default(Valid<int>);

        ValidatedAssert.IsValid(0, validated);
    }

    [Property]
    public void Positional_and_type_patterns_see_the_same_case(Validated<int, string> validated)
    {
        var positional = validated switch
        {
            Valid<int>(var value) => $"valid {value}",
            Invalid<string>(var errors) => $"invalid {string.Join(",", errors)}",
        };
        var typed = validated switch
        {
            Valid<int> success => $"valid {success.Value}",
            Invalid<string> failure => $"invalid {string.Join(",", failure.Errors)}",
        };

        Assert.Equal(positional, typed);
    }

    [Property]
    public void Validations_nest(Validated<Validated<int, string>, string> nested)
    {
        var description = nested switch
        {
            Valid<Validated<int, string>>(Valid<int>(var value)) => $"valid valid {value}",
            Valid<Validated<int, string>>(Invalid<string>(var inner)) => $"valid invalid {inner.Length}",
            Invalid<string>(var outer) => $"invalid {outer.Length}",
            _ => "uninitialized inner validation",
        };

        Assert.NotEqual("uninitialized inner validation", description);
    }

    [Property]
    public void Deconstructing_a_case_gives_its_content(int value, NonNull<string> failure)
    {
        new Valid<int>(value).Deconstruct(out var fromValid);
        new Invalid<string>(failure.Get).Deconstruct(out var fromInvalid);

        Assert.Equal(value, fromValid);
        Assert.Equal(new[] { failure.Get }, fromInvalid);
    }

    [Fact]
    public void The_errors_are_a_copy_of_what_was_passed()
    {
        string[] errors = ["first", "second"];

        var invalidated = new Invalid<string>(errors);
        errors[0] = "changed";

        Assert.Equal(new[] { "first", "second" }, invalidated.Errors);
    }
}
