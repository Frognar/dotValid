namespace DotValid.CompilerContractTests;

/// <summary>
/// The only ways into a validation are the two cases, <c>Valid&lt;T&gt;</c> and <c>Invalid&lt;TError&gt;</c>, which
/// convert implicitly wherever a validation is expected (V1, V2).
/// </summary>
public sealed class ConversionContracts
{
    [Fact]
    public void Valid_converts_to_a_validation_with_any_error_type()
    {
        var result = Snippet.Compile(Consumer.File("""
                public static Validated<int, string> WithText() => valid(42);
                public static Validated<int, Exception> WithException() => valid(42);
            """));

        ContractAssert.CompilesCleanly(result);
    }

    [Fact]
    public void Invalid_converts_with_one_error_or_several()
    {
        var result = Snippet.Compile(Consumer.File("""
                public static Validated<int, string> One() => invalid("empty");
                public static Validated<int, string> Two() => invalid("empty", "too short");
                public static Validated<int, string> Spread(string[] errors) => invalid([.. errors]);
            """));

        ContractAssert.CompilesCleanly(result);
    }

    [Theory]
    [InlineData("string[]")]
    [InlineData("System.Collections.Generic.List<string>")]
    public void A_collection_of_errors_needs_a_spread(string collection)
    {
        // invalid(errors) binds to invalid(TError error) with TError = the collection type (an identity conversion
        // beats array-to-span), so the collection becomes the single error and no longer converts to a validation with
        // string errors: a compile error, never a silent mix-up. The spread passes the elements.
        var bare = Snippet.Compile(Consumer.File($$"""
                public static Validated<int, string> Bare({{collection}} errors) => invalid(errors);
            """));
        var spread = Snippet.Compile(Consumer.File($$"""
                public static Validated<int, string> Spread({{collection}} errors) => invalid([.. errors]);
            """));

        ContractAssert.DoesNotCompile(bare);
        ContractAssert.CompilesCleanly(spread);
    }

    [Fact]
    public void A_conditional_between_valid_and_invalid_is_target_typed()
    {
        var result = Snippet.Compile(Consumer.File("""
                public static Validated<int, string> Parse(string text) =>
                    int.TryParse(text, out var number) ? valid(number) : invalid("not a number");
            """));

        ContractAssert.CompilesCleanly(result);
    }

    [Fact]
    public void Cases_convert_to_a_validation_parameter_and_variable()
    {
        var result = Snippet.Compile(Consumer.File("""
                public static int Use(Validated<int, string> validated) => 0;

                public static int Call()
                {
                    Validated<int, string> failed = invalid("no", "never");
                    return Use(valid(1)) + Use(failed);
                }
            """));

        ContractAssert.CompilesCleanly(result);
    }

    [Fact]
    public void Cases_of_type_parameters_convert_in_generic_code()
    {
        var result = Snippet.Compile(Consumer.File("""
                public static Validated<T, TError> Succeed<T, TError>(T value)
                    where T : notnull
                    where TError : notnull => valid(value);

                public static Validated<T, TError> Fail<T, TError>(TError first, TError second)
                    where T : notnull
                    where TError : notnull => invalid(first, second);
            """));

        ContractAssert.CompilesCleanly(result);
    }

    [Fact]
    public void A_validation_with_the_same_value_and_error_type_is_unambiguous()
    {
        var result = Snippet.Compile(Consumer.File("""
                public static Validated<string, string> Succeed() => valid("value");
                public static Validated<string, string> Fail() => invalid("error");
            """));

        ContractAssert.CompilesCleanly(result);
    }

    [Fact]
    public void A_bare_value_or_error_does_not_convert_to_a_validation()
    {
        var value = Snippet.Compile(Consumer.File("""
                public static Validated<int, string> Make() => 42;
            """));
        var error = Snippet.Compile(Consumer.File("""
                public static Validated<int, string> Make() => "not a number";
            """));

        ContractAssert.DoesNotCompile(value);
        ContractAssert.DoesNotCompile(error);
    }

    [Fact]
    public void The_prelude_works_next_to_application_types_named_Valid_and_Invalid()
    {
        var result = Snippet.Compile(Consumer.File("""
                public sealed record Valid(string Message);
                public sealed record Invalid(string Code);

                public static Validated<Valid, Invalid> Run(bool good) =>
                    good ? valid(new Valid("done")) : invalid(new Invalid("E42"));
            """));

        ContractAssert.CompilesCleanly(result);
    }

    [Fact]
    public void A_maybe_null_value_or_error_is_flagged_by_nullable_analysis()
    {
        var value = Snippet.Compile(Consumer.File("""
                public static Validated<string, int> From(string? text) => valid(text);
            """));
        var error = Snippet.Compile(Consumer.File("""
                public static Validated<int, string> From(string? text) => invalid(text);
            """));

        ContractAssert.WarnsAboutNullability(value);
        ContractAssert.WarnsAboutNullability(error);
    }
}
