namespace ReaderHelper.Desktop.Bridge;

public sealed class DesktopBridgeDispatcher
{
    private readonly HandlerCollection _handlers;

    public DesktopBridgeDispatcher(HandlerCollection handlers)
    {
        _handlers = handlers;
    }

    public async Task<BridgeResponse> DispatchAsync(BridgeRequest request)
    {
        if (!_handlers.TryGet(request.Method, out var handler) || handler is null)
        {
            return BridgeResponse.Error(request.Id, $"Unsupported method: {request.Method}");
        }

        var data = await handler.HandleAsync(request);
        return BridgeResponse.Ok(request.Id, data);
    }
}
