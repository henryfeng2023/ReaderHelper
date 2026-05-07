namespace ReaderHelper.Contracts.LocalApi;

public sealed class JobStartRequest
{
    public required string JobType { get; init; }

    public string? Payload { get; init; }
}
