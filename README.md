# dotValid

Applicative validation for C# 15 built on union types: a `Validated<T, TError>` is either a valid value or one or more
errors, and independent validations combine by accumulating every error instead of stopping at the first one.

> **Status:** 1.0 is a ground-up rewrite for .NET 11 and C# 15 unions, under active development. The 0.5 line is
> retired; its code lives on the [`archive/0.5`](https://github.com/Frognar/dotValid/tree/archive/0.5) branch.

```csharp
using DotValid;
using static DotValid.Prelude;

Validated<int, string> Age(int age) => age >= 0 ? valid(age) : invalid("age is negative");

var message = Age(-1) switch
{
    Valid<int>(var age) => $"age {age}",
    Invalid<string>(var errors) => string.Join(", ", errors),
};
```

The plan for 1.0 is tracked in the [milestones](https://github.com/Frognar/dotValid/milestones).

## Requirements

.NET 11 (C# 15).

## Building

```bash
dotnet build dotValid.slnx
dotnet test --solution dotValid.slnx
```

The SDK version is pinned in `global.json`. Tests run on Microsoft Testing Platform.

## License

[MIT](LICENSE)
