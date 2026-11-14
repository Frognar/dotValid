using BenchmarkDotNet.Attributes;
using static DotValid.Prelude;

namespace DotValid.Benchmarks;

/// <summary>
/// Validating a small form (three fields) and collecting every error: by hand with ifs and a list of errors (baseline),
/// and with <c>Validated.Combine</c>. <see cref="InvalidPercent"/> is the share of forms with every field invalid. V9: a
/// valid form allocates nothing with <c>Combine</c>; an invalid one allocates its errors, as the list does.
/// </summary>
[MemoryDiagnoser]
public class CombineBenchmarks
{
    private (string Name, int Age, string Email)[] _forms = [];

    [Params(0, 50)]
    public int InvalidPercent { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var invalid = 10 * InvalidPercent;
        _forms = [.. Enumerable.Range(0, 1000).Select(index => index < invalid ? ("", -1, "nobody") : ("Ada", 36, "ada@example.com"))];
    }

    [Benchmark(Baseline = true)]
    public int Imperative()
    {
        var valid = 0;
        foreach (var (name, age, email) in _forms)
        {
            var errors = new List<string>();
            if (name.Length == 0)
            {
                errors.Add("name is empty");
            }

            if (age < 0)
            {
                errors.Add("age is negative");
            }

            if (!email.Contains('@', StringComparison.Ordinal))
            {
                errors.Add("email has no @");
            }

            valid += errors.Count == 0 ? 1 : 0;
        }

        return valid;
    }

    [Benchmark]
    public int Combine()
    {
        var valid = 0;
        foreach (var (name, age, email) in _forms)
        {
            valid += Validated.Combine(Name(name), Age(age), Email(email), static (_, _, _) => 1).OrDefault(0);
        }

        return valid;
    }

    private static Validated<string, string> Name(string name) => name.Length > 0 ? valid(name) : invalid("name is empty");

    private static Validated<int, string> Age(int age) => age >= 0 ? valid(age) : invalid("age is negative");

    private static Validated<string, string> Email(string email) =>
        email.Contains('@', StringComparison.Ordinal) ? valid(email) : invalid("email has no @");
}
