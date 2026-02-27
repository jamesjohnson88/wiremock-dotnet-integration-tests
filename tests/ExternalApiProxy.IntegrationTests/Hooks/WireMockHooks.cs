using ExternalApiProxy.IntegrationTests.Support;
using NUnit.Framework;
using Reqnroll;
using Reqnroll.BoDi;
using WireMock.Server;
using WireMock.Settings;

namespace ExternalApiProxy.IntegrationTests.Hooks;

/// <summary>
/// Reqnroll hooks that manage the WireMock server lifecycle for each scenario.
///
/// A fresh WireMock server is started before each scenario and stopped after,
/// printing a detailed log of every WireMock request handled during the scenario.
/// This makes it easy to diagnose stub mismatches because developers can see
/// exactly which requests were matched and which were not.
/// </summary>
[Binding]
public sealed class WireMockHooks(IObjectContainer container)
{
    [BeforeScenario(Order = 0)]
    public void StartWireMock()
    {
        var logger = new WireMockLogger();

        var server = WireMockServer.Start(new WireMockServerSettings
        {
            Logger = logger,
            StartAdminInterface = false
        });

        container.RegisterInstanceAs(server);
        container.RegisterInstanceAs(logger);
    }

    [AfterScenario(Order = 0)]
    public void StopWireMock()
    {
        var server = container.Resolve<WireMockServer>();
        var logger = container.Resolve<WireMockLogger>();

        PrintRequestLog(logger);
        PrintRequestSummary(server);

        server.Stop();
        server.Dispose();
    }

    private static void PrintRequestLog(WireMockLogger logger)
    {
        var logs = logger.FlushRequestLogs();

        if (logs.Count == 0)
            return;

        TestContext.Out.WriteLine(string.Empty);
        TestContext.Out.WriteLine("── WireMock Request Log ───────────────────────────────────────");
        foreach (var entry in logs)
            TestContext.Out.WriteLine(entry);
        TestContext.Out.WriteLine("───────────────────────────────────────────────────────────────");
    }

    private static void PrintRequestSummary(WireMockServer server)
    {
        var entries = server.LogEntries.ToList();

        if (entries.Count == 0)
        {
            TestContext.Out.WriteLine("[WireMock] No requests were received during this scenario.");
            return;
        }

        var matched = entries.Count(e => e.MappingGuid.HasValue);
        var unmatched = entries.Count - matched;

        TestContext.Out.WriteLine(string.Empty);
        TestContext.Out.WriteLine("┌─ WireMock Request Summary ───────────────────────────────────┐");
        TestContext.Out.WriteLine($"│  Total requests : {entries.Count,-45}│");
        TestContext.Out.WriteLine($"│  Matched        : {matched,-45}│");
        TestContext.Out.WriteLine($"│  Unmatched      : {unmatched,-45}│");
        TestContext.Out.WriteLine("└──────────────────────────────────────────────────────────────┘");

        if (unmatched > 0)
        {
            TestContext.Out.WriteLine(string.Empty);
            TestContext.Out.WriteLine("[WireMock] ⚠ Unmatched requests — no stub was found for these:");
            foreach (var entry in entries.Where(e => !e.MappingGuid.HasValue))
            {
                TestContext.Out.WriteLine(
                    $"  ✗  {entry.RequestMessage.Method} {entry.RequestMessage.AbsoluteUrl}");
            }
        }
    }
}
