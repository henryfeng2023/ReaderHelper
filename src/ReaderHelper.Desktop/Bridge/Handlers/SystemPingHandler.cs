using ReaderHelper.Desktop.Common.Contracts;

namespace ReaderHelper.Desktop.Bridge.Handlers;

public sealed class SystemPingHandler : ICommandHandler
{
    private readonly IAppInfoService _appInfoService;

    public SystemPingHandler(IAppInfoService appInfoService)
    {
        _appInfoService = appInfoService;
    }

    public string Method => "system.ping";

    public Task<object?> HandleAsync(BridgeRequest request)
    {
        return Task.FromResult<object?>(_appInfoService.Ping());
    }
}
