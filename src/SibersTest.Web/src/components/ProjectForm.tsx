import { useState, useEffect } from 'react';
import type { ProjectRequest, EmployeeResponse, ProjectValidationErrors } from '../types/project';

interface ProjectFormProps {
  initialData?: ProjectRequest;
  employees: EmployeeResponse[];
  onSubmit: (data: ProjectRequest) => Promise<void>;
  onCancel: () => void;
  isSubmitting: boolean;
}

function validate(data: ProjectRequest): ProjectValidationErrors {
  const errors: ProjectValidationErrors = {};

  if (!data.projectName.trim()) {
    errors.projectName = 'Название проекта обязательно';
  } else if (data.projectName.length > 200) {
    errors.projectName = 'Название не должно превышать 200 символов';
  }

  if (!data.startDate) {
    errors.startDate = 'Дата начала обязательна';
  }

  if (!data.endDate) {
    errors.endDate = 'Дата окончания обязательна';
  } else if (data.startDate && new Date(data.endDate) < new Date(data.startDate)) {
    errors.endDate = 'Дата окончания не может быть раньше даты начала';
  }

  if (data.priority < 0 || data.priority > 100) {
    errors.priority = 'Приоритет должен быть от 0 до 100';
  }

  if (!data.customerCompanyName.trim()) {
    errors.customerCompanyName = 'Название компании-заказчика обязательно';
  }

  if (!data.contractorCompanyName.trim()) {
    errors.contractorCompanyName = 'Название компании-исполнителя обязательно';
  }

  if (!data.managerId) {
    errors.managerId = 'Выберите руководителя проекта';
  }

  return errors;
}

