namespace ReaderHelper.Contracts.LocalApi;

public sealed class JobStatusResponse
{
    public required string JobId { get; init; }

    public required string JobType { get; init; }

    public required string Status { get; init; }

    public int ProgressPercent { get; init; }

    public string? Result { get; init; }
}
