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

    private const string Steps = """
            private static Validated<int, string> Adult(int age) => age >= 18 ? valid(age) : invalid("too young");
        """;

    [Fact]
    public void Then_infers_the_type_of_a_step_that_returns_a_validation()
    {
        var result = Snippet.Compile(Consumer.File(Steps + """
                public static Validated<int, string> Group(Validated<int, string> age) => age.Then(Adult);
                public static Validated<int, string> Lambda(Validated<int, string> age) => age.Then(value => Adult(value));
            """));

        ContractAssert.CompilesCleanly(result);
    }

    [Fact]
    public void Then_cannot_infer_the_type_from_a_lambda_that_returns_valid_or_invalid()
    {
        var result = Snippet.Compile(Consumer.File(Steps + """
                public static Validated<int, string> Positive(Validated<int, string> age) =>
                    age.Then(value => value > 0 ? valid(value) : invalid("not positive"));
            """));

        ContractAssert.FailsWith(result, "CS0411");
    }

    [Fact]
    public void An_explicit_type_argument_or_lambda_return_type_lets_Then_take_valid_and_invalid()
    {
        var result = Snippet.Compile(Consumer.File(Steps + """
                public static Validated<int, string> Argument(Validated<int, string> age) =>
                    age.Then<int>(value => value > 0 ? valid(value) : invalid("not positive"));

                public static Validated<int, string> ReturnType(Validated<int, string> age) =>
                    age.Then(Validated<int, string> (int value) => value > 0 ? valid(value) : invalid("not positive"));
            """));

        ContractAssert.CompilesCleanly(result);
    }
}

