namespace ReaderHelper.Desktop.Bridge;

public sealed class HandlerCollection
{
    private readonly Dictionary<string, ICommandHandler> _handlers;

    public HandlerCollection(IEnumerable<ICommandHandler> handlers)
    {
        _handlers = handlers.ToDictionary(static handler => handler.Method, StringComparer.Ordinal);
    }

    public bool TryGet(string method, out ICommandHandler? handler)
    {
        return _handlers.TryGetValue(method, out handler);
    }
}
