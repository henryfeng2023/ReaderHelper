# Local API Sequence

## Target

Use the browser bridge for capability discovery and the localhost API for real work.

## Startup sequence

1. Desktop shell starts.
2. Desktop shell resolves `READERHELPER_LOCAL_API_URL`.
3. Desktop shell starts `ReaderHelper.LocalApi` if `READERHELPER_LOCAL_API_EXE` or the default worker path is available.
4. Desktop shell injects the local API base URL into `DesktopRuntimeInfo`.
5. Frontend creates `DesktopApi`.
6. Frontend creates `LocalApiClient` from the runtime info.

## Request sequence

1. Frontend calls `localApi.startJob({ jobType, payload })`.
2. Local API returns `jobId`.
3. Frontend polls `GET /api/jobs/{jobId}`.
4. UI renders progress and result.

## Why this matters

This keeps the host boundary stable:

- the browser bridge stays tiny
- heavy operations are HTTP-friendly
- browser mode can mock the same API shape
