namespace DotValid.CompilerContractTests;

/// <summary>
/// No accessors (V5): the value and the errors come out only through pattern matching and total functions, and the
/// union members stay out of the public API.
/// </summary>
public sealed class EncapsulationContracts
{
    [Theory]
    [InlineData("validated.Value")]
    [InlineData("validated.Errors")]
    [InlineData("validated.IsValid")]
    [InlineData("validated.TryGetValue(out Valid<int> valid)")]
    public void A_validation_has_no_accessors(string expression)
    {
        var result = Snippet.Compile(Consumer.File($$"""
                public static object Read(Validated<int, string> validated) => {{expression}};
            """));

        ContractAssert.DoesNotCompile(result);
    }

    [Fact]
    public void A_validation_has_no_public_constructor_from_a_case()
    {
        var result = Snippet.Compile(Consumer.File("""
                public static Validated<int, string> Make() => new Validated<int, string>(new Valid<int>(1));
            """));

        ContractAssert.DoesNotCompile(result);
    }

    [Fact]
    public void The_cases_expose_their_contents()
    {
        var result = Snippet.Compile(Consumer.File("""
                public static int Value(Valid<int> valid) => valid.Value;
                public static int Count(Invalid<string> invalid) => invalid.Errors.Length;
            """));

        ContractAssert.CompilesCleanly(result);
    }
}
