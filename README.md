# dotValid

[![CI](https://github.com/Frognar/dotValid/actions/workflows/ci.yml/badge.svg)](https://github.com/Frognar/dotValid/actions/workflows/ci.yml)

Applicative validation for C# 15 built on union types: a `Validated<T, TError>` is either a valid value or one or more
errors, and independent validations combine by accumulating every error instead of stopping at the first one.

> **Status:** 1.0 is a ground-up rewrite for .NET 11 and C# 15 unions, now in beta: the API is complete. The 0.5
> line is retired; see [Migrating from 0.5](#migrating-from-05).

```csharp
using DotValid;
using static DotValid.Prelude;

Validated<string, string> Name(string name) => name.Length > 0 ? valid(name) : invalid("name is empty");
Validated<int, string> Age(int age) => age >= 0 ? valid(age) : invalid("age is negative");

Validated<Person, string> person = Validated.Combine(Name(form.Name), Age(form.Age), (n, a) => new Person(n, a));

var message = person switch
{
    Valid<Person>(var p) => $"welcome, {p.Name}",
    Invalid<string>(var errors) => string.Join("; ", errors),   // "name is empty; age is negative"
};
```

- **Every error, not just the first.** Independent validations combine with `Zip`, `Combine`, `CombineAsync`,
  `Sequence` and `Traverse`, which read every input and collect every error in order. An invalid validation is never
  empty.
- **Union types.** `Validated<T, TError>` has exactly two cases, `Valid<T>` and `Invalid<TError>`. Both convert
  implicitly, and a `switch` that handles both is exhaustive. `Validated<string, string>` is never ambiguous.
- **Your error type.** Any type can be the error; `ValidationError(Property, Message)` is there for the common case.
- **No invalid values.** `null` is never a value or an error, and there is no `.Value`, `IsValid` or other accessor
  that can throw: you get the content out through pattern matching, `Match` or `OrDefault`.
- **Functional API.** `Match`, `OrDefault`, `Map`, `MapError`, `Zip`, `Combine`, `Apply`, dependent steps with `Then`
  and `Ensure`, `Check` and `FromNullable`, collections, and async (`MatchAsync`, `MapAsync`, `ThenAsync`,
  `CombineAsync`).
- **No allocations** on the valid path for value types; trimming and Native AOT compatible; no dependencies.

## Usage

Import the namespace, and `Prelude` for the lower-case `valid` and `invalid`:

```csharp
using DotValid;
using static DotValid.Prelude;
```

### Creating

```csharp
Validated<string, string> name = valid("Ada");
Validated<int, string> age = invalid("age is negative");
Validated<int, string> both = invalid("age is negative", "age is not a whole number");
Validated<int, string> many = invalid([.. problems]);           // a collection goes in with a spread

Validated<int, string> Parse(string text) =>
    int.TryParse(text, out var number) ? valid(number) : invalid($"'{text}' is not a number");
```

`valid` and `invalid` create the cases; they convert to any `Validated` with a matching type, so the same `valid(42)`
works for `Validated<int, string>` and `Validated<int, ValidationError>`. There is no conversion from a bare value or
error. `null` is rejected with `ArgumentNullException`, and `invalid` with no errors with `ArgumentException`. Pass a
collection with a spread: `invalid(problems)` would make the whole collection a single error, which the compiler then
rejects.

### Getting values out

```csharp
var message = Parse(text) switch
{
    Valid<int>(var number) => $"parsed {number}",
    Invalid<string>(var errors) => string.Join("; ", errors),   // errors: ImmutableArray<string>, never empty
};

if (Parse(text) is Valid<int>(var number) && number > 0) { /* ... */ }
```

A `switch` over both cases is exhaustive. The errors come in the order they were added. `ToString()` gives
`Valid(42)` or `Invalid([first, second])`, also in the debugger.

```csharp
string text = Parse(input).Match(
    valid: number => $"parsed {number}",
    invalid: errors => $"{errors.Length} problem(s)");     // exactly one branch runs

int number = Parse(input).OrDefault(0);
int length = Parse(input).OrDefault(errors => -errors.Length);   // the factory runs only for an invalid validation
```

`Match` takes the value first, then the errors. Inside such a lambda, a parameter named `valid` or `invalid` hides the
`Prelude` function of the same name; name it `value` or `errors` when the body creates a validation.

### Transforming

```csharp
Validated<string, string> label = Parse(input).Map(number => $"#{number}");     // the errors pass through
Validated<int, ValidationProblem> typed =
    Parse(input).MapError(error => new ValidationProblem("Age", error));        // every error, order kept
```

`Map` transforms the value of a valid validation; `MapError` transforms each error of an invalid one, for example to
attach a field name or translate codes into messages. A `null` from either delegate is rejected with
`InvalidOperationException`: a validation never holds `null`.

### Combining

```csharp
Validated<string, string> name = Name(form.Name);
Validated<int, string> age = Age(form.Age);

Validated<(string, int), string> both = name.Zip(age);                     // every error of both, name's first
Validated<Person, string> person = name.Zip(age, (n, a) => new Person(n, a));
```

For more parts, `Validated.Combine` takes two to eight validations and a function that receives every value:

```csharp
Validated<Person, string> person = Validated.Combine(
    Name(form.Name),
    Age(form.Age),
    Email(form.Email),
    (name, age, email) => new Person(name, age, email));         // or just: Person.Create

// Invalid(["name is empty", "email has no @"]) when two fields are wrong
```

This is where validation differs from a `Result`: `Zip` always reads both sides, and when both are invalid you get the
errors of both, in order. The combining function runs only when every part is valid. All parts share one error type;
translate a part with `MapError` first if they do not. `Apply` is the same thing in curried form,
`personFunction.Apply(name).Apply(age)`, for those who prefer it.

### Building blocks

```csharp
Validated<int, ValidationError> Age(int age) =>
    Validated.Check(age, a => a >= 0, new ValidationError("Age", "must not be negative"));

Validated<string, ValidationError> Email(string? email) =>
    Validated.FromNullable(email, new ValidationError("Email", "is required"))
        .Ensure(e => e.Contains('@'), e => new ValidationError("Email", $"'{e}' has no @"));
```

`Validated.Check` turns a predicate into a validation and `Validated.FromNullable` turns `null` into an error, for
nullable value types and references alike; both also take a function that produces the error only when needed.
`ValidationError(Property, Message)` is a ready-made error type (`ToString()` gives `Property: Message`); any type can
be the error, it is there for the common case. There is no rule DSL on purpose: rules are plain functions that return a
validation.

### Collections

```csharp
Validated<IReadOnlyList<int>, string> ages = lines.Traverse(Parse);   // every value, or every error of every line
Validated<IReadOnlyList<int>, string> all = validations.Sequence();    // Validated<int, string>[] -> list or every error

IEnumerable<int> good = validations.Values();                          // lazily
IEnumerable<string> problems = validations.Errors();                   // lazily, flattened
var (values, errors) = validations.Partition();                        // both, in one pass
```

`Sequence` and `Traverse` accumulate like `Combine`: they read the whole sequence and, if anything is invalid, give
every error of every element in order. As with `Then`, a `Traverse` lambda that returns `valid(...)` or `invalid(...)`
needs its return type spelled out: `lines.Traverse(Validated<int, string> (line) => ...)`.

### Dependent steps

```csharp
Validated<int, string> age = Parse(input)                      // first the format
    .Ensure(number => number >= 0, "age is negative")         // then a rule on the value
    .Then(number => CheckAgainstPolicy(number));             // then a step that may fail

Validated<string, string> label = from number in Parse(input) select $"#{number}";
```

`Then` and `Ensure` need the value, so they run only for a valid validation and stop at the first invalid one, just
like a `Result`: they never collect errors. That is deliberate, and it is why `Then` is not called `Bind` and why query
syntax stops at `select` (a second `from` or a `where` does not compile). To report every problem at once, validate the
independent parts separately and combine them with `Zip`. A `Then` lambda that returns `valid(...)` or
`invalid(...)` needs its type spelled out, `Then<int>(...)`, because union conversions take no part in type inference;
a step that already returns a validation needs nothing.

### Async

```csharp
Validated<User, string> user = await Login(form.Login)
    .ThenAsync(login => repository.FindAsync(login));            // a dependent step that hits the database

string message = await user.MatchAsync(
    async found => await GreetAsync(found),
    errors => string.Join("; ", errors));                         // a synchronous branch is fine too

Validated<string, string> name = await user.MapAsync(found => LoadDisplayNameAsync(found));
```

Independent checks that need I/O combine with `Validated.CombineAsync`, the asynchronous `Combine`: the tasks run
concurrently and every error of every check comes back. Every input is a task, so a synchronous check goes in as
`Task.FromResult(...)` (C# cannot infer the types of a mix of plain validations and tasks). Often it reads better to
combine the synchronous checks on their own and join both parts with `Zip`:

```csharp
var local = Validated.Combine(Name(dto.Name), Age(dto.Age), (name, age) => (name, age));
var remote = await Validated.CombineAsync(
    EmailIsFree(dto.Email),                                    // Task<Validated<string, string>>
    LoginIsFree(dto.Login),
    (email, login) => (email, login));

Validated<User, string> user = local.Zip(remote, (l, r) => new User(l.name, l.age, r.email, r.login));
```

The async members are deliberately few. The delegates return `Task<T>`; the methods return `ValueTask<T>`, already
complete (and allocation-free) when the case passes through. Arguments are checked immediately; anything a delegate
throws comes out when the result is awaited, and an `OperationCanceledException` for a requested token gives a
canceled result.

### The uninitialized validation

`Validated<T, TError>` is a struct, so `default(Validated<T, TError>)` exists, for example in a new array. It is
neither case: a `switch` over both cases throws `SwitchExpressionException`, and every operation that reads it throws
`InvalidOperationException`. Equality, hashing and `ToString()` (`Uninitialized`) still work, and `validated is null`
is `true` only for an uninitialized validation. Always create validations with `valid` or `invalid`.

## Migrating from 0.5

1.0 is a new library with the same name, not an upgrade in place: 0.5 lives on in the
[`archive/0.5`](https://github.com/Frognar/dotValid/tree/archive/0.5) branch and its package. The main differences:

| 0.5 | 1.0 |
|---|---|
| `Validated<T>` with a fixed `ValidationError(PropertyName, Error)` | `Validated<T, TError>` with any error type; `ValidationError(Property, Message)` is ready-made |
| `Validation.Succeed(value)` | `valid(value)` |
| `Validation.Failed<T>(errors)` | `invalid(error)`, `invalid(first, second)`, `invalid([.. errors])` |
| `IsValid` | pattern matching: `validated is Valid<T>`, a `switch` over `Valid<T>` and `Invalid<TError>` |
| `Match(errors, value)`, errors as `IEnumerable` | `Match(valid, invalid)`: the value comes first, the errors are an `ImmutableArray` |
| curried `function.Apply(v1).Apply(v2)...` (up to 16) | `Validated.Combine(v1, ..., v8, combine)`, `Zip`, or `Apply` on a validated function |
| — | `Then`/`Ensure`, `Check`/`FromNullable`, collections, async |
| .NET Standard 2.0 | .NET 11 (C# 15) |

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
