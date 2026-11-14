using FsCheck;
using FsCheck.Xunit;

namespace DotValid.PropertyTests;

/// <summary>
/// Validations are equal when they are the same case with equal contents: equal values, or the same errors in the same
/// order (V11). <c>Validated&lt;bool, bool&gt;</c> has few values, so equal pairs and triples come up often.
/// </summary>
public sealed class EqualityProperties
{
    [Property]
    public void Equality_follows_the_case_and_the_content(Validated<bool, bool> left, Validated<bool, bool> right) =>
        Assert.Equal(Validations.Describe(left) == Validations.Describe(right), left.Equals(right));

    [Property]
    public void Equality_is_reflexive(Validated<int, string> validated)
    {
        var copy = validated;

        Assert.True(validated.Equals(copy));
        Assert.True(validated == copy);
    }

    [Property]
    public void Equality_is_symmetric(Validated<bool, bool> left, Validated<bool, bool> right) =>
        Assert.Equal(left.Equals(right), right.Equals(left));

    [Property]
    public void Equality_is_transitive(Validated<bool, bool> a, Validated<bool, bool> b, Validated<bool, bool> c)
    {
        if (a.Equals(b) && b.Equals(c))
        {
            Assert.True(a.Equals(c));
        }
    }

    [Property]
    public void Operators_agree_with_Equals(Validated<bool, bool> left, Validated<bool, bool> right)
    {
        Assert.Equal(left.Equals(right), left == right);
        Assert.Equal(!left.Equals(right), left != right);
    }

    [Property]
    public void Equals_on_objects_agrees_with_typed_Equals(Validated<bool, bool> left, Validated<bool, bool> right)
    {
        Assert.Equal(left.Equals(right), left.Equals((object)right));
        Assert.False(left.Equals(null));
        Assert.False(left.Equals((object)true));
    }

    [Property]
    public void Equal_validations_have_equal_hash_codes(Validated<bool, bool> left, Validated<bool, bool> right)
    {
        if (left.Equals(right))
        {
            Assert.Equal(left.GetHashCode(), right.GetHashCode());
        }
    }

    [Property]
    public void A_valid_validation_never_equals_an_invalid_one_with_the_same_content(int content)
    {
        var success = Validations.Valid<int, int>(content);
        var failure = Validations.Invalid<int, int>(content);

        Assert.False(success.Equals(failure));
        Assert.True(success != failure);
    }

    [Property]
    public void The_order_of_the_errors_matters(int first, int second)
    {
        var forward = Validations.Invalid<bool, int>(first, second);
        var backward = Validations.Invalid<bool, int>(second, first);

        Assert.Equal(first == second, forward == backward);
    }

    [Property]
    public void Invalid_cases_with_more_errors_differ(NonEmptyArray<int> errors, int extra)
    {
        var some = new Invalid<int>(errors.Get);
        var more = new Invalid<int>([.. errors.Get, extra]);

        Assert.False(some == more);
        Assert.True(some != more);
    }

    [Property]
    public void Cases_are_equal_when_their_contents_are(bool left, bool right)
    {
        Assert.Equal(left == right, new Valid<bool>(left) == new Valid<bool>(right));
        Assert.Equal(left != right, new Valid<bool>(left) != new Valid<bool>(right));
        Assert.Equal(left == right, new Invalid<bool>(left) == new Invalid<bool>(right));
        Assert.Equal(left != right, new Invalid<bool>(left) != new Invalid<bool>(right));
        Assert.Equal(left == right, new Valid<bool>(left).Equals((object)new Valid<bool>(right)));
        Assert.Equal(left == right, new Invalid<bool>(left).Equals((object)new Invalid<bool>(right)));
    }

    [Property]
    public void Equal_cases_have_equal_hash_codes(int[] errors)
    {
        if (errors.Length > 0)
        {
            Assert.Equal(new Invalid<int>(errors).GetHashCode(), new Invalid<int>([.. errors]).GetHashCode());
        }

        Assert.Equal(new Valid<int[]>(errors).GetHashCode(), new Valid<int[]>(errors).GetHashCode());
    }

    [Fact]
    public void Hash_codes_depend_on_the_errors_and_the_case()
    {
        // Not a law (different contents may collide), but these fixed pairs must not: hashing reads every error.
        Assert.NotEqual(new Invalid<int>(1).GetHashCode(), new Invalid<int>(2).GetHashCode());
        Assert.NotEqual(new Invalid<int>(1, 2).GetHashCode(), new Invalid<int>(1, 3).GetHashCode());
        Assert.NotEqual(Validations.Valid<int, int>(1).GetHashCode(), Validations.Invalid<int, int>(1).GetHashCode());
    }

    [Fact]
    public void Default_invalid_cases_are_equal_to_each_other_only()
    {
        Assert.True(default(Invalid<int>) == default(Invalid<int>));
        Assert.Equal(default(Invalid<int>).GetHashCode(), default(Invalid<int>).GetHashCode());
        Assert.False(default(Invalid<int>) == new Invalid<int>(0));
        Assert.False(new Invalid<int>(0) == default(Invalid<int>));
    }

    [Property]
    public void A_case_never_equals_another_type(int content)
    {
        Assert.False(new Valid<int>(content).Equals(new Invalid<int>(content)));
        Assert.False(new Invalid<int>(content).Equals(new Valid<int>(content)));
        Assert.False(new Valid<int>(content).Equals(null));
        Assert.False(new Invalid<int>(content).Equals(null));
    }
}
