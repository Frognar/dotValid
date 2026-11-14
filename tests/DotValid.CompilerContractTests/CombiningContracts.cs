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

    [Fact]
    public void Combine_infers_every_type_from_the_validations_and_the_combiner()
    {
        var result = Snippet.Compile(Consumer.File("""
                public sealed record Person(string Name, int Age, string Email);

                private static Validated<string, string> Name(string text) => text.Length > 0 ? valid(text) : invalid("name is empty");
                private static Validated<int, string> Age(int age) => age >= 0 ? valid(age) : invalid("age is negative");
                private static Validated<string, string> Email(string text) => text.Contains('@') ? valid(text) : invalid("email has no @");

                public static Validated<Person, string> Lambda(string name, int age, string email) =>
                    Validated.Combine(Name(name), Age(age), Email(email), (n, a, e) => new Person(n, a, e));

                public static Validated<Person, string> Constructor(string name, int age, string email) =>
                    Validated.Combine(Name(name), Age(age), Email(email), Create);

                private static Person Create(string name, int age, string email) => new(name, age, email);
            """));

        ContractAssert.CompilesCleanly(result);
    }

    [Fact]
    public void Combine_needs_one_error_type()
    {
        var result = Snippet.Compile(Consumer.File("""
                public static Validated<int, string> Sum(Validated<int, string> first, Validated<int, int> second) =>
                    Validated.Combine(first, second, (a, b) => a + b);
            """));

        ContractAssert.DoesNotCompile(result);
    }
}

