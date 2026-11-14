using DotValid;

// Exercises the public API in a Native AOT binary. Every card adds checks for what it ships: generics over value and
// reference types, union conversions and patterns, accumulation, collections and async.
var failures = new List<string>();

Check(typeof(Validated<int, string>).IsValueType, "Validated is a struct");

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

void Check(bool condition, string what)
{
    if (!condition)
    {
        failures.Add(what);
    }
}
