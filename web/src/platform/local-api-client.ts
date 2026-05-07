import type { DesktopApi } from "./desktop-api";

export interface LocalApiRuntimeInfo {
  serviceName: string;
  serviceVersion: string;
  mode: string;
  startedAt: string;
}

export interface JobStartRequest {
  jobType: string;
  payload?: string;
}

export interface JobStartResponse {
  jobId: string;
  status: string;
}

export interface JobStatusResponse {
  jobId: string;
  jobType: string;
  status: string;
  progressPercent: number;
  result?: string | null;
}

export class LocalApiClient {
  private readonly baseUrl: string;

  private constructor(baseUrl: string) {
    if (!baseUrl) {
      throw new Error("Desktop runtime did not provide a local API base URL.");
    }

    this.baseUrl = baseUrl.replace(/\/$/, "");
  }

  static async fromDesktopApi(desktopApi: DesktopApi): Promise<LocalApiClient> {
    const runtime = await desktopApi.getRuntimeInfo();
    return new LocalApiClient(runtime.localApiBaseUrl);
  }

  async getRuntime(): Promise<LocalApiRuntimeInfo> {
    return this.getJson<LocalApiRuntimeInfo>("/api/runtime");
  }

  async startJob(request: JobStartRequest): Promise<JobStartResponse> {
    return this.sendJson<JobStartResponse>("/api/jobs", request);
  }

  async getJob(jobId: string): Promise<JobStatusResponse> {
    return this.getJson<JobStatusResponse>(`/api/jobs/${encodeURIComponent(jobId)}`);
  }

  private async getJson<T>(path: string): Promise<T> {
    const response = await fetch(`${this.baseUrl}${path}`, {
      method: "GET"
    });

    if (!response.ok) {
      throw new Error(`Local API request failed: ${response.status}`);
    }

    return (await response.json()) as T;
  }

  private async sendJson<T>(path: string, body: unknown): Promise<T> {
    const response = await fetch(`${this.baseUrl}${path}`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json"
      },
      body: JSON.stringify(body)
    });

    if (!response.ok) {
      throw new Error(`Local API request failed: ${response.status}`);
    }

    return (await response.json()) as T;
  }
}
