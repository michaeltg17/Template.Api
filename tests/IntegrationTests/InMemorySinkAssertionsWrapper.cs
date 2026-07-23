using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Events;
using Serilog.Parsing;

namespace IntegrationTests;

/// <summary>
/// Simple assertion helper for partitioned log event lists.
/// Replaces the InMemorySink assertion API with a simpler fluent interface
/// that works against a test-scoped log list, enabling parallel-safe assertions.
/// </summary>
internal static class LogEventAssertions
{
    /// <summary>
    /// Asserts that exactly one event matches the message template, optionally filtered by level and property.
    /// </summary>
    public static void HaveExactlyOneMessage(this List<LogEvent> events,
        string messageTemplate, LogEventLevel? level = null, string? propertyName = null, object? expectedPropertyValue = null)
    {
        var matches = events.Where(e => MatchesTemplate(e, messageTemplate)).ToList();

        if (level.HasValue)
            matches = matches.Where(e => e.Level == level.Value).ToList();

        var count = matches.Count;
        if (count == 0)
        {
            var actualText = events.Count > 0 ?
                "Actual events: [" + string.Join(", ", events.Select(e => $"[{e.Level}] {e.MessageTemplate.Text}")) + "]" : "";
            throw new Exception($"Expected message \"{messageTemplate}\" to be logged, but no matching log events were found. Total events: {events.Count}. {actualText}");
        }

        if (count > 1)
        {
            throw new Exception($"Expected message \"{messageTemplate}\" to appear once but found {count} times: " +
                string.Join("; ", matches.Select(e => $"[{e.Level}] {e.MessageTemplate.Text}")));
        }

        var @event = matches[0];

        if (propertyName != null)
        {
            if (!@event.Properties.TryGetValue(propertyName, out var prop))
            {
                throw new Exception($"Expected property \"{propertyName}\" on log event but it was not found. " +
                    $"Available properties: {string.Join(", ", @event.Properties.Keys)}");
            }

            if (expectedPropertyValue != null)
            {
                var actual = prop is ScalarValue scalar ? scalar.Value.ToString() : prop.ToString();
                var expected = expectedPropertyValue.ToString();
                if (actual != expected)
                {
                    throw new Exception($"Expected property \"{propertyName}\" to have value {expected} but found {actual}");
                }
            }
        }
    }

    private static bool MatchesTemplate(LogEvent logEvent, string messageTemplate)
    {
        var template = new MessageTemplateParser().Parse(messageTemplate);
        if (template.Text != logEvent.MessageTemplate.Text)
            return false;
        foreach (var token in template.Tokens)
        {
            if (token is PropertyToken propToken)
            {
                if (!logEvent.Properties.ContainsKey(propToken.PropertyName))
                    return false;
            }
        }
        return true;
    }
}