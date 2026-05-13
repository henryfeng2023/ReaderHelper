using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReaderHelper.Desktop.Bridge;

public sealed record BridgeRequest(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("method")] string Method,
    [property: JsonPropertyName("args")] JsonElement Args);
