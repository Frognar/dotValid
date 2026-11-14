using DotValid;
using static DotValid.Prelude;

// Exercises the public API in a Native AOT binary. Every card adds checks for what it ships: generics over value and
// reference types, union conversions and patterns, accumulation, collections and async.
var failures = new List<string>();

Validated<int, string> age = valid(42);
Validated<int, string> failed = invalid("empty", "too short");
Validated<string, string> same = invalid("same type");

Check(age is Valid<int>(42), "Valid pattern");
Check(failed is Invalid<string>(["empty", "too short"]), "Invalid pattern with every error");
Check(Describe(age) == "valid 42" && Describe(failed) == "2 errors", "exhaustive switch");
Check(same is Invalid<string> && same is not Valid<string>, "Validated<string, string>");
Check(Parse("7") == valid(7) && Parse("x") == invalid("not a number"), "conditional conversion");
Check(Generic<int, string>(1) && Generic<string, string>("text"), "generic patterns");
Check(failed == invalid("empty", "too short") && failed != invalid("too short", "empty"), "error order matters");
Check(age.GetHashCode() == Parse("42").GetHashCode(), "hash codes");
Check(default(Validated<int, string>) is null && age is not null, "uninitialized validation");
Check(Throws<ArgumentNullException>(static () => valid<string>(null!)), "null value rejected");
Check(Throws<ArgumentException>(static () => invalid<string>()), "no errors rejected");
Check($"{age} {failed} {default(Validated<int, string>)}" == "Valid(42) Invalid([empty, too short]) Uninitialized", "ToString");
Check(age.Match(static value => value, static errors => -errors.Length) == 42, "Match on Valid");
Check(failed.Match(static value => value, static errors => -errors.Length) == -2, "Match on Invalid");
Check(age.OrDefault(0) == 42 && failed.OrDefault(0) == 0 && failed.OrDefault(static errors => errors.Length) == 2, "OrDefault");
Check(Throws<InvalidOperationException>(static () => default(Validated<int, string>).OrDefault(0)), "uninitialized OrDefault");
Check(age.Map(static value => value * 2) == valid(84) && failed.Map(static value => value * 2) == failed.Map(static value => value), "Map");
Check(failed.MapError(static error => error.Length) == invalid(5, 9) && age.MapError(static error => error.Length) == valid(42), "MapError");
Check(Parse("20").Then(Adult) == valid(20) && Parse("7").Then(Adult) == invalid("too young") && failed.Then(Adult) == failed, "Then");
Check(age.Ensure(static value => value > 100, "too small") == invalid("too small") && age.Ensure(static value => value > 0, "never") == age, "Ensure");
Check((from value in age select value + 1) == valid(43), "query syntax");
Validated<string, string> name = invalid("name is empty");
Check(age.Zip(Parse("7")) == valid((42, 7)) && name.Zip(failed) == invalid("name is empty", "empty", "too short"), "Zip accumulates");
Check(name.Zip(age, static (n, a) => $"{n} {a}") == invalid("name is empty"), "Zip(combine)");
Validated<Func<int, int>, string> twice = valid<Func<int, int>>(static value => value * 2);
Check(twice.Apply(age) == valid(84) && twice.Apply(failed) == failed, "Apply");
Check(
    Validated.Combine(name, age, failed, static (n, a, f) => $"{n} {a} {f}") == invalid("name is empty", "empty", "too short")
    && Validated.Combine(age, age, age, static (a, b, c) => a + b + c) == valid(126),
    "Combine");
Check(
    Validated.Check(-1, static value => value >= 0, new ValidationError("Age", "is negative")).ToString() == "Invalid([Age: is negative])"
    && Validated.Check(5, static value => value >= 0, static value => $"{value}") == valid(5),
    "Check and ValidationError");
Check(
    Validated.FromNullable((int?)null, "missing") == invalid("missing")
    && Validated.FromNullable((string?)"text", static () => 0) == valid("text"),
    "FromNullable");
string[] lines = ["1", "x", "3", "y"];
Check(lines.Traverse(Parse) == invalid("not a number", "not a number") && lines[..1].Traverse(Parse).Map(static numbers => numbers.Sum()) == valid(1), "Traverse");
Validated<int, string>[] mixed = [age, failed, Parse("7")];
Check(mixed.Sequence() == invalid("empty", "too short") && mixed.Values().Sum() == 49 && mixed.Errors().Count() == 2, "Sequence, Values, Errors");
var (passed, problems) = mixed.Partition();
Check(passed is [42, 7] && problems is ["empty", "too short"], "Partition");
var described = await age.MatchAsync(
    static async value =>
    {
        await Task.Yield();
        return $"valid {value}";
    },
    static errors => $"{errors.Length} errors");
Check(described == "valid 42" && await failed.MatchAsync(static _ => Task.FromResult(0), static errors => errors.Length) == 2, "MatchAsync");
Check(await age.MapAsync(static value => Task.FromResult(value + 1)) == valid(43) && await failed.MapAsync(static value => Task.FromResult(value)) == failed, "MapAsync");
Check(await Parse("7").ThenAsync(static value => Task.FromResult(Adult(value))) == invalid("too young"), "ThenAsync");

if (failures.Count > 0)
{
    foreach (var failure in failures)
    {
        Console.Error.WriteLine($"FAILED: {failure}");
    }

    return 1;
}

Console.WriteLine("Native AOT smoke test passed.");
return 0;

static string Describe(Validated<int, string> validated) => validated switch
{
    Valid<int>(var value) => $"valid {value}",
    Invalid<string>(var errors) => $"{errors.Length} errors",
};

static Validated<int, string> Adult(int value) => value >= 18 ? valid(value) : invalid("too young");

static Validated<int, string> Parse(string text) =>
    int.TryParse(text, out var number) ? valid(number) : invalid("not a number");

static bool Generic<T, TError>(T value)
    where T : notnull
    where TError : notnull
{
    Validated<T, TError> validated = valid(value);
    return validated is Valid<T>(var kept) && EqualityComparer<T>.Default.Equals(kept, value);
}

static bool Throws<TException>(Action action)
    where TException : Exception
{
    try
    {
        action();
        return false;
    }
    catch (TException)
    {
        return true;
    }
}

void Check(bool condition, string what)
{
    if (!condition)
    {
        failures.Add(what);
    }
}
