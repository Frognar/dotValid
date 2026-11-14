using BenchmarkDotNet.Attributes;

namespace DotValid.Benchmarks;

/// <summary>
/// Validating a small form (three fields) and collecting every error, written by hand with ifs and a list of errors.
/// The validation-based versions arrive with <c>Combine</c> (TP-07). <see cref="InvalidPercent"/> is the share of
/// forms with at least one invalid field.
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
}
