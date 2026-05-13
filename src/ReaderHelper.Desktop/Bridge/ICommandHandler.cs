namespace ReaderHelper.Desktop.Bridge;

public interface ICommandHandler
{
    string Method { get; }

    Task<object?> HandleAsync(BridgeRequest request);
}
