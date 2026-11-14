namespace DotValid.CompilerContractTests;

/// <summary>
/// Query syntax stops at <c>select</c> (V8): a second <c>from</c> would be a monadic bind that silently stops at the
/// first invalid validation, which reads like accumulation but is not. Use <c>Then</c> for dependent steps and
/// <c>Zip</c>/<c>Combine</c> to collect every error. There is no <c>where</c> either: a failed predicate has no error.
/// </summary>
public sealed class QuerySyntaxContracts
{
    [Fact]
    public void A_query_with_one_from_and_select_compiles()
    {
        var result = Snippet.Compile(Consumer.File("""
                public static Validated<string, string> Label(Validated<int, string> age) =>
                    from value in age select $"age {value}";
            """));

        ContractAssert.CompilesCleanly(result);
    }

    [Fact]
    public void A_second_from_does_not_compile()
    {
        var result = Snippet.Compile(Consumer.File("""
                public static Validated<int, string> Sum(Validated<int, string> first, Validated<int, string> second) =>
                    from a in first
                    from b in second
                    select a + b;
            """));

        ContractAssert.FailsMentioning(result, "SelectMany");
    }

    [Fact]
    public void Where_does_not_compile()
    {
        var result = Snippet.Compile(Consumer.File("""
                public static Validated<int, string> Positive(Validated<int, string> age) =>
                    from value in age where value > 0 select value;
            """));

        ContractAssert.FailsMentioning(result, "Where");
    }
}
