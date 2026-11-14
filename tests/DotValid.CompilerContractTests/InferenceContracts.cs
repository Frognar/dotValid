namespace DotValid.CompilerContractTests;

/// <summary>
/// How type inference works with the transforming members. Union conversions do not take part in type inference, so
/// a step that returns <c>valid(...)</c> or <c>invalid(...)</c> needs its type spelled out; a delegate that returns a
/// plain value needs nothing.
/// </summary>
public sealed class InferenceContracts
{
    [Fact]
    public void Map_and_MapError_infer_the_new_types()
    {
        var result = Snippet.Compile(Consumer.File("""
                public static Validated<string, string> Text(Validated<int, string> validated) =>
                    validated.Map(value => value.ToString());

                public static Validated<int, int> Lengths(Validated<int, string> validated) =>
                    validated.MapError(error => error.Length);

                public static Validated<int, string> Prefixed(Validated<int, string> validated) =>
                    validated.MapError(error => $"age: {error}");
            """));

        ContractAssert.CompilesCleanly(result);
    }
}
