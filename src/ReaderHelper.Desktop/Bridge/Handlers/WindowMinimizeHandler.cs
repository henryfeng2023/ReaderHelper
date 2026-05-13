using ReaderHelper.Desktop.Common.Contracts;

namespace ReaderHelper.Desktop.Bridge.Handlers;

public sealed class WindowMinimizeHandler : ICommandHandler
{
    private readonly IWindowControlService _windowControlService;

    public WindowMinimizeHandler(IWindowControlService windowControlService)
    {
        _windowControlService = windowControlService;
    }

    public string Method => "window.minimize";

    public Task<object?> HandleAsync(BridgeRequest request)
    {
        return Task.FromResult<object?>(_windowControlService.Minimize());
    }
}
