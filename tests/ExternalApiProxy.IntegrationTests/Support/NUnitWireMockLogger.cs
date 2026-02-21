using System.Collections.Concurrent;
using NUnit.Framework;
using WireMock.Admin.Requests;
using WireMock.Logging;

namespace ExternalApiProxy.IntegrationTests.Support;

/// <summary>
/// A WireMock logger that:
/// - Immediately writes server lifecycle messages (startup, errors, etc.) to NUnit
///   test output from the test thread.
/// - Queues per-request log messages in a thread-safe collection because
///   <see cref="DebugRequestResponse"/> is invoked on WireMock's own HTTP server
///   thread, where NUnit's output context is not active.
/// - Exposes <see cref="FlushRequestLogs"/> so the AfterScenario hook can drain
///   the queue onto the test thread and write everything to TestContext.Out.
/// </summary>
public sealed class NUnitWireMockLogger : IWireMockLogger
{
    private readonly ConcurrentQueue<string> _requestLogs = new();

    // ── Server lifecycle messages ─────────────────────────────────────────

    public void Debug(string formatString, params object[] args) =>
        WriteImmediate("DEBUG", formatString, args);

    public void Info(string formatString, params object[] args) =>
        WriteImmediate("INFO", formatString, args);

    public void Warn(string formatString, params object[] args) =>
        WriteImmediate("WARN", formatString, args);

    public void Error(string formatString, params object[] args) =>
        WriteImmediate("ERROR", formatString, args);

    public void Error(string message, Exception exception) =>
        TestContext.Out.WriteLine($"[WireMock ERROR] {message}: {exception}");

    // ── Per-request logging ───────────────────────────────────────────────

    /// <summary>
    /// Called by WireMock on its own HTTP server thread for every handled request.
    /// Output is queued and flushed later via <see cref="FlushRequestLogs"/>.
    /// </summary>
    public void DebugRequestResponse(LogEntryModel logEntryModel, bool isAdminRequest)
    {
        if (isAdminRequest)
            return;

        var request = logEntryModel.Request;
        var response = logEntryModel.Response;
        var isMatched = logEntryModel.MappingGuid.HasValue;

        if (isMatched)
        {
            var mappingLabel = logEntryModel.MappingTitle ?? logEntryModel.MappingGuid.ToString();
            _requestLogs.Enqueue(
                $"  [✓] {request.Method} {request.AbsolutePath} " +
                $"→ {response.StatusCode} (mapping: \"{mappingLabel}\")");
        }
        else
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"  [✗] {request.Method} {request.AbsolutePath} → {response.StatusCode}  ⚠ NO MATCHING STUB");
            sb.AppendLine($"      URL    : {request.AbsoluteUrl}");
            sb.AppendLine($"      Body   : {request.Body ?? "<empty>"}");

            if (logEntryModel.PartialMappingGuid.HasValue)
            {
                var partialLabel = logEntryModel.PartialMappingTitle ?? logEntryModel.PartialMappingGuid.ToString();
                sb.AppendLine($"      Closest: \"{partialLabel}\" (closest partial match)");
            }

            _requestLogs.Enqueue(sb.ToString().TrimEnd());
        }
    }

    // ── Flush ─────────────────────────────────────────────────────────────

    /// <summary>
    /// Drains the queued request log lines. Call this from the test thread
    /// (e.g. AfterScenario hook) to write the messages into NUnit's output.
    /// </summary>
    public IReadOnlyList<string> FlushRequestLogs()
    {
        var logs = new List<string>();
        while (_requestLogs.TryDequeue(out var log))
            logs.Add(log);
        return logs;
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    private static void WriteImmediate(string level, string formatString, object[] args)
    {
        var message = args.Length > 0
            ? string.Format(formatString, args)
            : formatString;

        TestContext.Out.WriteLine($"[WireMock {level}] {message}");
    }
}

