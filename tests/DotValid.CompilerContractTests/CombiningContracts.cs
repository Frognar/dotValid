namespace DotValid.CompilerContractTests;

/// <summary>
/// How the accumulating combinators read at the call site: every validation in one combination shares one error type.
/// </summary>
public sealed class CombiningContracts
{
    [Fact]
    public void Zip_infers_the_pair_and_the_combined_value()
    {
        var result = Snippet.Compile(Consumer.File("""
                public static Validated<(string, int), string> Pair(Validated<string, string> name, Validated<int, string> age) =>
                    name.Zip(age);

                public static Validated<string, string> Person(Validated<string, string> name, Validated<int, string> age) =>
                    name.Zip(age, (n, a) => $"{n} ({a})");
            """));

        ContractAssert.CompilesCleanly(result);
    }

    [Fact]
    public void Apply_chains_over_a_curried_function()
    {
        var result = Snippet.Compile(Consumer.File("""
                public static Validated<string, string> Person(
                    Validated<Func<string, Func<int, string>>, string> person,
                    Validated<string, string> name,
                    Validated<int, string> age) => person.Apply(name).Apply(age);
            """));

        ContractAssert.CompilesCleanly(result);
    }

    [Fact]
    public void Validations_with_different_error_types_do_not_combine()
    {
        // Translate one side with MapError first.
        var result = Snippet.Compile(Consumer.File("""
                public static Validated<(string, int), string> Pair(Validated<string, string> name, Validated<int, int> age) =>
                    name.Zip(age);
            """));

        ContractAssert.DoesNotCompile(result);
    }
}
