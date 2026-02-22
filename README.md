# wiremock-dotnet-integration-tests

A testbed for exploring developer-friendly logging when using **WireMock.Net** in .NET integration tests.

The goal is to find patterns that make it easy for a developer to understand exactly what happened whenever a mocked HTTP server request doesn't go as expected — logged mapped requests, unmatched misses, closest partial matches, and a per-scenario request summary.

---

## Projects

### `src/ExternalApiProxy` — .NET 10 Web API

A minimal ASP.NET Core Web API that proxies requests to the public [JSONPlaceholder](https://jsonplaceholder.typicode.com) REST API.

| Endpoint | Upstream call |
|---|---|
| `GET /api/todos/{id}` | `GET https://jsonplaceholder.typicode.com/todos/{id}` |
| `GET /api/posts/{id}` | `GET https://jsonplaceholder.typicode.com/posts/{id}` |
| `GET /api/users/{id}` | `GET https://jsonplaceholder.typicode.com/users/{id}` |

The upstream base URL is read from configuration (`ExternalApis:JsonPlaceholder:BaseUrl`) so integration tests can swap it for a local WireMock server.

### `tests/ExternalApiProxy.IntegrationTests` — Integration Tests

BDD-style integration tests built with:

| Library | Role |
|---|---|
| [Reqnroll](https://reqnroll.net/) | BDD test runner (Gherkin feature files → NUnit tests) |
| [BoDi](https://github.com/reqnroll/BoDi) | Dependency injection inside Reqnroll bindings |
| [WireMock.Net](https://wiremock.org/docs/solutions/dotnet/) | Lightweight HTTP stub server |
| [FluentAssertions](https://fluentassertions.com/) | Expressive assertion library |
| [Microsoft.AspNetCore.Mvc.Testing](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests) | In-process test host |

---

## Developer Experience / WireMock Logging

The key focus of this repo is producing useful output when something goes wrong with a WireMock stub.

### What you see in test output

After every scenario the test runner prints a **WireMock Request Log** and a **Summary**:

**Happy path (all requests matched):**

```
── WireMock Request Log ───────────────────────────────────────
  [✓] GET /todos/1 → 200 (mapping: "GET /todos/1")
───────────────────────────────────────────────────────────────

┌─ WireMock Request Summary ───────────────────────────────────┐
│  Total requests : 1                                            │
│  Matched        : 1                                            │
│  Unmatched      : 0                                            │
└──────────────────────────────────────────────────────────────┘
```

**Unmatched request (stub was misconfigured or missing):**

```
── WireMock Request Log ───────────────────────────────────────
  [✗] GET /todos/1 → 404  ⚠ NO MATCHING STUB
      URL    : http://localhost:54321/todos/1
      Body   : <empty>
      Closest: "GET /todo/1" (closest partial match)
───────────────────────────────────────────────────────────────

┌─ WireMock Request Summary ───────────────────────────────────┐
│  Total requests : 1                                            │
│  Matched        : 0                                            │
│  Unmatched      : 1                                            │
└──────────────────────────────────────────────────────────────┘

[WireMock] ⚠ Unmatched requests — no stub was found for these:
  ✗  GET http://localhost:54321/todos/1
```

### How it works

- `NUnitWireMockLogger` implements `IWireMockLogger` and collects per-request entries in a thread-safe queue (WireMock processes requests on its own HTTP listener thread, separate from the NUnit test thread).
- `WireMockHooks` (Reqnroll `[BeforeScenario]`/`[AfterScenario]`) starts a fresh WireMock server for each scenario and flushes the log queue to `TestContext.Out` after the scenario completes.
- `WebApplicationHooks` creates a `TestWebApplicationFactory` that overrides `ExternalApis:JsonPlaceholder:BaseUrl` to point at the WireMock server's URL.

---

## Running the tests

```bash
dotnet test
```

Or to run a specific feature:

```bash
dotnet test --filter "FullyQualifiedName~Todos"
```
