namespace BrewUp.Shared.CustomTypes;

public sealed record McpError(
    int Code,
    string Message);