using System.Text.Json.Serialization;

namespace ReaderHelper.Desktop.Bridge;

public sealed record BridgeResponse(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("success")] bool Success,
    [property: JsonPropertyName("data")] object? Data,
    [property: JsonPropertyName("errorMsg")] string? ErrorMsg)
{
    public static BridgeResponse Ok(string id, object? data) => new(id, true, data, null);

    public static BridgeResponse Error(string id, string error) => new(id, false, null, error);
}
