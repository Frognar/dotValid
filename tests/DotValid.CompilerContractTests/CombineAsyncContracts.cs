namespace DotValid.CompilerContractTests;

/// <summary>
/// How <c>Validated.CombineAsync</c> reads at the call site (V10, decided in #16): every input is a task. C# infers no
/// type arguments through conversions, so a bare validation does not fit; it goes in as <c>Task.FromResult</c>, or the
/// synchronous checks are combined on their own and joined with <c>Zip</c>.
/// </summary>
public sealed class CombineAsyncContracts
{
    private const string Checks = """
            private static Validated<int, string> Age(int age) => age >= 0 ? valid(age) : invalid("age is negative");
            private static Validated<string, string> Name(string name) => name.Length > 0 ? valid(name) : invalid("name is empty");
            private static Task<Validated<string, string>> EmailFree(string email) => Task.FromResult<Validated<string, string>>(valid(email));
            private static Task<Validated<string, string>> LoginFree(string login) => Task.FromResult<Validated<string, string>>(valid(login));
        """;

    [Fact]
    public void Tasks_infer_every_type()
    {
        var result = Snippet.Compile(Consumer.File(Checks + """
                public static async Task<Validated<string, string>> User(string email, string login) =>
                    await Validated.CombineAsync(EmailFree(email), LoginFree(login), (e, l) => $"{e} {l}");
            """));

        ContractAssert.CompilesCleanly(result);
    }

    [Fact]
    public void A_bare_validation_does_not_fit_but_Task_FromResult_does()
    {
        var bare = Snippet.Compile(Consumer.File(Checks + """
                public static async Task<Validated<string, string>> User(int age, string email) =>
                    await Validated.CombineAsync(Age(age), EmailFree(email), (a, e) => $"{e} {a}");
            """));
        var wrapped = Snippet.Compile(Consumer.File(Checks + """
                public static async Task<Validated<string, string>> User(int age, string email) =>
                    await Validated.CombineAsync(Task.FromResult(Age(age)), EmailFree(email), (a, e) => $"{e} {a}");
            """));

        ContractAssert.FailsWith(bare, "CS0411");
        ContractAssert.CompilesCleanly(wrapped);
    }

    [Fact]
    public void Synchronous_and_asynchronous_checks_combine_separately_and_join_with_Zip()
    {
        var result = Snippet.Compile(Consumer.File(Checks + """
                public static async Task<Validated<string, string>> User(string name, int age, string email, string login)
                {
                    var local = Validated.Combine(Name(name), Age(age), (n, a) => (n, a));
                    var remote = await Validated.CombineAsync(EmailFree(email), LoginFree(login), (e, l) => (e, l));
                    return local.Zip(remote, (l, r) => $"{l.n} {l.a} {r.e} {r.l}");
                }
            """));

        ContractAssert.CompilesCleanly(result);
    }
}
