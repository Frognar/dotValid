namespace DotValid.CompilerContractTests;

/// <summary>
/// How the collection helpers read at the call site. The consumer file imports only <c>System</c>, so collection types
/// are written out in full.
/// </summary>
public sealed class CollectionContracts
{
    private const string Steps = """
            private static Validated<int, string> Parse(string text) =>
                int.TryParse(text, out var number) ? valid(number) : invalid("not a number");
        """;

    [Fact]
    public void Collection_helpers_read_like_LINQ()
    {
        var result = Snippet.Compile(Consumer.File(Steps + """
                public static Validated<System.Collections.Generic.IReadOnlyList<int>, string> All(string[] texts) =>
                    texts.Traverse(Parse);

                public static Validated<System.Collections.Generic.IReadOnlyList<int>, string> AllOf(
                    System.Collections.Generic.List<Validated<int, string>> validations) => validations.Sequence();

                public static System.Collections.Generic.IEnumerable<int> Good(Validated<int, string>[] validations) =>
                    validations.Values();

                public static System.Collections.Generic.IEnumerable<string> Problems(Validated<int, string>[] validations) =>
                    validations.Errors();

                public static int Split(Validated<int, string>[] validations)
                {
                    var (values, errors) = validations.Partition();
                    return values.Count - errors.Count;
                }
            """));

        ContractAssert.CompilesCleanly(result);
    }

    [Fact]
    public void Traverse_cannot_infer_the_types_from_a_lambda_that_returns_valid_or_invalid()
    {
        var bare = Snippet.Compile(Consumer.File(Steps + """
                public static Validated<System.Collections.Generic.IReadOnlyList<int>, string> Lengths(string[] texts) =>
                    texts.Traverse(text => text.Length > 0 ? valid(text.Length) : invalid("empty"));
            """));
        var typed = Snippet.Compile(Consumer.File(Steps + """
                public static Validated<System.Collections.Generic.IReadOnlyList<int>, string> Lengths(string[] texts) =>
                    texts.Traverse(Validated<int, string> (string text) => text.Length > 0 ? valid(text.Length) : invalid("empty"));
            """));

        ContractAssert.FailsWith(bare, "CS0411");
        ContractAssert.CompilesCleanly(typed);
    }
}
