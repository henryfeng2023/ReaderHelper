using System.Text.Json;

namespace ReaderHelper.Desktop.Bridge;

public static class BridgeJson
{
    public static JsonSerializerOptions Options { get; } = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}
