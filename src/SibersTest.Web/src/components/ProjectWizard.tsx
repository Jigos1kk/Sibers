import { useState, useEffect } from 'react';
import { HiChevronLeft, HiChevronRight, HiCheck, HiX } from 'react-icons/hi';
import type { WizardData } from '../types/project';
import { projectsApi, employeesApi } from '../services/api';
import EmployeeAutocomplete from './EmployeeAutocomplete';
import FileUploader from './FileUploader';

interface ProjectWizardProps {
  onClose: () => void;
  onSuccess: () => void;
}

const STEPS = [
  { title: 'Основная информация', description: 'Название, даты, приоритет' },
  { title: 'Компании', description: 'Заказчик и исполнитель' },
  { title: 'Руководитель', description: 'Выбор руководителя проекта' },
  { title: 'Исполнители', description: 'Выбор сотрудников проекта' },
  { title: 'Документы', description: 'Загрузка файлов проекта' },
];

interface StepErrors {
  projectName?: string;
  startDate?: string;
  endDate?: string;
  priority?: string;
  customerCompanyName?: string;
  contractorCompanyName?: string;
  managerId?: string;
}

export default function ProjectWizard({ onClose, onSuccess }: ProjectWizardProps) {
  const [currentStep, setCurrentStep] = useState(0);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [wizardData, setWizardData] = useState<WizardData>({
    projectName: '',
    startDate: '',
    endDate: '',
    priority: 0,
    customerCompanyName: '',
    contractorCompanyName: '',
    managerId: 0,
    employeeIds: [],
    documents: [],
  });
  const [errors, setErrors] = useState<StepErrors>({});

  const updateField = <K extends keyof WizardData>(field: K, value: WizardData[K]) => {
    setWizardData(prev => ({ ...prev, [field]: value }));
    if (errors[field as keyof StepErrors]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };

  const validateStep = (step: number): boolean => {
    const newErrors: StepErrors = {};

    switch (step) {
      case 0:
        if (!wizardData.projectName.trim()) {
          newErrors.projectName = 'Название проекта обязательно';
        } else if (wizardData.projectName.length > 200) {
          newErrors.projectName = 'Название не должно превышать 200 символов';
        }
        if (!wizardData.startDate) {
          newErrors.startDate = 'Дата начала обязательна';
        }
        if (!wizardData.endDate) {
          newErrors.endDate = 'Дата окончания обязательна';
        } else if (wizardData.startDate && new Date(wizardData.endDate) < new Date(wizardData.startDate)) {
          newErrors.endDate = 'Дата окончания не может быть раньше даты начала';
        }
        if (wizardData.priority < 0 || wizardData.priority > 100) {
          newErrors.priority = 'Приоритет должен быть от 0 до 100';
        }
        break;
      case 1:
        if (!wizardData.customerCompanyName.trim()) {
          newErrors.customerCompanyName = 'Название компании-заказчика обязательно';
        }
        if (!wizardData.contractorCompanyName.trim()) {
          newErrors.contractorCompanyName = 'Название компании-исполнителя обязательно';
        }
        break;
      case 2:
        if (!wizardData.managerId) {
          newErrors.managerId = 'Выберите руководителя проекта';
        }
        break;
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleNext = () => {
    if (validateStep(currentStep)) {
      setCurrentStep(prev => Math.min(prev + 1, STEPS.length - 1));
    }
  };

  const handleBack = () => {
    setCurrentStep(prev => Math.max(prev - 1, 0));
  };

  const handleSubmit = async () => {
    if (!validateStep(currentStep)) return;

    setIsSubmitting(true);
    setError(null);

    try {
      const project = await projectsApi.create({
        projectName: wizardData.projectName,
        startDate: wizardData.startDate,
        endDate: wizardData.endDate,
        priority: wizardData.priority,
        customerCompanyName: wizardData.customerCompanyName,
        contractorCompanyName: wizardData.contractorCompanyName,
        managerId: wizardData.managerId,
        employeeIds: wizardData.employeeIds,
      });

      if (wizardData.documents.length > 0) {
        await projectsApi.uploadDocuments(project.id, wizardData.documents);
      }

      onSuccess();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Ошибка при создании проекта');
    } finally {
      setIsSubmitting(false);
    }
  };

  const inputClass = (field: keyof StepErrors) =>
    `w-full px-3 py-2 border rounded-lg text-sm transition-colors focus:outline-none focus:ring-2 ${
      errors[field]
        ? 'border-red-400 focus:ring-red-200 bg-red-50'
        : 'border-gray-300 focus:ring-blue-200 focus:border-blue-400'
    }`;

  const renderStep = () => {
    switch (currentStep) {
      case 0:
        return (
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Название проекта <span className="text-red-500">*</span>
              </label>
              <input
                type="text"
                value={wizardData.projectName}
                onChange={e => updateField('projectName', e.target.value)}
                className={inputClass('projectName')}
                placeholder="Введите название проекта"
              />
              {errors.projectName && <p className="mt-1 text-xs text-red-500">{errors.projectName}</p>}
            </div>
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Дата начала <span className="text-red-500">*</span>
                </label>
                <input
                  type="date"
                  value={wizardData.startDate}
                  onChange={e => updateField('startDate', e.target.value)}
                  className={inputClass('startDate')}
                />
                {errors.startDate && <p className="mt-1 text-xs text-red-500">{errors.startDate}</p>}
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Дата окончания <span className="text-red-500">*</span>
                </label>
                <input
                  type="date"
                  value={wizardData.endDate}
                  onChange={e => updateField('endDate', e.target.value)}
                  className={inputClass('endDate')}
                />
                {errors.endDate && <p className="mt-1 text-xs text-red-500">{errors.endDate}</p>}
              </div>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">Приоритет</label>
              <input
                type="number"
                min={0}
                max={100}
                value={wizardData.priority}
                onChange={e => updateField('priority', parseInt(e.target.value) || 0)}
                className={inputClass('priority')}
              />
              {errors.priority && <p className="mt-1 text-xs text-red-500">{errors.priority}</p>}
            </div>
          </div>
        );

      case 1:
        return (
          <div className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Компания-заказчик <span className="text-red-500">*</span>
              </label>
              <input
                type="text"
                value={wizardData.customerCompanyName}
                onChange={e => updateField('customerCompanyName', e.target.value)}
                className={inputClass('customerCompanyName')}
                placeholder="Название компании-заказчика"
              />
              {errors.customerCompanyName && <p className="mt-1 text-xs text-red-500">{errors.customerCompanyName}</p>}
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Компания-исполнитель <span className="text-red-500">*</span>
              </label>
              <input
                type="text"
                value={wizardData.contractorCompanyName}
                onChange={e => updateField('contractorCompanyName', e.target.value)}
                className={inputClass('contractorCompanyName')}
                placeholder="Название компании-исполнителя"
              />
              {errors.contractorCompanyName && <p className="mt-1 text-xs text-red-500">{errors.contractorCompanyName}</p>}
            </div>
          </div>
        );

      case 2:
        return (
          <div className="space-y-4">
            <div className="bg-blue-50 border border-blue-200 rounded-lg p-3 text-sm text-blue-700">
              Начните вводить фамилию, имя или отчество сотрудника для поиска. 
              Поиск выполняется на сервере по мере ввода текста.
            </div>
            <EmployeeAutocomplete
              value={wizardData.managerId || null}
              onChange={(id) => updateField('managerId', id || 0)}
              label="Руководитель проекта"
              required
              error={errors.managerId}
              placeholder="Начните вводить ФИО руководителя..."
            />
          </div>
        );

      case 3:
        return (
          <div className="space-y-4">
            <div className="bg-blue-50 border border-blue-200 rounded-lg p-3 text-sm text-blue-700">
              Начните вводить фамилию, имя или отчество для поиска сотрудников. 
              Вы можете добавить нескольких исполнителей для проекта.
            </div>
            <EmployeeAutocomplete
              value={null}
              onChange={(id) => {
                if (id && !wizardData.employeeIds.includes(id)) {
                  updateField('employeeIds', [...wizardData.employeeIds, id]);
                }
              }}
              label="Добавить исполнителя"
              placeholder="Начните вводить ФИО сотрудника..."
              excludeIds={wizardData.employeeIds}
            />
            {wizardData.employeeIds.length > 0 && (
              <div>
                <p className="text-xs font-medium text-gray-500 uppercase tracking-wider mb-2">
                  Выбранные исполнители ({wizardData.employeeIds.length})
                </p>
                <div className="space-y-2">
                  {wizardData.employeeIds.map(id => (
                    <EmployeeBadge
                      key={id}
                      employeeId={id}
                      onRemove={() => updateField('employeeIds', wizardData.employeeIds.filter(eid => eid !== id))}
                    />
                  ))}
                </div>
              </div>
            )}
          </div>
        );

      case 4:
        return (
          <FileUploader
            files={wizardData.documents}
            onFilesChange={(files) => updateField('documents', files)}
          />
        );

      default:
        return null;
    }
  };

  return (
    <div className="space-y-6">
      {/* Step Description */}
      <div>
        <h3 className="text-lg font-semibold text-gray-900">
          {STEPS[currentStep].title}
        </h3>
        <p className="text-sm text-gray-500 mt-1">
          {STEPS[currentStep].description}
        </p>
      </div>

      {/* Step Content */}
      <div className="min-h-[200px]">
        {renderStep()}
      </div>

      {/* Error */}
      {error && (
        <div className="flex items-center gap-2 px-3 py-2 bg-red-50 border border-red-200 rounded-lg text-sm text-red-600">
          <HiX className="w-4 h-4 flex-shrink-0" />
          {error}
        </div>
      )}

      {/* Navigation */}
      <div className="flex justify-between items-center pt-4 border-t border-gray-100">
        <button
          type="button"
          onClick={currentStep === 0 ? onClose : handleBack}
          className="inline-flex items-center gap-1.5 px-4 py-2 text-sm font-medium text-gray-600 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors"
        >
          <HiChevronLeft className="w-4 h-4" />
          {currentStep === 0 ? 'Отмена' : 'Назад'}
        </button>

        <div className="text-xs text-gray-400">
          Шаг {currentStep + 1} из {STEPS.length}
        </div>

        {currentStep < STEPS.length - 1 ? (
          <button
            type="button"
            onClick={handleNext}
            className="inline-flex items-center gap-1.5 px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 transition-colors"
          >
            Далее
            <HiChevronRight className="w-4 h-4" />
          </button>
        ) : (
          <button
            type="button"
            onClick={handleSubmit}
            disabled={isSubmitting}
            className="inline-flex items-center gap-1.5 px-4 py-2 text-sm font-medium text-white bg-green-600 rounded-lg hover:bg-green-700 transition-colors disabled:opacity-50"
          >
            {isSubmitting ? (
              <>
                <svg className="animate-spin h-4 w-4" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" />
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
                </svg>
                Создание...
              </>
            ) : (
              <>
                <HiCheck className="w-4 h-4" />
                Создать проект
              </>
            )}
          </button>
        )}
      </div>
    </div>
  );
}

function EmployeeBadge({ employeeId, onRemove }: { employeeId: number; onRemove: () => void }) {
  const [employee, setEmployee] = useState<{ lastName: string; firstName: string; middleName?: string; email: string } | null>(null);

  useEffect(() => {
    let cancelled = false;
    employeesApi.getAll().then(all => {
      if (cancelled) return;
      const found = all.find(e => e.id === employeeId);
      if (found) setEmployee(found);
    }).catch(() => {});
    return () => { cancelled = true; };
  }, [employeeId]);

  if (!employee) {
    return (
      <div className="flex items-center gap-2 px-3 py-2 bg-gray-50 rounded-lg border border-gray-200">
        <div className="w-5 h-5 bg-gray-200 rounded-full animate-pulse" />
        <div className="h-4 w-32 bg-gray-200 rounded animate-pulse" />
      </div>
    );
  }

  return (
    <div className="flex items-center justify-between px-3 py-2 bg-blue-50 rounded-lg border border-blue-200">
      <div className="flex items-center gap-2">
        <div className="w-7 h-7 bg-blue-100 text-blue-700 rounded-full flex items-center justify-center text-xs font-medium">
          {employee.firstName[0]}{employee.lastName[0]}
        </div>
        <div>
          <p className="text-sm font-medium text-gray-900">
            {employee.lastName} {employee.firstName} {employee.middleName || ''}
          </p>
          <p className="text-xs text-gray-500">{employee.email}</p>
        </div>
      </div>
      <button
        type="button"
        onClick={onRemove}
        className="p-1 text-gray-400 hover:text-red-500 hover:bg-red-50 rounded transition-colors"
      >
        <HiX className="w-4 h-4" />
      </button>
    </div>
  );
}