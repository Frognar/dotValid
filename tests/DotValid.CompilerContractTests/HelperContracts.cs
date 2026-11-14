namespace DotValid.CompilerContractTests;

/// <summary>
/// How the helpers read at the call site: every type is inferred, and a lambda is an error factory, not an error.
/// </summary>
public sealed class HelperContracts
{
    [Fact]
    public void Check_and_FromNullable_infer_their_types()
    {
        var result = Snippet.Compile(Consumer.File("""
                public static Validated<int, string> Age(int age) => Validated.Check(age, a => a >= 0, "age is negative");
                public static Validated<int, ValidationError> Typed(int age) =>
                    Validated.Check(age, a => a >= 0, new ValidationError("Age", "is negative"));
                public static Validated<int, string> Number(int? number) => Validated.FromNullable(number, "missing");
                public static Validated<string, string> Text(string? text) => Validated.FromNullable(text, "missing");
            """));

        ContractAssert.CompilesCleanly(result);
    }

    [Fact]
    public void A_lambda_is_an_error_factory_not_an_error()
    {
        // A lambda has a natural type, so TError = Func<...> would fit the other overload too; the more specific factory
        // overload wins and the error type stays string.
        var result = Snippet.Compile(Consumer.File("""
                public static Validated<int, string> Age(int age)
                {
                    var checkedAge = Validated.Check(age, a => a >= 0, a => $"{a} is negative");
                    return checkedAge;
                }

                public static Validated<string, string> Text(string? text)
                {
                    var found = Validated.FromNullable(text, () => "missing");
                    return found;
                }
            """));

        ContractAssert.CompilesCleanly(result);
    }
}
