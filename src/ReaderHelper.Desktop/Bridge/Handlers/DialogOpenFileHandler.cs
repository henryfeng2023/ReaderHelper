using System.Text.Json;
using ReaderHelper.Desktop.Common.Contracts;
using ReaderHelper.Desktop.Common.Models;
using ReaderHelper.Desktop.Bridge;

namespace ReaderHelper.Desktop.Bridge.Handlers;

public sealed class DialogOpenFileHandler : ICommandHandler
{
    private readonly IFileDialogService _fileDialogService;

    public DialogOpenFileHandler(IFileDialogService fileDialogService)
    {
        _fileDialogService = fileDialogService;
    }

    public string Method => "dialog.openFile";

    public Task<object?> HandleAsync(BridgeRequest request)
    {
        var args = request.Args.Deserialize<OpenFileArgs>(BridgeJson.Options) ?? new OpenFileArgs();
        var result = _fileDialogService.OpenFile(new OpenFileRequest(args.Filter, args.Multiselect));
        return Task.FromResult<object?>(result);
    }

    private sealed record OpenFileArgs
    {
        public string? Filter { get; init; }

        public bool Multiselect { get; init; }
    }
}
