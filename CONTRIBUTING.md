# Contributing

Bug reports and pull requests are welcome.

## Development setup

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download), [Rider](https://www.jetbrains.com/rider/) or any editor with C# support.

```bash
git clone git@github.com:Duracell1989/RomForge.git
cd RomForge
make build    # build solution
make test     # run all tests
make check    # clean + build + test
make run      # launch the UI
```

## Code style

- C# 13 / .NET 10; `<Nullable>enable</Nullable>` everywhere; zero warnings
- Private fields: `_camelCase`; no primary constructors; no implicit usings
- `string.Empty` not `""`; `new TypeName(...)` not target-typed `new()`
- `async` methods always suffixed `Async`
- Tests: NUnit 4 + AwesomeAssertions; naming `MethodName_Scenario_ExpectedResult`
- One test class per source class, `[TestOf(typeof(X))]` on every test class

All PRs must pass CI (build + tests, ≥60% line coverage on `RomForge.Core`).