export default function ProjectForm({ initialData, employees, onSubmit, onCancel, isSubmitting }: ProjectFormProps) {
  const [formData, setFormData] = useState<ProjectRequest>({
    projectName: '',
    startDate: '',
    endDate: '',
    priority: 0,
    customerCompanyName: '',
    contractorCompanyName: '',
    managerId: 0,
    employeeIds: [],
  });
  const [errors, setErrors] = useState<ProjectValidationErrors>({});
  const [selectedEmployees, setSelectedEmployees] = useState<number[]>([]);

  useEffect(() => {
    if (initialData) {
      setFormData(initialData);
      setSelectedEmployees(initialData.employeeIds);
    }
  }, [initialData]);

  const handleChange = (field: keyof ProjectRequest, value: string | number | number[]) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    if (errors[field as keyof ProjectValidationErrors]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };

  const toggleEmployee = (empId: number) => {
    setSelectedEmployees(prev => {
      const updated = prev.includes(empId)
        ? prev.filter(id => id !== empId)
        : [...prev, empId];
      setFormData(prevData => ({ ...prevData, employeeIds: updated }));
      return updated;
    });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const validationErrors = validate(formData);
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }
    await onSubmit(formData);
  };

  const inputClass = (field: keyof ProjectValidationErrors) =>
    `w-full px-3 py-2 border rounded-lg text-sm transition-colors focus:outline-none focus:ring-2 ${
      errors[field]
        ? 'border-red-400 focus:ring-red-200 bg-red-50'
        : 'border-gray-300 focus:ring-blue-200 focus:border-blue-400'
    }`;

  return (
    <form onSubmit={handleSubmit} className="space-y-5">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {/* Project Name */}
        <div className="md:col-span-2">
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Название проекта <span className="text-red-500">*</span>
          </label>
          <input
            type="text"
            value={formData.projectName}
            onChange={e => handleChange('projectName', e.target.value)}
            className={inputClass('projectName')}
            placeholder="Введите название проекта"
          />
          {errors.projectName && <p className="mt-1 text-xs text-red-500">{errors.projectName}</p>}
        </div>

        {/* Start Date */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Дата начала <span className="text-red-500">*</span>
          </label>
          <input
            type="date"
            value={formData.startDate}
            onChange={e => handleChange('startDate', e.target.value)}
            className={inputClass('startDate')}
          />
          {errors.startDate && <p className="mt-1 text-xs text-red-500">{errors.startDate}</p>}
        </div>

        {/* End Date */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Дата окончания <span className="text-red-500">*</span>
          </label>
          <input
            type="date"
            value={formData.endDate}
            onChange={e => handleChange('endDate', e.target.value)}
            className={inputClass('endDate')}
          />
          {errors.endDate && <p className="mt-1 text-xs text-red-500">{errors.endDate}</p>}
        </div>

        {/* Priority */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Приоритет
          </label>
          <input
            type="number"
            min={0}
            max={100}
            value={formData.priority}
            onChange={e => handleChange('priority', parseInt(e.target.value) || 0)}
            className={inputClass('priority')}
          />
          {errors.priority && <p className="mt-1 text-xs text-red-500">{errors.priority}</p>}
        </div>

        {/* Manager */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Руководитель проекта <span className="text-red-500">*</span>
          </label>
          <select
            value={formData.managerId}
            onChange={e => handleChange('managerId', parseInt(e.target.value) || 0)}
            className={inputClass('managerId')}
          >
            <option value={0}>Выберите руководителя</option>
            {employees.map(emp => (
              <option key={emp.id} value={emp.id}>
                {emp.lastName} {emp.firstName} {emp.middleName || ''}
              </option>
            ))}
          </select>
          {errors.managerId && <p className="mt-1 text-xs text-red-500">{errors.managerId}</p>}
        </div>

        {/* Customer Company */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Компания-заказчик <span className="text-red-500">*</span>
          </label>
          <input
            type="text"
            value={formData.customerCompanyName}
            onChange={e => handleChange('customerCompanyName', e.target.value)}
            className={inputClass('customerCompanyName')}
            placeholder="Название компании"
          />
          {errors.customerCompanyName && <p className="mt-1 text-xs text-red-500">{errors.customerCompanyName}</p>}
        </div>

        {/* Contractor Company */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Компания-исполнитель <span className="text-red-500">*</span>
          </label>
          <input
            type="text"
            value={formData.contractorCompanyName}
            onChange={e => handleChange('contractorCompanyName', e.target.value)}
            className={inputClass('contractorCompanyName')}
            placeholder="Название компании"
          />
          {errors.contractorCompanyName && <p className="mt-1 text-xs text-red-500">{errors.contractorCompanyName}</p>}
        </div>
      </div>

      {/* Employees Selection */}
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-2">
          Сотрудники проекта
        </label>
        <div className="max-h-40 overflow-y-auto border border-gray-200 rounded-lg p-2 space-y-1">
          {employees.length === 0 ? (
            <p className="text-sm text-gray-400 py-2 text-center">Нет доступных сотрудников</p>
          ) : (
            employees.map(emp => (
              <label
                key={emp.id}
                className={`flex items-center gap-2 px-3 py-1.5 rounded-md cursor-pointer text-sm transition-colors ${
                  selectedEmployees.includes(emp.id)
                    ? 'bg-blue-50 text-blue-700'
                    : 'hover:bg-gray-50 text-gray-600'
                }`}
              >
                <input
                  type="checkbox"
                  checked={selectedEmployees.includes(emp.id)}
                  onChange={() => toggleEmployee(emp.id)}
                  className="rounded border-gray-300 text-blue-600 focus:ring-blue-200"
                />
                {emp.lastName} {emp.firstName} {emp.middleName || ''}
                <span className="text-gray-400 ml-auto text-xs">{emp.email}</span>
              </label>
            ))
          )}
        </div>
      </div>

      {/* Actions */}
      <div className="flex justify-end gap-3 pt-2 border-t border-gray-100">
        <button
          type="button"
          onClick={onCancel}
          disabled={isSubmitting}
          className="px-4 py-2 text-sm font-medium text-gray-600 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors disabled:opacity-50"
        >
          Отмена
        </button>
        <button
          type="submit"
          disabled={isSubmitting}
          className="px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50 flex items-center gap-2"
        >
          {isSubmitting && (
            <svg className="animate-spin h-4 w-4" viewBox="0 0 24 24">
              <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" />
              <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
            </svg>
          )}
          {initialData ? 'Сохранить изменения' : 'Создать проект'}
        </button>
      </div>
    </form>
  );
}