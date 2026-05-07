using ReaderHelper.Contracts.LocalApi;
using ReaderHelper.LocalApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls(LocalApiEnvironment.ResolveListenUrl());
builder.Services.AddSingleton<JobStore>();

var app = builder.Build();
var startedAt = DateTimeOffset.UtcNow;

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    service = "ReaderHelper.LocalApi"
}));

app.MapGet("/api/runtime", () => Results.Ok(new LocalApiRuntimeInfo
{
    ServiceName = "ReaderHelper.LocalApi",
    ServiceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "0.0.0",
    Mode = "desktop-local-service",
    StartedAt = startedAt
}));

app.MapPost("/api/jobs", (JobStartRequest request, JobStore store) =>
{
    var created = store.Create(request.JobType, request.Payload);
    return Results.Ok(created);
});

app.MapGet("/api/jobs/{jobId}", (string jobId, JobStore store) =>
{
    var job = store.TryGet(jobId);
    return job is null ? Results.NotFound() : Results.Ok(job);
});

app.Run();
