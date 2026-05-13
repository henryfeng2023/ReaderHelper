using System.Text.Json;
using ReaderHelper.Desktop.Common.Contracts;
using ReaderHelper.Desktop.Common.Models;

namespace ReaderHelper.Desktop.Bridge.Handlers;

public sealed class ShellOpenExternalHandler : ICommandHandler
{
    private readonly IExternalLauncher _externalLauncher;

    public ShellOpenExternalHandler(IExternalLauncher externalLauncher)
    {
        _externalLauncher = externalLauncher;
    }

    public string Method => "shell.openExternal";

    public Task<object?> HandleAsync(BridgeRequest request)
    {
        var args = request.Args.Deserialize<OpenExternalArgs>(BridgeJson.Options);
        if (args is null)
        {
            throw new InvalidOperationException("Missing url.");
        }

        var result = _externalLauncher.OpenUri(new OpenExternalRequest(args.Url ?? string.Empty));
        return Task.FromResult<object?>(result);
    }

    private sealed record OpenExternalArgs
    {
        public string? Url { get; init; }
    }
}
