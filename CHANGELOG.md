# Changelog

## [1.0.0](https://github.com/Frognar/dotValid/compare/v1.0.0-beta.1...v1.0.0) (2026-11-14)

### Highlights

A ground-up rewrite for .NET 11 and C# 15 unions. The 0.5 line is retired; the README has a migration table.

- `Validated<T, TError>` is a C# 15 union with exactly two cases, `Valid<T>` and `Invalid<TError>`: `valid(...)` and
  `invalid(...)` convert implicitly, a `switch` over both is exhaustive, and an invalid validation is never empty.
- Every error, not just the first: `Zip`, `Combine` (2–8), `Apply`, `Sequence`, `Traverse` and `CombineAsync` read every
  input and collect every error in order. The applicative laws are properties.
- Dependent steps when they are needed: `Then` and `Ensure` short-circuit on purpose (no `Bind`, no `SelectMany`).
- Any type can be the error; `ValidationError(Property, Message)`, `Validated.Check` and `Validated.FromNullable` cover
  the common cases. No `.Value` or `IsValid`: values come out through pattern matching, `Match` or `OrDefault`.
- Async where it pays off: `MatchAsync`, `MapAsync`, `ThenAsync`, and `CombineAsync` for checks that run as tasks.
- Zero allocations on the valid path for value types (checked on every build); trimming and Native AOT compatible; no
  dependencies. Requires .NET 11.

### Build System

* prepare the 1.0.0 release ([#43](https://github.com/Frognar/dotValid/issues/43)) ([c8853e8](https://github.com/Frognar/dotValid/commit/c8853e82f4aacabad499c41953ab339725da10b6))

## [1.0.0-beta.1](https://github.com/Frognar/dotValid/compare/v1.0.0-alpha.1...v1.0.0-beta.1) (2026-11-14)


### Features

* add Check, FromNullable and ValidationError ([#33](https://github.com/Frognar/dotValid/issues/33)) ([fad47b6](https://github.com/Frognar/dotValid/commit/fad47b6dfd613660b13600d7e4b33d21de660704))
* add CombineAsync ([#37](https://github.com/Frognar/dotValid/issues/37)) ([e4e92e2](https://github.com/Frognar/dotValid/commit/e4e92e2e4a98cb3659b4425002d1e03d598810a2))
* add MatchAsync, MapAsync and ThenAsync ([#36](https://github.com/Frognar/dotValid/issues/36)) ([4395635](https://github.com/Frognar/dotValid/commit/43956355e7cd500372179ea7a03dace78fe3d34e))
* add Sequence, Traverse and Partition ([#35](https://github.com/Frognar/dotValid/issues/35)) ([4cdac18](https://github.com/Frognar/dotValid/commit/4cdac1867070713f9064f9540771b51b77d5786d))


### Miscellaneous Chores

* start beta releases ([#38](https://github.com/Frognar/dotValid/issues/38)) ([935953f](https://github.com/Frognar/dotValid/commit/935953ffb19c12187d91d0072235d30d7c0936e1))

## [1.0.0-alpha.1](https://github.com/Frognar/dotValid/compare/v1.0.0-alpha.0...v1.0.0-alpha.1) (2026-11-14)


### Features

* add Combine for up to eight validations ([#32](https://github.com/Frognar/dotValid/issues/32)) ([6d2baa8](https://github.com/Frognar/dotValid/commit/6d2baa85ae89972689c795f426ac54ed2a75815d))
* add Map and MapError ([#29](https://github.com/Frognar/dotValid/issues/29)) ([11da356](https://github.com/Frognar/dotValid/commit/11da35683d0572509c44a4b43223d377599cbd66))
* add Match and OrDefault ([#28](https://github.com/Frognar/dotValid/issues/28)) ([62710c1](https://github.com/Frognar/dotValid/commit/62710c10f24a2f81c0d4bf227e3d45f3000d30b7))
* add the Validated&lt;T, TError&gt; union with Valid and Invalid cases ([#25](https://github.com/Frognar/dotValid/issues/25)) ([59687d1](https://github.com/Frognar/dotValid/commit/59687d1ff0ec04836b5d94f6ada310acc74d86a5))
* add Then, Ensure and select ([#30](https://github.com/Frognar/dotValid/issues/30)) ([49e0101](https://github.com/Frognar/dotValid/commit/49e010116bececf8673d3edd5cffcb3a07435b5b))
* add ToString and debugger display for validations ([#27](https://github.com/Frognar/dotValid/issues/27)) ([39be4cb](https://github.com/Frognar/dotValid/commit/39be4cb8042799230cd4273011dbcdb905b59ad0))
* add Zip and Apply ([#31](https://github.com/Frognar/dotValid/issues/31)) ([9c661b5](https://github.com/Frognar/dotValid/commit/9c661b5e121130d7452cbb0bbae9f7a03cdc0d0c))

## Changelog

This file is maintained by [release-please](https://github.com/googleapis/release-please) from Conventional Commits.
