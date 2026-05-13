using ReaderHelper.Desktop.Common.Contracts;

namespace ReaderHelper.Desktop.Bridge.Handlers;

public sealed class WindowCloseHandler : ICommandHandler
{
    private readonly IWindowControlService _windowControlService;

    public WindowCloseHandler(IWindowControlService windowControlService)
    {
        _windowControlService = windowControlService;
    }

    public string Method => "window.close";

    public Task<object?> HandleAsync(BridgeRequest request)
    {
        return Task.FromResult<object?>(_windowControlService.RequestClose());
    }
}
