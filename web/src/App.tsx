import { useEffect, useState } from "react";
import {
  type AppSettings,
  createDesktopApi,
  type DesktopApi,
  type DesktopOperationResult,
  type SaveFileResult,
  type FileDialogResult,
  type DesktopRuntimeInfo
} from "./platform/desktop-api";
import {
  LocalApiClient,
  type JobStartResponse,
  type JobStatusResponse,
  type LocalApiRuntimeInfo
} from "./platform/local-api-client";

type BootState =
  | { status: "loading" }
  | {
      status: "ready";
      desktopApi: DesktopApi;
      runtimeInfo: DesktopRuntimeInfo;
      localApi: LocalApiClient;
      localApiRuntime: LocalApiRuntimeInfo | null;
    }
  | {
      status: "error";
      message: string;
    };

export default function App() {
  const [bootState, setBootState] = useState<BootState>({ status: "loading" });
  const [pingValue, setPingValue] = useState<string>("");
  const [openExternalResult, setOpenExternalResult] = useState<DesktopOperationResult | null>(null);
  const [jobStartResult, setJobStartResult] = useState<JobStartResponse | null>(null);
  const [jobStatus, setJobStatus] = useState<JobStatusResponse | null>(null);
  const [jobError, setJobError] = useState<string | null>(null);
  const [fileDialogResult, setFileDialogResult] = useState<FileDialogResult | null>(null);
  const [clipboardValue, setClipboardValue] = useState<string>("");
  const [clipboardWriteResult, setClipboardWriteResult] = useState<string>("");
  const [saveFileResult, setSaveFileResult] = useState<SaveFileResult | null>(null);
  const [settingsState, setSettingsState] = useState<AppSettings>({
    theme: "shell-sand",
    lastOpenedFolder: "",
    lastExportFileName: "readerhelper-export.txt",
    notesDraft: ""
  });
  const [settingsMessage, setSettingsMessage] = useState<string>("");

  useEffect(() => {
    let cancelled = false;

    async function bootstrap() {
      try {
        const desktopApi = await createDesktopApi();
        const runtimeInfo = await desktopApi.getRuntimeInfo();
        const localApi = await LocalApiClient.fromDesktopApi(desktopApi);
        const settings = await desktopApi.getAppSettings();

        let localApiRuntime: LocalApiRuntimeInfo | null = null;
        try {
          localApiRuntime = await localApi.getRuntime();
        } catch {
          localApiRuntime = null;
        }

        if (!cancelled) {
          setBootState({
            status: "ready",
            desktopApi,
            runtimeInfo,
            localApi,
            localApiRuntime
          });
          setSettingsState(settings);
        }
      } catch (error) {
        if (!cancelled) {
          setBootState({
            status: "error",
            message: error instanceof Error ? error.message : String(error)
          });
        }
      }
    }

    void bootstrap();

    return () => {
      cancelled = true;
    };
  }, []);

  useEffect(() => {
    if (bootState.status !== "ready" || !jobStartResult) {
      return;
    }

    let disposed = false;

    async function pollJob() {
      try {
        // @ts-ignore
        const status = await bootState.localApi.getJob(jobStartResult.jobId);
        if (!disposed) {
          setJobStatus(status);
        }
      } catch (error) {
        if (!disposed) {
          setJobError(error instanceof Error ? error.message : String(error));
        }
      }
    }

    void pollJob();

    if (jobStatus?.status === "completed") {
      return;
    }

    const timerId = window.setInterval(() => {
      void pollJob();
    }, 1500);

    return () => {
      disposed = true;
      window.clearInterval(timerId);
    };
  }, [bootState, jobStartResult, jobStatus?.status]);

  if (bootState.status === "loading") {
    return (
      <div className="page-shell">
        <section className="hero-panel">
          <p className="eyebrow">ReaderHelper</p>
          <h1>Bootstrapping hybrid shell</h1>
          <p className="summary">Initializing desktop bridge and local API client.</p>
        </section>
      </div>
    );
  }

  if (bootState.status === "error") {
    return (
      <div className="page-shell">
        <section className="hero-panel">
          <p className="eyebrow">ReaderHelper</p>
          <h1>Bootstrap failed</h1>
          <p className="summary">{bootState.message}</p>
        </section>
      </div>
    );
  }

  const { desktopApi, runtimeInfo, localApiRuntime, localApi } = bootState;

  async function handlePing() {
    try {
      const result = await desktopApi.getRuntimeInfo();
      setPingValue(`${result.hostKind}:${result.chromiumMode}:${result.machineName}`);
    } catch (error) {
      setPingValue(error instanceof Error ? error.message : String(error));
    }
  }

  async function handleOpenExternal() {
    const result = await desktopApi.openExternal("https://openai.com");
    setOpenExternalResult(result);
  }

  async function handleStartSampleJob() {
    setJobError(null);
    setJobStatus(null);

    try {
      const response = await localApi.startJob({
        jobType: "sample-index",
        payload: "seed=readerhelper"
      });

      setJobStartResult(response);
    } catch (error) {
      setJobError(error instanceof Error ? error.message : String(error));
    }
  }

  async function handleSelectFiles() {
    const result = await desktopApi.selectFiles({
      allowMultiple: true,
      title: "Select Reader Sources",
      filter: "Documents|*.pdf;*.epub;*.txt|All Files|*.*"
    });

    setFileDialogResult(result);

    if (!result.cancelled && result.selectedPaths.length > 0) {
      const firstPath = result.selectedPaths[0];
      const lastSlashIndex = Math.max(firstPath.lastIndexOf("\\"), firstPath.lastIndexOf("/"));
      const lastOpenedFolder = lastSlashIndex >= 0 ? firstPath.slice(0, lastSlashIndex) : "";
      const nextSettings = {
        ...settingsState,
        lastOpenedFolder
      };

      setSettingsState(nextSettings);
      await desktopApi.saveAppSettings(nextSettings);
      setSettingsMessage("last opened folder saved");
    }
  }

  async function handleReadClipboard() {
    const value = await desktopApi.readClipboardText();
    setClipboardValue(value);
  }

  async function handleWriteClipboard() {
    const success = await desktopApi.writeClipboardText(
      "ReaderHelper desktop bridge clipboard sample"
    );
    setClipboardWriteResult(success ? "clipboard updated" : "clipboard write failed");
  }

  async function handleSaveTextFile() {
    const result = await desktopApi.saveTextFile({
      title: "Export Reader Notes",
      suggestedFileName: settingsState.lastExportFileName || "readerhelper-export.txt",
      filter: "Text Files|*.txt|Markdown Files|*.md|All Files|*.*",
      content: settingsState.notesDraft
    });

    setSaveFileResult(result);

    if (!result.cancelled && result.savedPath) {
      const fileName = result.savedPath.split(/[/\\]/).pop() || result.savedPath;
      const nextSettings = {
        ...settingsState,
        lastExportFileName: fileName
      };

      setSettingsState(nextSettings);
      await desktopApi.saveAppSettings(nextSettings);
      setSettingsMessage("export file name saved");
    }
  }

  async function handleLoadSettings() {
    const settings = await desktopApi.getAppSettings();
    setSettingsState(settings);
    setSettingsMessage("settings loaded");
  }

  async function handleSaveSettings() {
    const saved = await desktopApi.saveAppSettings(settingsState);
    setSettingsState(saved);
    setSettingsMessage("settings saved");
  }

  return (
    <div className="page-shell">
      <section className="hero-panel">
        <div className="hero-header">
          <div>
            <p className="eyebrow">ReaderHelper</p>
            <h1>Web and desktop hybrid shell</h1>
          </div>
          <span className={`mode-pill ${desktopApi.isDesktopShell() ? "desktop" : "browser"}`}>
            {desktopApi.isDesktopShell() ? "Desktop shell" : "Browser mode"}
          </span>
        </div>

        <p className="summary">
          The frontend talks to the desktop shell through a thin bridge and pushes heavy work to a localhost service.
        </p>

        <div className="action-row">
          <button onClick={handlePing}>Read Runtime</button>
          <button onClick={handleOpenExternal}>Open External</button>
          <button onClick={handleStartSampleJob}>Start Sample Job</button>
          <button onClick={handleSelectFiles}>Select Files</button>
          <button onClick={handleReadClipboard}>Read Clipboard</button>
          <button onClick={handleWriteClipboard}>Write Clipboard</button>
          <button onClick={handleSaveTextFile}>Save Text File</button>
          <button onClick={handleLoadSettings}>Load Settings</button>
          <button onClick={handleSaveSettings}>Save Settings</button>
        </div>

        {pingValue ? <p className="feedback">{pingValue}</p> : null}
        {openExternalResult ? (
          <p className="feedback">
            External call: {openExternalResult.success ? "success" : openExternalResult.errorMessage ?? "failed"}
          </p>
        ) : null}
        {jobError ? <p className="feedback error">{jobError}</p> : null}
        {clipboardWriteResult ? <p className="feedback">{clipboardWriteResult}</p> : null}
        {saveFileResult ? (
          <p className="feedback">
            File save: {saveFileResult.cancelled ? "cancelled" : saveFileResult.savedPath ?? "saved"}
          </p>
        ) : null}
        {settingsMessage ? <p className="feedback">{settingsMessage}</p> : null}
      </section>

      <section className="grid">
        <article className="card">
          <h2>Desktop Runtime</h2>
          <dl className="details">
            <div>
              <dt>App</dt>
              <dd>{runtimeInfo.appName}</dd>
            </div>
            <div>
              <dt>Version</dt>
              <dd>{runtimeInfo.appVersion}</dd>
            </div>
            <div>
              <dt>Host</dt>
              <dd>{runtimeInfo.hostKind}</dd>
            </div>
            <div>
              <dt>Chromium</dt>
              <dd>{runtimeInfo.chromiumMode}</dd>
            </div>
            <div>
              <dt>Start URL</dt>
              <dd>{runtimeInfo.startUrl}</dd>
            </div>
            <div>
              <dt>Local API</dt>
              <dd>{runtimeInfo.localApiBaseUrl}</dd>
            </div>
            <div>
              <dt>Machine</dt>
              <dd>{runtimeInfo.machineName}</dd>
            </div>
          </dl>
        </article>

        <article className="card">
          <h2>Local Service</h2>
          <dl className="details">
            <div>
              <dt>Status</dt>
              <dd>{localApiRuntime ? "reachable" : "not reachable"}</dd>
            </div>
            <div>
              <dt>Service</dt>
              <dd>{localApiRuntime?.serviceName ?? "n/a"}</dd>
            </div>
            <div>
              <dt>Version</dt>
              <dd>{localApiRuntime?.serviceVersion ?? "n/a"}</dd>
            </div>
            <div>
              <dt>Mode</dt>
              <dd>{localApiRuntime?.mode ?? "n/a"}</dd>
            </div>
            <div>
              <dt>Started</dt>
              <dd>{localApiRuntime?.startedAt ?? "n/a"}</dd>
            </div>
          </dl>
        </article>

        <article className="card wide">
          <h2>Sample Job Flow</h2>
          <dl className="details">
            <div>
              <dt>Accepted</dt>
              <dd>{jobStartResult?.status ?? "not started"}</dd>
            </div>
            <div>
              <dt>Job Id</dt>
              <dd>{jobStartResult?.jobId ?? "n/a"}</dd>
            </div>
            <div>
              <dt>Current Status</dt>
              <dd>{jobStatus?.status ?? "n/a"}</dd>
            </div>
            <div>
              <dt>Progress</dt>
              <dd>{jobStatus ? `${jobStatus.progressPercent}%` : "n/a"}</dd>
            </div>
            <div>
              <dt>Result</dt>
              <dd>{jobStatus?.result ?? "n/a"}</dd>
            </div>
          </dl>
        </article>

        <article className="card">
          <h2>Native File Picker</h2>
          <dl className="details">
            <div>
              <dt>Cancelled</dt>
              <dd>{fileDialogResult ? String(fileDialogResult.cancelled) : "n/a"}</dd>
            </div>
            <div>
              <dt>Selected</dt>
              <dd>
                {fileDialogResult?.selectedPaths.length
                  ? fileDialogResult.selectedPaths.join(", ")
                  : "n/a"}
              </dd>
            </div>
          </dl>
        </article>

        <article className="card">
          <h2>Clipboard</h2>
          <dl className="details">
            <div>
              <dt>Last Read</dt>
              <dd>{clipboardValue || "n/a"}</dd>
            </div>
            <div>
              <dt>Last Write</dt>
              <dd>{clipboardWriteResult || "n/a"}</dd>
            </div>
          </dl>
        </article>

        <article className="card wide">
          <h2>App Settings And Export</h2>
          <div className="form-grid">
            <label className="field">
              <span>Theme</span>
              <input
                value={settingsState.theme}
                onChange={(event) =>
                  setSettingsState((current) => ({
                    ...current,
                    theme: event.target.value
                  }))
                }
              />
            </label>

            <label className="field">
              <span>Last Opened Folder</span>
              <input
                value={settingsState.lastOpenedFolder}
                onChange={(event) =>
                  setSettingsState((current) => ({
                    ...current,
                    lastOpenedFolder: event.target.value
                  }))
                }
              />
            </label>

            <label className="field">
              <span>Last Export File Name</span>
              <input
                value={settingsState.lastExportFileName}
                onChange={(event) =>
                  setSettingsState((current) => ({
                    ...current,
                    lastExportFileName: event.target.value
                  }))
                }
              />
            </label>

            <label className="field field-wide">
              <span>Notes Draft</span>
              <textarea
                value={settingsState.notesDraft}
                onChange={(event) =>
                  setSettingsState((current) => ({
                    ...current,
                    notesDraft: event.target.value
                  }))
                }
                rows={8}
              />
            </label>
          </div>
        </article>
      </section>
    </div>
  );
}
