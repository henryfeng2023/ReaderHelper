export interface DesktopRuntimeInfo {
  appName: string;
  appVersion: string;
  hostKind: string;
  chromiumMode: string;
  startUrl: string;
  localApiBaseUrl: string;
  machineName: string;
}

export interface DesktopOperationResult {
  success: boolean;
  errorMessage?: string | null;
}

export interface FileDialogOptions {
  allowMultiple?: boolean;
  title?: string;
  filter?: string;
}

export interface FileDialogResult {
  cancelled: boolean;
  selectedPaths: string[];
}

export interface SaveFileRequest {
  title?: string;
  suggestedFileName?: string;
  filter?: string;
  content: string;
}

export interface SaveFileResult {
  cancelled: boolean;
  savedPath?: string | null;
}

export interface AppSettings {
  theme: string;
  lastOpenedFolder: string;
  lastExportFileName: string;
  notesDraft: string;
}

export interface DesktopBridge {
  pingAsync(message: string): Promise<string>;
  getRuntimeInfoAsync(): Promise<unknown>;
  openExternalAsync(url: string): Promise<unknown>;
  selectFilesAsync(options?: unknown): Promise<unknown>;
  readClipboardTextAsync(): Promise<string>;
  writeClipboardTextAsync(text: string): Promise<boolean>;
  saveTextFileAsync(request: unknown): Promise<unknown>;
  getAppSettingsAsync(): Promise<unknown>;
  saveAppSettingsAsync(settings: unknown): Promise<unknown>;
}

export interface DesktopApi {
  isDesktopShell(): boolean;
  getRuntimeInfo(): Promise<DesktopRuntimeInfo>;
  openExternal(url: string): Promise<DesktopOperationResult>;
  selectFiles(options?: FileDialogOptions): Promise<FileDialogResult>;
  readClipboardText(): Promise<string>;
  writeClipboardText(text: string): Promise<boolean>;
  saveTextFile(request: SaveFileRequest): Promise<SaveFileResult>;
  getAppSettings(): Promise<AppSettings>;
  saveAppSettings(settings: AppSettings): Promise<AppSettings>;
}

declare global {
  interface Window {
    CefSharp?: {
      BindObjectAsync(...names: string[]): Promise<void>;
    };
    desktop?: DesktopBridge;
  }
}

function getObjectValue(source: unknown, camelKey: string, pascalKey: string): unknown {
  if (!source || typeof source !== "object") {
    return undefined;
  }

  const record = source as Record<string, unknown>;
  return record[camelKey] ?? record[pascalKey];
}

function getStringValue(source: unknown, camelKey: string, pascalKey: string, fallback = ""): string {
  const value = getObjectValue(source, camelKey, pascalKey);
  return typeof value === "string" ? value : fallback;
}

function getBooleanValue(source: unknown, camelKey: string, pascalKey: string, fallback = false): boolean {
  const value = getObjectValue(source, camelKey, pascalKey);
  return typeof value === "boolean" ? value : fallback;
}

function getStringArrayValue(source: unknown, camelKey: string, pascalKey: string): string[] {
  const value = getObjectValue(source, camelKey, pascalKey);
  if (!Array.isArray(value)) {
    return [];
  }

  return value.filter((item): item is string => typeof item === "string");
}

function normalizeRuntimeInfo(value: unknown): DesktopRuntimeInfo {
  return {
    appName: getStringValue(value, "appName", "AppName"),
    appVersion: getStringValue(value, "appVersion", "AppVersion"),
    hostKind: getStringValue(value, "hostKind", "HostKind"),
    chromiumMode: getStringValue(value, "chromiumMode", "ChromiumMode"),
    startUrl: getStringValue(value, "startUrl", "StartUrl"),
    localApiBaseUrl: getStringValue(value, "localApiBaseUrl", "LocalApiBaseUrl"),
    machineName: getStringValue(value, "machineName", "MachineName")
  };
}

function normalizeOperationResult(value: unknown): DesktopOperationResult {
  return {
    success: getBooleanValue(value, "success", "Success"),
    errorMessage: getStringValue(value, "errorMessage", "ErrorMessage") || null
  };
}

function normalizeFileDialogResult(value: unknown): FileDialogResult {
  return {
    cancelled: getBooleanValue(value, "cancelled", "Cancelled", true),
    selectedPaths: getStringArrayValue(value, "selectedPaths", "SelectedPaths")
  };
}

function normalizeSaveFileResult(value: unknown): SaveFileResult {
  return {
    cancelled: getBooleanValue(value, "cancelled", "Cancelled", true),
    savedPath: getStringValue(value, "savedPath", "SavedPath") || null
  };
}

function normalizeAppSettings(value: unknown): AppSettings {
  return {
    theme: getStringValue(value, "theme", "Theme", "shell-sand"),
    lastOpenedFolder: getStringValue(value, "lastOpenedFolder", "LastOpenedFolder"),
    lastExportFileName: getStringValue(
      value,
      "lastExportFileName",
      "LastExportFileName",
      "readerhelper-export.txt"
    ),
    notesDraft: getStringValue(value, "notesDraft", "NotesDraft")
  };
}

function toBridgeFileDialogOptions(options?: FileDialogOptions): unknown {
  if (!options) {
    return undefined;
  }

  return {
    AllowMultiple: options.allowMultiple,
    Title: options.title,
    Filter: options.filter
  };
}

function toBridgeSaveFileRequest(request: SaveFileRequest): unknown {
  return {
    Title: request.title,
    SuggestedFileName: request.suggestedFileName,
    Filter: request.filter,
    Content: request.content
  };
}

