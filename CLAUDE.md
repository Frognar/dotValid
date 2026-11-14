# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

dotValid 1.0: a single-package applicative `Validated<T, TError>` library for .NET 11 / C# 15, built on C# **union
types**. It is a ground-up rewrite (0.5 lives on `archive/0.5`). Its siblings dotResult 2.0 (`../dotResult`, the
template for tooling and process) and dotMaybe 2.0 share the same quality gates. The SDK is pinned in `global.json`
(11.0.100, `rollForward: latestFeature`); C# 15 is the default language version for `net11.0`.
Tests run on Microsoft Testing Platform (xUnit v3), so `dotnet test` takes `--project`/`--solution`.

The plan (decisions, milestones, STATUS) is tracked as GitHub milestones and issues: one issue per card, and the PR
that implements it says `Closes #n`.

## Commands

```bash
dotnet restore dotValid.slnx --locked-mode              # what CI does; lock files must be up to date
dotnet build dotValid.slnx
dotnet test --solution dotValid.slnx -c Release         # allocation (V9) properties are skipped in Debug
dotnet format whitespace --folder --verify-no-changes   # CI formatting check
dotnet build src/DotValid/DotValid.csproj -c Release -p:LangVersion=14   # CI: no union conversions in src (Stryker)

# Single class / method (xUnit v3 MTP filters, wildcards allowed)
dotnet test --project tests/DotValid.PropertyTests/DotValid.PropertyTests.csproj --filter-class "*MapProperties"

# Nightly profile: every property with 10 000 cases
dotnet build tests/DotValid.PropertyTests/DotValid.PropertyTests.csproj -c Release -p:PropertyTestProfile=Nightly
dotnet test --project tests/DotValid.PropertyTests/DotValid.PropertyTests.csproj -c Release --no-build

# After changing Directory.Packages.props or adding a project: refresh and commit packages.lock.json
dotnet restore dotValid.slnx --force-evaluate

# Record new public API in PublicAPI.Unshipped.txt (RS0016 otherwise fails the build)
dotnet format analyzers src/DotValid/DotValid.csproj --diagnostics RS0016 --severity warn

# Mutation testing (Stryker), benchmarks (manual only), Native AOT smoke test
dotnet tool restore && (cd tests/DotValid.PropertyTests && dotnet stryker)
dotnet run -c Release --project benchmarks/DotValid.Benchmarks -- --filter '*Combine*' --inProcess  # BDN 0.15.8 needs --inProcess on .NET 11
dotnet publish smoke/DotValid.AotSmoke/DotValid.AotSmoke.csproj -c Release -r linux-x64 -o artifacts/aot && ./artifacts/aot/DotValid.AotSmoke
git restore src/DotValid/packages.lock.json   # the AOT publish rewrites it for linux-x64; never commit that
```

## Architecture

`src/DotValid`: one `readonly partial struct Validated<T, TError>` split by concern, as in dotResult:
`Validated.cs` (the `[Union]` core with private case storage and an internal `ValidatedCase` tag, implementing the
nested `IUnionMembers` explicitly so it stays out of the public API), `Valid.cs` / `Invalid.cs` (the case types),
`Prelude.cs` (`valid`, `invalid`), and further partials as features land (`Validated.Combinators.cs`,
`Validated.Combine.cs`, `Validated.Static.cs`, `Validated.Collections.cs`, `Validated.Async.cs`, `ValidationError.cs`).

### Design invariants (enforced by tests and analyzers)

- **V1 Union:** exactly two cases, `Valid<T>` (`Value`) and `Invalid<TError>` (`Errors`, an `ImmutableArray<TError>`
  that is never empty); `T : notnull`, `TError : notnull`. No implicit conversion from a bare `T` or `TError`.
- **V2 Prelude:** `valid(value)`, `invalid(error)`, `invalid(params ReadOnlySpan<TError>)`; a list goes in as
  `invalid([.. errors])`. The static class `Validated` holds `Combine`, `CombineAsync`, `Check`, `FromNullable` and the
  collection functions.
- **V3 Uninitialized default:** `default(Validated<T, TError>)` is neither case. Consuming members throw
  `InvalidOperationException`, a `switch` throws `SwitchExpressionException`, observing members (`Equals`,
  `GetHashCode`, `ToString`, `is null`) do not throw. No operation produces it.
- **V4 No null:** `null` is never a value or an error, nor an element of the errors.
- **V5 No accessors:** no `IsValid`, `Value` or `Errors` on the validation itself.
- **V6 Vocabulary:** `Match(valid, invalid)`, `OrDefault`, `Map`, `MapError`, `Zip`, `Combine`, `Apply`, `Then`,
  `Ensure`, `Select`.
- **V7 Accumulation:** `Zip`, `Combine`, `Apply`, `Sequence` and `Traverse` evaluate every input and concatenate the
  errors in argument order; applicative laws are properties.
- **V8 Dependent steps:** `Then`/`Ensure` short-circuit; deliberately not called `Bind`, and no `SelectMany`.
- **V9 Zero allocation** on the valid path for value types in Release builds; an invalid validation allocates its
  errors (documented exclusion).
- **V10 Async, minimal:** `MatchAsync`, `MapAsync`, `ThenAsync`, and `Validated.CombineAsync` over 2–8
  `Task<Validated>` inputs (a synchronous one goes in as `Task.FromResult(x)`; mixing bare validations and tasks cannot
  infer types, see #16). `ValueTask` results with dotResult's R8 semantics. Split sync and async checks, then `Zip`.
- **V11 Equality:** values by `EqualityComparer<T>.Default`, errors as sequences; valid never equals invalid. No ordering.
- **V12 Collections** accumulate all errors. **V13** Small helpers (`Check`, `FromNullable`, `ValidationError`).
- **V14 Quality gates:** AOT/trimming clean, `AnalysisLevel latest-all` + `TreatWarningsAsErrors`, PublicAPI analyzer,
  locked restore, Stryker, CodeQL. No runtime dependencies (not even dotResult), no JSON.

### Rules learned in dotResult

- Library code builds validations with internal constructors, never with union conversions or patterns: Stryker's
  Roslyn treats unions as preview (CS8652); CI builds the library as C# 14 to catch them.
- Test types that hold validations in fields are `internal` (xUnit loads public types before Stryker swaps in the
  mutated library, and every test host crashes).
- Allocation tests use `Allocations.AssertNone` (retried up to three times; `AllocationsTests` is the canary) with
  `Skip = Allocations.SkipInDebug` and `static` lambdas.

### Tests

- `tests/DotValid.PropertyTests`: FsCheck properties; `ValidatedArbitraries` (valid/invalid 50/50, one to five errors,
  never null or default); the `Validations` helper builds and describes validations in generic code.
- `tests/DotValid.CompilerContractTests`: compiles consumer snippets with the SDK's own Roslyn and asserts on
  diagnostics. Run them first after bumping the SDK.
- Test method names are sentence-like with underscores.

## Workflow

- Squash-merged PRs; the PR title is a Conventional Commit and the description says `Closes #n`. Versions come from git
  tags via MinVer. Only three releases are planned (`1.0.0-alpha.1`, `1.0.0-beta.1`, `1.0.0`); release PRs are merged
  by the maintainer. To force a release without `feat:`/`fix:`/`perf:`, merge with a `Release-As:` footer
  (`gh pr merge --squash --body "Release-As: x.y.z"`).
- Cards are built test-first: failing tests plus `NotImplementedException` stubs, then implement until green.
