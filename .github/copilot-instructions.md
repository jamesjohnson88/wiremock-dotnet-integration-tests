# Copilot Instructions

## Project Overview
This repository demonstrates how to write .NET integration tests using [WireMock.Net](https://github.com/WireMock-Net/WireMock.Net) to mock HTTP dependencies. It serves as a reference implementation and learning resource.

## Tech Stack
- Language: C#
- Framework: .NET 9
- Testing: WireMock.Net for HTTP stubbing/mocking in integration tests
- Test runner: NUnit

## Build & Test
- Build the solution: `dotnet build`
- Run all tests: `dotnet test`
- Restore NuGet packages: `dotnet restore`

## Coding Conventions
- Follow standard C# naming conventions (PascalCase for types and methods, camelCase for local variables)
- Use `var` where the type is obvious from the right-hand side
- Prefer `async`/`await` over blocking calls (`.Result`, `.Wait()`) in test code
- Dispose WireMock server instances after each test or test class (use `IDisposable` or `IAsyncLifetime`)

## WireMock Usage
- Start a `WireMockServer` per test class (or per test if isolation is required)
- Configure stubs using `.Given(...).RespondWith(...)` fluent API
- Tear down the server in `Dispose` / `DisposeAsync` to free the port
- Prefer specific matchers (exact URL, method, headers) over wildcards to keep tests deterministic

## Workflow
- Create feature branches from `main`
- Use clear, descriptive commit messages
- Open a pull request to `main` for review before merging
- Ensure all tests pass locally (`dotnet test`) before opening a PR
