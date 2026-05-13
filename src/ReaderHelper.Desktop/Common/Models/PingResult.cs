namespace ReaderHelper.Desktop.Common.Models;

public sealed record PingResult(
    string Message,
    DateTimeOffset ServerTime);
