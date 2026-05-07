namespace ReaderHelper.Contracts.LocalApi;

public sealed class JobStartResponse
{
    public required string JobId { get; init; }

    public required string Status { get; init; }
}
