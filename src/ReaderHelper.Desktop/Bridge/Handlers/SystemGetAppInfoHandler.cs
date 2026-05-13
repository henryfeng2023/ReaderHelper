
using ReaderHelper.Desktop.Common.Contracts;

namespace ReaderHelper.Desktop.Bridge.Handlers;

public sealed class SystemGetAppInfoHandler : ICommandHandler
{
    private readonly IAppInfoService _appInfoService;

    public SystemGetAppInfoHandler(IAppInfoService appInfoService)
    {
        _appInfoService = appInfoService;
    }

    public string Method => "system.getAppInfo";

    public Task<object?> HandleAsync(BridgeRequest request)
    {
        return Task.FromResult<object?>(_appInfoService.GetAppInfo());
    }
}
