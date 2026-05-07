namespace ReaderHelper.Contracts.LocalApi;

public sealed class LocalApiRuntimeInfo
{
    public required string ServiceName { get; init; }

    public required string ServiceVersion { get; init; }

    public required string Mode { get; init; }

    public required DateTimeOffset StartedAt { get; init; }
}
