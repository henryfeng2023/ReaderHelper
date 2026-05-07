namespace ReaderHelper.Contracts.DesktopInterop;

public sealed class DesktopOperationResult
{
    public bool Success { get; init; }

    public string? ErrorMessage { get; init; }

    public static DesktopOperationResult Ok()
    {
        return new DesktopOperationResult
        {
            Success = true
        };
    }

    public static DesktopOperationResult Fail(string message)
    {
        return new DesktopOperationResult
        {
            Success = false,
            ErrorMessage = message
        };
    }
}