function toBridgeAppSettings(settings: AppSettings): unknown {
  return {
    Theme: settings.theme,
    LastOpenedFolder: settings.lastOpenedFolder,
    LastExportFileName: settings.lastExportFileName,
    NotesDraft: settings.notesDraft
  };
}

class BrowserDesktopApi implements DesktopApi {
  isDesktopShell(): boolean {
    return false;
  }

  async getRuntimeInfo(): Promise<DesktopRuntimeInfo> {
    return {
      appName: "ReaderHelper",
      appVersion: "browser",
      hostKind: "browser",
      chromiumMode: "external-browser",
      startUrl: window.location.href,
      localApiBaseUrl: "http://127.0.0.1:5057",
      machineName: "n/a"
    };
  }

  async openExternal(url: string): Promise<DesktopOperationResult> {
    window.open(url, "_blank", "noopener,noreferrer");
    return { success: true };
  }

  async selectFiles(options?: FileDialogOptions): Promise<FileDialogResult> {
    const input = document.createElement("input");
    input.type = "file";
    input.multiple = options?.allowMultiple ?? false;

    return await new Promise<FileDialogResult>((resolve) => {
      let settled = false;

      const complete = (result: FileDialogResult) => {
        if (settled) {
          return;
        }

        settled = true;
        window.removeEventListener("focus", handleFocus);
        resolve(result);
      };

      const handleFocus = () => {
        window.setTimeout(() => {
          const selectedPaths = Array.from(input.files ?? []).map((file) => file.name);
          complete({
            cancelled: selectedPaths.length === 0,
            selectedPaths
          });
        }, 0);
      };

      input.addEventListener(
        "change",
        () => {
          const selectedPaths = Array.from(input.files ?? []).map((file) => file.name);
          complete({
            cancelled: selectedPaths.length === 0,
            selectedPaths
          });
        },
        { once: true }
      );

      window.addEventListener("focus", handleFocus, { once: true });
      input.click();
    });
  }

  async readClipboardText(): Promise<string> {
    if (!navigator.clipboard?.readText) {
      return "";
    }

    try {
      return await navigator.clipboard.readText();
    } catch {
      return "";
    }
  }

  async writeClipboardText(text: string): Promise<boolean> {
    if (!navigator.clipboard?.writeText) {
      return false;
    }

    try {
      await navigator.clipboard.writeText(text);
      return true;
    } catch {
      return false;
    }
  }

  async saveTextFile(request: SaveFileRequest): Promise<SaveFileResult> {
    const fileName = request.suggestedFileName || "readerhelper-export.txt";
    const blob = new Blob([request.content], { type: "text/plain;charset=utf-8" });
    const url = URL.createObjectURL(blob);
    const anchor = document.createElement("a");
    anchor.href = url;
    anchor.download = fileName;
    anchor.click();
    URL.revokeObjectURL(url);

    return {
      cancelled: false,
      savedPath: fileName
    };
  }

  async getAppSettings(): Promise<AppSettings> {
    const raw = window.localStorage.getItem("readerhelper.appsettings");
    if (!raw) {
      return {
        theme: "shell-sand",
        lastOpenedFolder: "",
        lastExportFileName: "readerhelper-export.txt",
        notesDraft: ""
      };
    }

    try {
      return JSON.parse(raw) as AppSettings;
    } catch {
      return {
        theme: "shell-sand",
        lastOpenedFolder: "",
        lastExportFileName: "readerhelper-export.txt",
        notesDraft: ""
      };
    }
  }

  async saveAppSettings(settings: AppSettings): Promise<AppSettings> {
    window.localStorage.setItem("readerhelper.appsettings", JSON.stringify(settings));
    return settings;
  }
}

class CefSharpDesktopApi implements DesktopApi {
  private readonly bridge: DesktopBridge;

  constructor(bridge: DesktopBridge) {
    this.bridge = bridge;
  }

  isDesktopShell(): boolean {
    return true;
  }

  getRuntimeInfo(): Promise<DesktopRuntimeInfo> {
    return this.bridge.getRuntimeInfoAsync().then(normalizeRuntimeInfo);
  }

  openExternal(url: string): Promise<DesktopOperationResult> {
    return this.bridge.openExternalAsync(url).then(normalizeOperationResult);
  }

  selectFiles(options?: FileDialogOptions): Promise<FileDialogResult> {
    return this.bridge.selectFilesAsync(toBridgeFileDialogOptions(options)).then(
      normalizeFileDialogResult
    );
  }

  readClipboardText(): Promise<string> {
    return this.bridge.readClipboardTextAsync();
  }

  writeClipboardText(text: string): Promise<boolean> {
    return this.bridge.writeClipboardTextAsync(text);
  }

  saveTextFile(request: SaveFileRequest): Promise<SaveFileResult> {
    return this.bridge.saveTextFileAsync(toBridgeSaveFileRequest(request)).then(
      normalizeSaveFileResult
    );
  }

  getAppSettings(): Promise<AppSettings> {
    return this.bridge.getAppSettingsAsync().then(normalizeAppSettings);
  }

  saveAppSettings(settings: AppSettings): Promise<AppSettings> {
    return this.bridge.saveAppSettingsAsync(toBridgeAppSettings(settings)).then(
      normalizeAppSettings
    );
  }
}

export async function createDesktopApi(): Promise<DesktopApi> {
  if (!window.CefSharp?.BindObjectAsync) {
    return new BrowserDesktopApi();
  }

  await window.CefSharp.BindObjectAsync("desktop");

  if (!window.desktop) {
    return new BrowserDesktopApi();
  }

  return new CefSharpDesktopApi(window.desktop);
}
