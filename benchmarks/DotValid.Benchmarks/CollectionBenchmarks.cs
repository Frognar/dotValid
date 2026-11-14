using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using static DotValid.Prelude;

namespace DotValid.Benchmarks;

/// <summary>
/// Sequences of validations, read by hand with pattern matching (the baseline of each category) and with the collection
/// helpers: all-or-every-error (<c>Sequence</c>) and splitting (<c>Partition</c>). Every third validation in the mixed
/// input is invalid. The helpers allocate like LINQ does (enumerators, lists); the point is to stay close to the
/// hand-written loops.
/// </summary>
[MemoryDiagnoser]
[CategoriesColumn]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
public class CollectionBenchmarks
{
    private Validated<int, string>[] _mixed = [];
    private Validated<int, string>[] _complete = [];

    [Params(10, 1_000)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _mixed = [.. Enumerable.Range(0, Size).Select(static index => index % 3 == 0 ? Fail(index) : Pass(index))];
        _complete = [.. Enumerable.Range(0, Size).Select(static index => Pass(index))];
    }

    [BenchmarkCategory("Sequence")]
    [Benchmark(Baseline = true)]
    public int ManualSequence()
    {
        var values = new List<int>(_complete.Length);
        var errors = new List<string>();
        foreach (var validated in _complete)
        {
            switch (validated)
            {
                case Valid<int>(var value):
                    values.Add(value);
                    break;
                case Invalid<string>(var some):
                    errors.AddRange(some);
                    break;
            }
        }

        return errors.Count == 0 ? values.Count : -errors.Count;
    }

    [BenchmarkCategory("Sequence")]
    [Benchmark]
    public int Sequence() => _complete.Sequence().Match(static values => values.Count, static errors => -errors.Length);

    [BenchmarkCategory("Partition")]
    [Benchmark(Baseline = true)]
    public int ManualPartition()
    {
        var values = new List<int>();
        var errors = new List<string>();
        foreach (var validated in _mixed)
        {
            switch (validated)
            {
                case Valid<int>(var value):
                    values.Add(value);
                    break;
                case Invalid<string>(var some):
                    errors.AddRange(some);
                    break;
            }
        }

        return values.Count - errors.Count;
    }

    [BenchmarkCategory("Partition")]
    [Benchmark]
    public int Partition()
    {
        var (values, errors) = _mixed.Partition();
        return values.Count - errors.Count;
    }

    private static Validated<int, string> Pass(int value) => valid(value);

    private static Validated<int, string> Fail(int value) => invalid($"{value} failed");
}
