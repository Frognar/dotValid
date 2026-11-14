using FsCheck;
using FsCheck.Xunit;
using static DotValid.Prelude;

namespace DotValid.PropertyTests;

/// <summary>
/// The explicitly implemented union members that the compiler calls: exactly one <c>TryGetValue</c> succeeds, and
/// <c>Value</c> is the boxed case.
/// </summary>
public sealed class UnionMembersProperties
{
    [Property]
    public void Exactly_one_TryGetValue_succeeds(Validated<int, string> validated)
    {
        var members = Members(validated);

        Assert.NotEqual(members.TryGetValue(out Valid<int> _), members.TryGetValue(out Invalid<string> _));
    }

    [Property]
    public void TryGetValue_gives_the_valid_case(int value)
    {
        var members = Members(valid(value));

        Assert.True(members.TryGetValue(out Valid<int> success));
        Assert.Equal(new Valid<int>(value), success);
        Assert.False(members.TryGetValue(out Invalid<string> failure));
        Assert.Equal(default, failure);
    }

    [Property]
    public void TryGetValue_gives_the_invalid_case(NonNull<string> failure)
    {
        var members = Members(invalid(failure.Get));

        Assert.True(members.TryGetValue(out Invalid<string> found));
        Assert.Equal(new Invalid<string>(failure.Get), found);
        Assert.False(members.TryGetValue(out Valid<int> success));
        Assert.Equal(default, success);
    }

    [Property]
    public void Value_is_the_boxed_case(Validated<int, string> validated)
    {
        var expected = validated switch
        {
            Valid<int> success => (object)success,
            Invalid<string> failure => failure,
        };

        Assert.Equal(expected, Members(validated).Value);
    }

    private static Validated<int, string>.IUnionMembers Members(Validated<int, string> validated) => validated;
}
