import { useState, useEffect } from 'react';
import { HiOutlineSave } from 'react-icons/hi';
import type { TaskRequest, EmployeeResponse, TaskValidationErrors } from '../types/project';
import { TaskStatusEnum as TaskStatusValues } from '../types/project';

interface TaskFormProps {
  initialData?: TaskRequest;
  employees: EmployeeResponse[];
  projectId: number;
  onSubmit: (data: TaskRequest) => Promise<void>;
  onCancel: () => void;
  isSubmitting: boolean;
}

function validate(data: TaskRequest): TaskValidationErrors {
  const errors: TaskValidationErrors = {};

  if (!data.name.trim()) {
    errors.name = 'Название задачи обязательно';
  } else if (data.name.length > 200) {
    errors.name = 'Название не должно превышать 200 символов';
  }

  if (data.priority < 0 || data.priority > 100) {
    errors.priority = 'Приоритет должен быть от 0 до 100';
  }

  if (!data.authorId) {
    errors.authorId = 'Выберите автора задачи';
  }

  if (!data.assignedId) {
    errors.assignedId = 'Выберите исполнителя задачи';
  }

  return errors;
}

export default function TaskForm({ initialData, employees, projectId, onSubmit, onCancel, isSubmitting }: TaskFormProps) {
  const [formData, setFormData] = useState<TaskRequest>({
    name: '',
    comment: '',
    priority: 0,
    status: TaskStatusValues.ToDo,
    authorId: 0,
    assignedId: 0,
    projectId: projectId,
  });
  const [errors, setErrors] = useState<TaskValidationErrors>({});

  useEffect(() => {
    if (initialData) {
      setFormData(initialData);
    }
  }, [initialData]);

  const handleChange = (field: keyof TaskRequest, value: string | number) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    if (errors[field as keyof TaskValidationErrors]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
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

  const inputClass = (field: keyof TaskValidationErrors) =>
    `w-full px-3 py-2 border rounded-lg text-sm transition-colors focus:outline-none focus:ring-2 ${
      errors[field]
        ? 'border-red-400 focus:ring-red-200 bg-red-50'
        : 'border-gray-300 focus:ring-blue-200 focus:border-blue-400'
    }`;

  return (
    <form onSubmit={handleSubmit} className="space-y-5">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {/* Task Name */}
        <div className="md:col-span-2">
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Название задачи <span className="text-red-500">*</span>
          </label>
          <input
            type="text"
            value={formData.name}
            onChange={e => handleChange('name', e.target.value)}
            className={inputClass('name')}
            placeholder="Введите название задачи"
          />
          {errors.name && <p className="mt-1 text-xs text-red-500">{errors.name}</p>}
        </div>

        {/* Comment */}
        <div className="md:col-span-2">
          <label className="block text-sm font-medium text-gray-700 mb-1">Комментарий</label>
          <textarea
            value={formData.comment || ''}
            onChange={e => handleChange('comment', e.target.value)}
            className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-200 focus:border-blue-400"
            rows={3}
            placeholder="Дополнительная информация"
          />
        </div>

        {/* Priority */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Приоритет</label>
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

        {/* Status */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Статус</label>
          <select
            value={formData.status}
            onChange={e => handleChange('status', e.target.value)}
            className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-200 focus:border-blue-400"
          >
            <option value={TaskStatusValues.ToDo}>К выполнению</option>
            <option value={TaskStatusValues.Progress}>В работе</option>
            <option value={TaskStatusValues.Done}>Выполнено</option>
          </select>
        </div>

        {/* Author */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Автор <span className="text-red-500">*</span>
          </label>
          <select
            value={formData.authorId}
            onChange={e => handleChange('authorId', parseInt(e.target.value) || 0)}
            className={inputClass('authorId')}
          >
            <option value={0}>Выберите автора</option>
            {employees.map(emp => (
              <option key={emp.id} value={emp.id}>
                {emp.lastName} {emp.firstName} {emp.middleName || ''}
              </option>
            ))}
          </select>
          {errors.authorId && <p className="mt-1 text-xs text-red-500">{errors.authorId}</p>}
        </div>

        {/* Assigned */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">
            Исполнитель <span className="text-red-500">*</span>
          </label>
          <select
            value={formData.assignedId}
            onChange={e => handleChange('assignedId', parseInt(e.target.value) || 0)}
            className={inputClass('assignedId')}
          >
            <option value={0}>Выберите исполнителя</option>
            {employees.map(emp => (
              <option key={emp.id} value={emp.id}>
                {emp.lastName} {emp.firstName} {emp.middleName || ''}
              </option>
            ))}
          </select>
          {errors.assignedId && <p className="mt-1 text-xs text-red-500">{errors.assignedId}</p>}
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
          <HiOutlineSave className="w-4 h-4" />
          {initialData ? 'Сохранить изменения' : 'Создать задачу'}
        </button>
      </div>
    </form>
  );
}