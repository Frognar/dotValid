using System.Runtime.CompilerServices;
using FsCheck.Xunit;

namespace DotValid.PropertyTests;

/// <summary>
/// V3: <c>default(Validated&lt;T, TError&gt;)</c> is neither case. Pattern matching never mistakes it for one, a
/// <c>switch</c> fails loudly, and the observing members (equality, hashing, <c>is null</c>) keep working.
/// </summary>
/// <remarks>
/// Every consuming member (Match, Map, Combine, …) adds a line here: it throws <see cref="InvalidOperationException"/>.
/// </remarks>
public sealed class UninitializedProperties
{
    private static Validated<int, string> Uninitialized => default;

    [Fact]
    public void A_switch_over_both_cases_throws()
    {
        Assert.Throws<SwitchExpressionException>(() => Describe(Uninitialized));

        static string Describe(Validated<int, string> validated) => validated switch
        {
            Valid<int>(var value) => $"valid {value}",
            Invalid<string>(var errors) => errors[0],
        };
    }

    [Fact]
    public void Neither_case_pattern_matches()
    {
        Assert.False(Uninitialized is Valid<int>);
        Assert.False(Uninitialized is Invalid<string>);
    }

    [Fact]
    public void A_discard_arm_catches_it()
    {
        var description = Uninitialized switch
        {
            Valid<int> => "valid",
            Invalid<string> => "invalid",
            _ => "uninitialized",
        };

        Assert.Equal("uninitialized", description);
    }

    [Fact]
    public void The_null_pattern_matches_it()
    {
        Assert.True(Uninitialized is null);
        Assert.False(Uninitialized is not null);
    }

    [Property]
    public void The_null_pattern_matches_no_initialized_validation(Validated<int, string> validated)
    {
        Assert.False(validated is null);
        Assert.True(validated is not null);
    }

    [Fact]
    public void Uninitialized_validations_are_equal()
    {
        var other = default(Validated<int, string>);

        Assert.True(Uninitialized.Equals(other));
        Assert.True(Uninitialized == other);
        Assert.False(Uninitialized != other);
        Assert.Equal(Uninitialized.GetHashCode(), other.GetHashCode());
    }

    [Property]
    public void An_uninitialized_validation_equals_no_initialized_one(Validated<int, string> validated)
    {
        Assert.False(Uninitialized.Equals(validated));
        Assert.False(validated.Equals(Uninitialized));
        Assert.True(validated != Uninitialized);
    }

    [Fact]
    public void Match_and_OrDefault_throw_and_run_no_branch()
    {
        var calls = 0;

        var exception = Assert.Throws<InvalidOperationException>(() => Uninitialized.Match(_ => ++calls, _ => ++calls));
        Assert.Contains("valid(value) or invalid(error)", exception.Message, StringComparison.Ordinal);
        Assert.Throws<InvalidOperationException>(() => Uninitialized.OrDefault(0));
        Assert.Throws<InvalidOperationException>(() => Uninitialized.OrDefault(_ => ++calls));
        Assert.Equal(0, calls);
    }

    [Fact]
    public void Map_and_MapError_throw_and_run_nothing()
    {
        var calls = 0;

        Assert.Throws<InvalidOperationException>(() => Uninitialized.Map(value => ++calls + value));
        Assert.Throws<InvalidOperationException>(() => Uninitialized.MapError(error => ++calls + error.Length));
        Assert.Equal(0, calls);
    }

    [Fact]
    public void Then_Ensure_and_Select_throw_and_run_nothing()
    {
        var calls = 0;

        Assert.Throws<InvalidOperationException>(() => Uninitialized.Then(value => Validations.Valid<int, string>(++calls)));
        Assert.Throws<InvalidOperationException>(() => Uninitialized.Ensure(_ => ++calls > 0, "error"));
        Assert.Throws<InvalidOperationException>(() => Uninitialized.Ensure(_ => ++calls > 0, _ => $"{++calls}"));
        Assert.Throws<InvalidOperationException>(() => Uninitialized.Select(value => ++calls + value));
        Assert.Equal(0, calls);
    }

    [Fact]
    public void Zip_and_Apply_throw_and_run_nothing()
    {
        var calls = 0;

        Assert.Throws<InvalidOperationException>(() => Uninitialized.Zip(Validations.Valid<int, string>(1)));
        Assert.Throws<InvalidOperationException>(() => Uninitialized.Zip(Validations.Valid<int, string>(1), (a, b) => ++calls + a + b));
        Assert.Throws<InvalidOperationException>(
            () => default(Validated<Func<int, int>, string>).Apply(Validations.Valid<int, string>(1)));
        Assert.Equal(0, calls);
    }

    [Fact]
    public void The_union_members_report_no_case()
    {
        Validated<int, string>.IUnionMembers members = Uninitialized;

        Assert.Null(members.Value);
        Assert.False(members.TryGetValue(out Valid<int> _));
        Assert.False(members.TryGetValue(out Invalid<string> _));
    }
}
