# ReaderHelper

Windows hybrid shell architecture based on `.NET + WPF + CefSharp`.

## Goal

- Desktop shell wraps the web frontend.
- Web code can call desktop capabilities.
- The embedded browser uses Chromium through CEF instead of the system WebView.

## Projects

- `src/ReaderHelper.Contracts`
  Shared DTOs used by the desktop shell and the web-facing bridge.
- `src/ReaderHelper.Desktop`
  WPF desktop host with `CefSharp.Wpf.NETCore`.
- `src/ReaderHelper.LocalApi`
  Local `ASP.NET Core` process for heavy or long-running native tasks.
- `web`
  `Vite + React + TypeScript` frontend shell with browser and desktop modes.
- `docs/architecture.md`
  Architecture notes and boundary rules.

## Runtime model

- Local development:
  Set `READERHELPER_START_URL=http://localhost:5173` and let the desktop shell load the web dev server.
- Desktop packaged mode:
  The shell falls back to `src/ReaderHelper.Desktop/www/index.html`.
- Local service mode:
  Set `READERHELPER_LOCAL_API_URL=http://127.0.0.1:5057` or keep the default.
  If the shell should start the worker automatically, set `READERHELPER_LOCAL_API_EXE` to the built `ReaderHelper.LocalApi` executable.

## Notes

- This repository was scaffolded manually in the current environment.
- Build and restore were not validated here because `dotnet` is unavailable in the workspace shell.
- The desktop project references `CefSharp.Wpf.NETCore` `146.0.100`, which is listed on NuGet for `.NET 6+`.
- The local API project is a minimal in-memory sample and is intended as the second channel for heavy operations.
- The desktop bridge currently includes native file picker, clipboard, file save, and app settings capabilities for the frontend.

## Frontend commands

Inside `web`:

- `npm install`
- `npm run dev`
- `npm run build`

For desktop development, start the frontend dev server first and then launch the desktop shell with:

- `READERHELPER_START_URL=http://localhost:5173`

For desktop packaged mode, run:

- `cd web`
- `npm run build`

The frontend build output is written to `src/ReaderHelper.Desktop/www`, which is the desktop shell's default local content path.
