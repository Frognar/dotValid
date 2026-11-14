namespace DotValid.CompilerContractTests;

/// <summary>
/// A <c>switch</c> over both cases is exhaustive, also in generic code, and positional patterns read the value or
/// the errors (V1).
/// </summary>
public sealed class PatternMatchingContracts
{
    [Fact]
    public void A_switch_over_both_cases_is_exhaustive()
    {
        var result = Snippet.Compile(Consumer.File("""
                public static string Describe(Validated<int, string> validated) => validated switch
                {
                    Valid<int>(var value) => $"valid {value}",
                    Invalid<string>(var errors) => string.Join(", ", errors),
                };
            """));

        ContractAssert.CompilesCleanly(result);
    }

    [Fact]
    public void A_switch_over_both_cases_is_exhaustive_in_generic_code()
    {
        var result = Snippet.Compile(Consumer.File("""
                public static int Count<T, TError>(Validated<T, TError> validated)
                    where T : notnull
                    where TError : notnull => validated switch
                    {
                        Valid<T> => 0,
                        Invalid<TError>(var errors) => errors.Length,
                    };
            """));

        ContractAssert.CompilesCleanly(result);
    }

    [Fact]
    public void A_switch_without_the_invalid_case_is_not_exhaustive()
    {
        var result = Snippet.Compile(Consumer.File("""
                public static int Read(Validated<int, string> validated) => validated switch
                {
                    Valid<int>(var value) => value,
                };
            """));

        ContractAssert.WarnsSwitchIsNotExhaustive(result);
    }

    [Fact]
    public void Cases_of_a_validation_with_the_same_types_are_told_apart()
    {
        var result = Snippet.Compile(Consumer.File("""
                public static string Describe(Validated<string, string> validated) => validated switch
                {
                    Valid<string>(var value) => value,
                    Invalid<string>(var errors) => errors[0],
                };
            """));

        ContractAssert.CompilesCleanly(result);
    }

    [Fact]
    public void Is_patterns_read_the_cases()
    {
        var result = Snippet.Compile(Consumer.File("""
                public static bool Positive(Validated<int, string> validated) =>
                    validated is Valid<int>(var value) && value > 0;

                public static bool Many(Validated<int, string> validated) =>
                    validated is Invalid<string>(var errors) && errors.Length > 1;
            """));

        ContractAssert.CompilesCleanly(result);
    }
}
