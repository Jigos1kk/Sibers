import type { ProjectResponse, ProjectRequest, EmployeeResponse, ProjectFilters, TaskRequest, TaskResponse } from '../types/project';

const API_BASE = '/api';

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const errorText = await response.text().catch(() => 'Unknown error');
    throw new Error(`HTTP ${response.status}: ${errorText}`);
  }
  return response.json();
}

function buildQueryString(filters?: ProjectFilters): string {
  if (!filters) return '';
  const params = new URLSearchParams();
  const filterMap: Record<string, string | number | undefined | boolean> = {
    startDateFrom: filters.startDateFrom,
    startDateTo: filters.startDateTo,
    priorityFrom: filters.priorityFrom,
    priorityTo: filters.priorityTo,
    customerCompanyName: filters.customerCompanyName,
    contractorCompanyName: filters.contractorCompanyName,
    managerId: filters.managerId,
    sortBy: filters.sortBy,
    isDescending: filters.isDescending,
  };

  for (const [key, value] of Object.entries(filterMap)) {
    if (value !== undefined && value !== '') {
      params.append(key, String(value));
    }
  }

  const qs = params.toString();
  return qs ? `?${qs}` : '';
}

export const projectsApi = {
  async getAll(filters?: ProjectFilters): Promise<ProjectResponse[]> {
    const qs = buildQueryString(filters);
    const res = await fetch(`${API_BASE}/project${qs}`);
    return handleResponse<ProjectResponse[]>(res);
  },

  async getById(id: number): Promise<ProjectResponse> {
    const res = await fetch(`${API_BASE}/project/${id}`);
    return handleResponse<ProjectResponse>(res);
  },

  async create(data: ProjectRequest): Promise<ProjectResponse> {
    const res = await fetch(`${API_BASE}/project`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });
    return handleResponse<ProjectResponse>(res);
  },

  async update(id: number, data: ProjectRequest): Promise<void> {
    const res = await fetch(`${API_BASE}/project/${id}`, {
      method: 'PATCH',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });
    if (!res.ok) {
      const errorText = await res.text().catch(() => 'Unknown error');
      throw new Error(`HTTP ${res.status}: ${errorText}`);
    }
  },

  async delete(id: number): Promise<void> {
    const res = await fetch(`${API_BASE}/project/${id}`, {
      method: 'DELETE',
    });
    if (!res.ok) {
      const errorText = await res.text().catch(() => 'Unknown error');
      throw new Error(`HTTP ${res.status}: ${errorText}`);
    }
  },

  async uploadDocuments(id: number, files: File[]): Promise<string[]> {
    const formData = new FormData();
    files.forEach(file => formData.append('files', file));
    const res = await fetch(`${API_BASE}/project/${id}/documents`, {
      method: 'POST',
      body: formData,
    });
    return handleResponse<string[]>(res);
  },
};

export const employeesApi = {
  async getAll(): Promise<EmployeeResponse[]> {
    const res = await fetch(`${API_BASE}/employee`);
    return handleResponse<EmployeeResponse[]>(res);
  },

  async search(term: string): Promise<EmployeeResponse[]> {
    const res = await fetch(`${API_BASE}/employee/search?term=${encodeURIComponent(term)}`);
    return handleResponse<EmployeeResponse[]>(res);
  },

  async create(data: { firstName: string; lastName: string; middleName?: string; email: string }): Promise<EmployeeResponse> {
    const res = await fetch(`${API_BASE}/employee`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });
    return handleResponse<EmployeeResponse>(res);
  },
};

export const tasksApi = {
  async getAll(projectId?: number): Promise<TaskResponse[]> {
    const qs = projectId ? `?projectId=${projectId}` : '';
    const res = await fetch(`${API_BASE}/projecttask${qs}`);
    return handleResponse<TaskResponse[]>(res);
  },

  async getById(id: number): Promise<TaskResponse> {
    const res = await fetch(`${API_BASE}/projecttask/${id}`);
    return handleResponse<TaskResponse>(res);
  },

  async create(data: TaskRequest): Promise<TaskResponse> {
    const res = await fetch(`${API_BASE}/projecttask`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });
    return handleResponse<TaskResponse>(res);
  },

  async update(id: number, data: TaskRequest): Promise<void> {
    const res = await fetch(`${API_BASE}/projecttask/${id}`, {
      method: 'PATCH',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(data),
    });
    if (!res.ok) {
      const errorText = await res.text().catch(() => 'Unknown error');
      throw new Error(`HTTP ${res.status}: ${errorText}`);
    }
  },

  async delete(id: number): Promise<void> {
    const res = await fetch(`${API_BASE}/projecttask/${id}`, {
      method: 'DELETE',
    });
    if (!res.ok) {
      const errorText = await res.text().catch(() => 'Unknown error');
      throw new Error(`HTTP ${res.status}: ${errorText}`);
    }
  },
};