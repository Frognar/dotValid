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
