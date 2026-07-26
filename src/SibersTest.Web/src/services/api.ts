import type { ProjectResponse, ProjectRequest, EmployeeResponse, ProjectFilters } from '../types/project';

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
};

export const employeesApi = {
  async getAll(): Promise<EmployeeResponse[]> {
    const res = await fetch(`${API_BASE}/employee`);
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