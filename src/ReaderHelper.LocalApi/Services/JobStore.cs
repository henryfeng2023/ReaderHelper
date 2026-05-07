using System.Collections.Concurrent;
using ReaderHelper.Contracts.LocalApi;

namespace ReaderHelper.LocalApi.Services;

public sealed class JobStore
{
    private readonly ConcurrentDictionary<string, JobStatusResponse> _jobs = new();

    public JobStartResponse Create(string jobType, string? payload)
    {
        var jobId = Guid.NewGuid().ToString("N");

        var job = new JobStatusResponse
        {
            JobId = jobId,
            JobType = jobType,
            Status = "completed",
            ProgressPercent = 100,
            Result = payload is null
                ? $"Job {jobType} completed."
                : $"Job {jobType} completed with payload: {payload}"
        };

        _jobs[jobId] = job;

        return new JobStartResponse
        {
            JobId = jobId,
            Status = "accepted"
        };
    }

    public JobStatusResponse? TryGet(string jobId)
    {
        return _jobs.TryGetValue(jobId, out var job) ? job : null;
    }
}
