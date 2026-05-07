# Architecture

## Positioning

This solution is for a Windows-first hybrid application where the desktop process is the owner of native capability and the web frontend is the owner of presentation and workflow.

The host is `WPF`.
The browser engine is `Chromium` through `CEF/CefSharp`.
The frontend can run in two modes:

- Browser mode
- Desktop shell mode

## Why this split

The UI must remain portable and web-friendly.
The native capability must remain explicit, versioned, and testable.
That means the frontend should not call arbitrary host objects directly throughout the codebase.

Use a thin adapter:

- Frontend depends on `DesktopApi`
- Desktop shell implements the real adapter
- Browser mode uses a no-op or fallback adapter

## Boundary rules

Keep the JS bridge small.
It should expose only thin methods such as:

- `getRuntimeInfo`
- `openExternal`
- `selectFiles`
- `startJob`

Move heavy or long-running work out of the JS bridge and into a local service exposed by:

- `NamedPipe`
- `gRPC`
- `localhost HTTP`

Recommended rule:

- Synchronous or tiny calls: JS bridge
- Streaming, progress, file transfer, indexing, OCR, sync jobs: local service

## Service split

The browser bridge should answer questions like:

- Am I running in desktop shell mode
- What is the local API base URL
- Open a browser or shell target
- Open a native picker
- Read or write the clipboard
- Save small user-driven exports
- Read and update application settings

The local API should handle:

- File upload or export
- Search indexing
- OCR
- Sync pipelines
- Background jobs with progress and cancellation

## Deployment modes

### Browser mode

- Frontend runs in Chrome/Edge
- `DesktopApi` falls back to browser-safe behavior
- Native features are unavailable or proxied to backend services

### Desktop shell mode

- `ReaderHelper.Desktop` hosts the frontend in Chromium
- The shell registers a `desktop` bridge object
- The shell can start the local worker process
- The packaged frontend assets are emitted into `src/ReaderHelper.Desktop/www`
- The frontend resolves the bridge at startup and swaps in the desktop implementation

## Initial call chain

1. WPF starts the CEF runtime.
2. Main window creates `ChromiumWebBrowser`.
3. The desktop shell registers the `desktop` bridge object.
4. Frontend executes `CefSharp.BindObjectAsync("desktop")`.
5. Frontend calls typed methods through `DesktopApi`.
6. Frontend calls the local service through `LocalApiClient` for heavy operations.

## Next architectural step

After the shell bootstrap is stable, let the desktop process own the lifecycle of the local service:

- reserve a localhost port
- start the worker process
- pass the base URL into the bridge
- stop the worker when the shell exits

The sample local API in this repository already models:

- health endpoint
- runtime endpoint
- job start endpoint
- job status endpoint
