using System.Text.Json;

namespace BrewUp.Shared.CustomTypes;

public sealed record McpResponseEnvelope(
    string Jsonrpc,
    string Id,
    JsonElement? Result,
    McpError? Error);