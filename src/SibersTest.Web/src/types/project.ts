export interface ProjectResponse {
  id: number;
  name: string;
  startDate: string;
  endDate: string;
  priority: number;
  customerCompanyName: string;
  contractorCompanyName: string;
  manager: EmployeeResponse;
  employes: EmployeeResponse[];
  createdAt: string;
  updatedAt: string;
}

export interface ProjectRequest {
  projectName: string;
  startDate: string;
  endDate: string;
  priority: number;
  customerCompanyName: string;
  contractorCompanyName: string;
  managerId: number;
  employeeIds: number[];
}

export interface EmployeeResponse {
  id: number;
  firstName: string;
  lastName: string;
  middleName?: string;
  email: string;
}

export interface ProjectFilters {
  startDateFrom?: string;
  startDateTo?: string;
  priorityFrom?: number;
  priorityTo?: number;
  customerCompanyName?: string;
  contractorCompanyName?: string;
  managerId?: number;
  sortBy?: string;
  isDescending?: boolean;
}

export interface ProjectValidationErrors {
  projectName?: string;
  startDate?: string;
  endDate?: string;
  priority?: string;
  customerCompanyName?: string;
  contractorCompanyName?: string;
  managerId?: string;
}