import { useState, useEffect, useCallback, useMemo } from 'react';
import { HiPlus, HiSearch, HiChevronUp, HiChevronDown, HiSwitchVertical, HiPencil, HiTrash, HiX, HiCheck, HiExclamation, HiOutlineDocumentText, HiOutlineUserGroup, HiOutlineOfficeBuilding, HiOutlineCalendar, HiOutlineClipboardCheck } from 'react-icons/hi';
import type { ProjectResponse, ProjectRequest, EmployeeResponse, ProjectFilters, TaskResponse, TaskRequest, TaskStatusEnum as TaskStatusType } from '../types/project';
import { TaskStatusEnum as TaskStatusValues } from '../types/project';
import { projectsApi, employeesApi, tasksApi } from '../services/api';
import ProjectForm from './ProjectForm';
import EmployeeForm from './EmployeeForm';
import TaskForm from './TaskForm';
import Modal from './Modal';

type SortField = 'startDate' | 'endDate' | 'priority' | 'name';
type NotificationType = 'success' | 'error' | 'warning';

interface Notification {
  message: string;
  type: NotificationType;
}

export default function ProjectList() {
  const [projects, setProjects] = useState<ProjectResponse[]>([]);
  const [employees, setEmployees] = useState<EmployeeResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [notification, setNotification] = useState<Notification | null>(null);

  // Modal state
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isDeleteModalOpen, setIsDeleteModalOpen] = useState(false);
  const [isViewModalOpen, setIsViewModalOpen] = useState(false);
  const [isEmployeeModalOpen, setIsEmployeeModalOpen] = useState(false);
  const [isFilterExpanded, setIsFilterExpanded] = useState(false);
  const [selectedProject, setSelectedProject] = useState<ProjectResponse | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Task state
  const [tasks, setTasks] = useState<TaskResponse[]>([]);
  const [isCreateTaskModalOpen, setIsCreateTaskModalOpen] = useState(false);
  const [isEditTaskModalOpen, setIsEditTaskModalOpen] = useState(false);
  const [isDeleteTaskModalOpen, setIsDeleteTaskModalOpen] = useState(false);
  const [selectedTask, setSelectedTask] = useState<TaskResponse | null>(null);
  const [taskToDelete, setTaskToDelete] = useState<TaskResponse | null>(null);
  const [isTaskViewModalOpen, setIsTaskViewModalOpen] = useState(false);

  // Filters state
  const [filters, setFilters] = useState<ProjectFilters>({
    sortBy: 'StartDate',
    isDescending: true,
  });
  const [searchTerm, setSearchTerm] = useState('');

  // Extended filter inputs
  const [filterInputs, setFilterInputs] = useState({
    startDateFrom: '',
    startDateTo: '',
    priorityFrom: '',
    priorityTo: '',
    customerCompanyName: '',
    contractorCompanyName: '',
  });

  const [projectToDelete, setProjectToDelete] = useState<ProjectResponse | null>(null);

  const showNotification = useCallback((message: string, type: NotificationType) => {
    setNotification({ message, type });
    setTimeout(() => setNotification(null), 4000);
  }, []);

  const fetchProjects = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await projectsApi.getAll(filters);
      setProjects(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Ошибка загрузки проектов');
    } finally {
      setLoading(false);
    }
  }, [filters]);

  const fetchEmployees = useCallback(async () => {
    try {
      const data = await employeesApi.getAll();
      setEmployees(data);
    } catch {
      // silently fail
    }
  }, []);

  useEffect(() => {
    fetchProjects();
    fetchEmployees();
  }, [fetchProjects, fetchEmployees]);

  const filteredProjects = useMemo(() => {
    if (!searchTerm) return projects;
    const term = searchTerm.toLowerCase();
    return projects.filter(p =>
      p.name.toLowerCase().includes(term) ||
      p.customerCompanyName.toLowerCase().includes(term) ||
      p.contractorCompanyName.toLowerCase().includes(term)
    );
  }, [projects, searchTerm]);

  const handleCreate = async (data: ProjectRequest) => {
    try {
      setIsSubmitting(true);
      await projectsApi.create(data);
      setIsCreateModalOpen(false);
      showNotification('Проект успешно создан', 'success');
      await fetchProjects();
    } catch (err) {
      showNotification(
        err instanceof Error ? err.message : 'Ошибка при создании проекта',
        'error'
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleUpdate = async (data: ProjectRequest) => {
    if (!selectedProject) return;
    try {
      setIsSubmitting(true);
      await projectsApi.update(selectedProject.id, data);
      setIsEditModalOpen(false);
      setSelectedProject(null);
      showNotification('Проект успешно обновлён', 'success');
      await fetchProjects();
    } catch (err) {
      showNotification(
        err instanceof Error ? err.message : 'Ошибка при обновлении проекта',
        'error'
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDelete = async () => {
    if (!projectToDelete) return;
    try {
      setIsSubmitting(true);
      await projectsApi.delete(projectToDelete.id);
      setIsDeleteModalOpen(false);
      setProjectToDelete(null);
      showNotification('Проект успешно удалён', 'success');
      await fetchProjects();
    } catch (err) {
      showNotification(
        err instanceof Error ? err.message : 'Ошибка при удалении проекта',
        'error'
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleCreateEmployee = async (data: { firstName: string; lastName: string; middleName?: string; email: string }) => {
    try {
      setIsSubmitting(true);
      await employeesApi.create(data);
      setIsEmployeeModalOpen(false);
      showNotification('Сотрудник успешно добавлен', 'success');
      await fetchEmployees();
    } catch (err) {
      showNotification(
        err instanceof Error ? err.message : 'Ошибка при добавлении сотрудника',
        'error'
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  const applyFilters = () => {
    const newFilters: ProjectFilters = {
      sortBy: filters.sortBy,
      isDescending: filters.isDescending,
    };
    if (filterInputs.startDateFrom) newFilters.startDateFrom = filterInputs.startDateFrom;
    if (filterInputs.startDateTo) newFilters.startDateTo = filterInputs.startDateTo;
    if (filterInputs.priorityFrom) newFilters.priorityFrom = parseInt(filterInputs.priorityFrom);
    if (filterInputs.priorityTo) newFilters.priorityTo = parseInt(filterInputs.priorityTo);
    if (filterInputs.customerCompanyName) newFilters.customerCompanyName = filterInputs.customerCompanyName;
    if (filterInputs.contractorCompanyName) newFilters.contractorCompanyName = filterInputs.contractorCompanyName;
    setFilters(newFilters);
  };

  const resetFilters = () => {
    setFilterInputs({
      startDateFrom: '',
      startDateTo: '',
      priorityFrom: '',
      priorityTo: '',
      customerCompanyName: '',
      contractorCompanyName: '',
    });
    setFilters({ sortBy: 'StartDate', isDescending: true });
    setSearchTerm('');
  };

  const hasActiveFilters = useMemo(() => {
    return Object.values(filterInputs).some(v => v !== '') ||
      filters.sortBy !== 'StartDate' ||
      filters.isDescending !== true;
  }, [filterInputs, filters]);

  const openEditModal = (project: ProjectResponse) => {
    setSelectedProject(project);
    setIsViewModalOpen(false);
    // Use setTimeout to ensure view modal closes before edit opens
    setTimeout(() => setIsEditModalOpen(true), 50);
  };

  const fetchTasks = useCallback(async (projectId: number) => {
    try {
      const data = await tasksApi.getAll(projectId);
      setTasks(data);
    } catch {
      setTasks([]);
    }
  }, []);

  const openViewModal = (project: ProjectResponse) => {
    setSelectedProject(project);
    setIsViewModalOpen(true);
    fetchTasks(project.id);
  };

  const openDeleteConfirm = (project: ProjectResponse) => {
    setProjectToDelete(project);
    setIsDeleteModalOpen(true);
  };

  const getEditFormData = (project: ProjectResponse): ProjectRequest => ({
    projectName: project.name,
    startDate: project.startDate.split('T')[0],
    endDate: project.endDate.split('T')[0],
    priority: project.priority,
    customerCompanyName: project.customerCompanyName,
    contractorCompanyName: project.contractorCompanyName,
    managerId: project.manager.id,
    employeeIds: project.employes.map(e => e.id),
  });

  const handleSort = (field: SortField) => {
    setFilters(prev => ({
      ...prev,
      sortBy: field === 'name' ? 'StartDate' : field.charAt(0).toUpperCase() + field.slice(1),
      isDescending: prev.sortBy === field ? !prev.isDescending : true,
    }));
  };

  const SortIcon = ({ field }: { field: SortField }) => {
    const sortField = field === 'name' ? 'StartDate' : field.charAt(0).toUpperCase() + field.slice(1);
    if (filters.sortBy !== sortField) return <HiSwitchVertical className="w-3 h-3 opacity-50" />;
    return filters.isDescending ? <HiChevronDown className="w-3 h-3" /> : <HiChevronUp className="w-3 h-3" />;
  };

  const formatDate = (dateStr: string) => {
    return new Date(dateStr).toLocaleDateString('ru-RU', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
    });
  };

  const getPriorityColor = (priority: number) => {
    if (priority >= 70) return 'text-red-600 bg-red-50 border-red-200';
    if (priority >= 40) return 'text-yellow-600 bg-yellow-50 border-yellow-200';
    return 'text-green-600 bg-green-50 border-green-200';
  };

  const getPriorityLabel = (priority: number) => {
    if (priority >= 70) return 'Высокий';
    if (priority >= 40) return 'Средний';
    return 'Низкий';
  };

  const handleCreateTask = async (data: TaskRequest) => {
    try {
      setIsSubmitting(true);
      await tasksApi.create(data);
      setIsCreateTaskModalOpen(false);
      showNotification('Задача успешно создана', 'success');
      if (selectedProject) fetchTasks(selectedProject.id);
    } catch (err) {
      showNotification(
        err instanceof Error ? err.message : 'Ошибка при создании задачи',
        'error'
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleUpdateTask = async (data: TaskRequest) => {
    if (!selectedTask) return;
    try {
      setIsSubmitting(true);
      await tasksApi.update(selectedTask.id, data);
      setIsEditTaskModalOpen(false);
      setSelectedTask(null);
      showNotification('Задача успешно обновлена', 'success');
      if (selectedProject) fetchTasks(selectedProject.id);
    } catch (err) {
      showNotification(
        err instanceof Error ? err.message : 'Ошибка при обновлении задачи',
        'error'
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleDeleteTask = async () => {
    if (!taskToDelete) return;
    try {
      setIsSubmitting(true);
      await tasksApi.delete(taskToDelete.id);
      setIsDeleteTaskModalOpen(false);
      setTaskToDelete(null);
      showNotification('Задача успешно удалена', 'success');
      if (selectedProject) fetchTasks(selectedProject.id);
    } catch (err) {
      showNotification(
        err instanceof Error ? err.message : 'Ошибка при удалении задачи',
        'error'
      );
    } finally {
      setIsSubmitting(false);
    }
  };

  const openEditTaskModal = (task: TaskResponse) => {
    setSelectedTask(task);
    setIsTaskViewModalOpen(false);
    setTimeout(() => setIsEditTaskModalOpen(true), 50);
  };

  const openDeleteTaskConfirm = (task: TaskResponse) => {
    setTaskToDelete(task);
    setIsDeleteTaskModalOpen(true);
  };

  const getEditTaskFormData = (task: TaskResponse): TaskRequest => ({
    name: task.name,
    comment: task.comment || '',
    priority: task.priority,
    status: task.status,
    authorId: task.author.id,
    assignedId: task.assigned.id,
    projectId: task.projectId,
  });

  const getStatusLabel = (status: string) => {
    switch (status) {
      case TaskStatusValues.ToDo: return 'К выполнению';
      case TaskStatusValues.Progress: return 'В работе';
      case TaskStatusValues.Done: return 'Выполнено';
      default: return status;
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case TaskStatusValues.ToDo: return 'bg-gray-100 text-gray-700';
      case TaskStatusValues.Progress: return 'bg-blue-100 text-blue-700';
      case TaskStatusValues.Done: return 'bg-green-100 text-green-700';
      default: return 'bg-gray-100 text-gray-700';
    }
  };

  const filterInputClass = "w-full px-3 py-1.5 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-200 focus:border-blue-400";

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Notification Toast */}
      {notification && (
        <div
          className={`fixed top-4 right-4 z-[100] px-4 py-3 rounded-lg shadow-lg text-sm font-medium transition-all animate-slide-in flex items-center gap-2 ${
            notification.type === 'success'
              ? 'bg-green-50 text-green-800 border border-green-200'
              : notification.type === 'error'
              ? 'bg-red-50 text-red-800 border border-red-200'
              : 'bg-yellow-50 text-yellow-800 border border-yellow-200'
          }`}
        >
          {notification.type === 'success' && <HiCheck className="w-4 h-4 text-green-500" />}
          {notification.type === 'error' && <HiX className="w-4 h-4 text-red-500" />}
          {notification.type === 'warning' && <HiExclamation className="w-4 h-4 text-yellow-500" />}
          {notification.message}
        </div>
      )}

      {/* Header */}
      <header className="bg-white border-b border-gray-200">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-2xl font-bold text-gray-900">Управление проектами</h1>
              <p className="text-sm text-gray-500 mt-1">
                Всего проектов: {filteredProjects.length}
              </p>
            </div>
            <div className="flex items-center gap-2">
              <button
                onClick={() => setIsEmployeeModalOpen(true)}
                className="inline-flex items-center gap-2 px-4 py-2.5 bg-white text-gray-700 text-sm font-medium rounded-lg border border-gray-300 hover:bg-gray-50 transition-colors"
              >
                <HiOutlineUserGroup className="w-4 h-4" />
                Добавить сотрудника
              </button>
              <button
                onClick={() => setIsCreateModalOpen(true)}
                className="inline-flex items-center gap-2 px-4 py-2.5 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors shadow-sm"
              >
                <HiPlus className="w-4 h-4" />
                Новый проект
              </button>
            </div>
          </div>
        </div>
      </header>

      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">
        {/* Filters */}
        <div className="bg-white rounded-xl border border-gray-200 p-4 mb-6">
          <div className="flex flex-wrap items-center gap-3">
            <div className="flex-1 min-w-[200px]">
              <div className="relative">
                <HiSearch className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
                <input
                  type="text"
                  value={searchTerm}
                  onChange={e => setSearchTerm(e.target.value)}
                  placeholder="Поиск проектов..."
                  className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-200 focus:border-blue-400"
                />
              </div>
            </div>
            <div className="flex items-center gap-2 text-sm text-gray-500">
              <span>Сортировка:</span>
              <button
                onClick={() => handleSort('startDate')}
                className={`inline-flex items-center gap-1 px-3 py-1.5 rounded-md border transition-colors ${
                  filters.sortBy === 'StartDate' ? 'bg-blue-50 border-blue-200 text-blue-700' : 'border-gray-200 hover:bg-gray-50'
                }`}
              >
                Дата <SortIcon field="startDate" />
              </button>
              <button
                onClick={() => handleSort('priority')}
                className={`inline-flex items-center gap-1 px-3 py-1.5 rounded-md border transition-colors ${
                  filters.sortBy === 'Priority' ? 'bg-blue-50 border-blue-200 text-blue-700' : 'border-gray-200 hover:bg-gray-50'
                }`}
              >
                Приоритет <SortIcon field="priority" />
              </button>
              <button
                onClick={() => handleSort('endDate')}
                className={`inline-flex items-center gap-1 px-3 py-1.5 rounded-md border transition-colors ${
                  filters.sortBy === 'EndDate' ? 'bg-blue-50 border-blue-200 text-blue-700' : 'border-gray-200 hover:bg-gray-50'
                }`}
              >
                Окончание <SortIcon field="endDate" />
              </button>
            </div>
            <button
              onClick={() => setIsFilterExpanded(!isFilterExpanded)}
              className={`inline-flex items-center gap-1 px-3 py-1.5 rounded-md border text-sm transition-colors ${
                hasActiveFilters || isFilterExpanded ? 'bg-blue-50 border-blue-200 text-blue-700' : 'border-gray-200 text-gray-500 hover:bg-gray-50'
              }`}
            >
              <HiOutlineDocumentText className="w-4 h-4" />
              Фильтры
              {(hasActiveFilters || isFilterExpanded) && <span className="w-2 h-2 rounded-full bg-blue-500" />}
            </button>
            {hasActiveFilters && (
              <button
                onClick={resetFilters}
                className="text-sm text-red-500 hover:text-red-700 transition-colors"
              >
                Сбросить
              </button>
            )}
          </div>

          {/* Expanded Filters */}
          {isFilterExpanded && (
            <div className="mt-4 pt-4 border-t border-gray-100">
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
                <div>
                  <label className="block text-xs font-medium text-gray-500 mb-1">Дата начала от</label>
                  <input
                    type="date"
                    value={filterInputs.startDateFrom}
                    onChange={e => setFilterInputs(prev => ({ ...prev, startDateFrom: e.target.value }))}
                    className={filterInputClass}
                  />
                </div>
                <div>
                  <label className="block text-xs font-medium text-gray-500 mb-1">Дата начала до</label>
                  <input
                    type="date"
                    value={filterInputs.startDateTo}
                    onChange={e => setFilterInputs(prev => ({ ...prev, startDateTo: e.target.value }))}
                    className={filterInputClass}
                  />
                </div>
                <div>
                  <label className="block text-xs font-medium text-gray-500 mb-1">Приоритет от</label>
                  <input
                    type="number"
                    min={0}
                    max={100}
                    value={filterInputs.priorityFrom}
                    onChange={e => setFilterInputs(prev => ({ ...prev, priorityFrom: e.target.value }))}
                    placeholder="0"
                    className={filterInputClass}
                  />
                </div>
                <div>
                  <label className="block text-xs font-medium text-gray-500 mb-1">Приоритет до</label>
                  <input
                    type="number"
                    min={0}
                    max={100}
                    value={filterInputs.priorityTo}
                    onChange={e => setFilterInputs(prev => ({ ...prev, priorityTo: e.target.value }))}
                    placeholder="100"
                    className={filterInputClass}
                  />
                </div>
                <div>
                  <label className="block text-xs font-medium text-gray-500 mb-1">Компания-заказчик</label>
                  <input
                    type="text"
                    value={filterInputs.customerCompanyName}
                    onChange={e => setFilterInputs(prev => ({ ...prev, customerCompanyName: e.target.value }))}
                    placeholder="Название компании"
                    className={filterInputClass}
                  />
                </div>
                <div>
                  <label className="block text-xs font-medium text-gray-500 mb-1">Компания-исполнитель</label>
                  <input
                    type="text"
                    value={filterInputs.contractorCompanyName}
                    onChange={e => setFilterInputs(prev => ({ ...prev, contractorCompanyName: e.target.value }))}
                    placeholder="Название компании"
                    className={filterInputClass}
                  />
                </div>
              </div>
              <div className="flex justify-end mt-4">
                <button
                  onClick={applyFilters}
                  className="px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors"
                >
                  Применить фильтры
                </button>
              </div>
            </div>
          )}
        </div>

        {/* Error State */}
        {error && (
          <div className="bg-red-50 border border-red-200 rounded-xl p-6 text-center mb-6">
            <HiX className="w-12 h-12 text-red-400 mx-auto mb-3" />
            <p className="text-red-600 font-medium mb-2">Ошибка загрузки</p>
            <p className="text-red-500 text-sm mb-4">{error}</p>
            <button
              onClick={fetchProjects}
              className="px-4 py-2 bg-red-600 text-white text-sm rounded-lg hover:bg-red-700 transition-colors"
            >
              Попробовать снова
            </button>
          </div>
        )}

        {/* Loading State */}
        {loading && (
          <div className="bg-white rounded-xl border border-gray-200 p-12 text-center">
            <div className="inline-flex items-center gap-3 text-gray-500">
              <svg className="animate-spin h-6 w-6 text-blue-600" viewBox="0 0 24 24">
                <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" />
                <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
              </svg>
              <span>Загрузка проектов...</span>
            </div>
          </div>
        )}

        {/* Empty State */}
        {!loading && !error && filteredProjects.length === 0 && (
          <div className="bg-white rounded-xl border border-gray-200 p-12 text-center">
            <HiOutlineDocumentText className="w-16 h-16 text-gray-300 mx-auto mb-4" />
            <h3 className="text-lg font-medium text-gray-900 mb-1">
              {searchTerm ? 'Проекты не найдены' : 'Нет проектов'}
            </h3>
            <p className="text-sm text-gray-500 mb-4">
              {searchTerm
                ? 'Попробуйте изменить параметры поиска'
                : 'Создайте первый проект, чтобы начать работу'}
            </p>
            {!searchTerm && (
              <button
                onClick={() => setIsCreateModalOpen(true)}
                className="inline-flex items-center gap-2 px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors"
              >
                <HiPlus className="w-4 h-4" />
                Создать проект
              </button>
            )}
          </div>
        )}

        {/* Projects Table */}
        {!loading && !error && filteredProjects.length > 0 && (
          <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="bg-gray-50 border-b border-gray-200">
                    <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">Название</th>
                    <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">Даты</th>
                    <th className="text-center px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">Приоритет</th>
                    <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">Компании</th>
                    <th className="text-left px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">Руководитель</th>
                    <th className="text-center px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">Сотрудники</th>
                    <th className="text-right px-4 py-3 text-xs font-semibold text-gray-500 uppercase tracking-wider">Действия</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-100">
                  {filteredProjects.map(project => (
                    <tr
                      key={project.id}
                      className="hover:bg-gray-50 transition-colors cursor-pointer"
                      onClick={() => openViewModal(project)}
                    >
                      <td className="px-4 py-3">
                        <span className="text-sm font-medium text-gray-900">{project.name}</span>
                      </td>
                      <td className="px-4 py-3">
                        <div className="flex items-center gap-1 text-sm text-gray-600">
                          <HiOutlineCalendar className="w-3.5 h-3.5 text-gray-400" />
                          <span>{formatDate(project.startDate)}</span>
                          <span className="text-gray-300">—</span>
                          <span>{formatDate(project.endDate)}</span>
                        </div>
                      </td>
                      <td className="px-4 py-3 text-center">
                        <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border ${getPriorityColor(project.priority)}`}>
                          {getPriorityLabel(project.priority)} ({project.priority})
                        </span>
                      </td>
                      <td className="px-4 py-3">
                        <div className="flex items-center gap-1 text-sm text-gray-600">
                          <HiOutlineOfficeBuilding className="w-3.5 h-3.5 text-gray-400 flex-shrink-0" />
                          <div>
                            <div>{project.customerCompanyName}</div>
                            <div className="text-xs text-gray-400">{project.contractorCompanyName}</div>
                          </div>
                        </div>
                      </td>
                      <td className="px-4 py-3">
                        <div className="text-sm text-gray-700">
                          {project.manager.lastName} {project.manager.firstName}
                        </div>
                      </td>
                      <td className="px-4 py-3 text-center">
                        <span className="inline-flex items-center justify-center w-7 h-7 bg-blue-50 text-blue-700 text-xs font-medium rounded-full">
                          {project.employes.length}
                        </span>
                      </td>
                      <td className="px-4 py-3 text-right">
                        <div className="flex items-center justify-end gap-1" onClick={e => e.stopPropagation()}>
                          <button
                            onClick={() => openEditModal(project)}
                            className="p-1.5 text-gray-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                            title="Редактировать"
                          >
                            <HiPencil className="w-4 h-4" />
                          </button>
                          <button
                            onClick={() => openDeleteConfirm(project)}
                            className="p-1.5 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                            title="Удалить"
                          >
                            <HiTrash className="w-4 h-4" />
                          </button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}
      </main>

      {/* Create Project Modal */}
      <Modal isOpen={isCreateModalOpen} onClose={() => setIsCreateModalOpen(false)} title="Создание проекта" size="lg">
        <ProjectForm
          employees={employees}
          onSubmit={handleCreate}
          onCancel={() => setIsCreateModalOpen(false)}
          isSubmitting={isSubmitting}
        />
      </Modal>

      {/* Edit Project Modal */}
      <Modal isOpen={isEditModalOpen} onClose={() => { setIsEditModalOpen(false); setSelectedProject(null); }} title="Редактирование проекта" size="lg">
        {selectedProject && (
          <ProjectForm
            initialData={getEditFormData(selectedProject)}
            employees={employees}
            onSubmit={handleUpdate}
            onCancel={() => { setIsEditModalOpen(false); setSelectedProject(null); }}
            isSubmitting={isSubmitting}
          />
        )}
      </Modal>

      {/* View Project Modal */}
      <Modal isOpen={isViewModalOpen} onClose={() => { setIsViewModalOpen(false); setSelectedProject(null); }} title="Детали проекта" size="lg">
        {selectedProject && (
          <div className="space-y-6">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">Название</label>
                <p className="text-sm font-medium text-gray-900">{selectedProject.name}</p>
              </div>
              <div>
                <label className="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">Приоритет</label>
                <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border ${getPriorityColor(selectedProject.priority)}`}>
                  {getPriorityLabel(selectedProject.priority)} ({selectedProject.priority})
                </span>
              </div>
              <div>
                <label className="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">Дата начала</label>
                <p className="text-sm text-gray-900">{formatDate(selectedProject.startDate)}</p>
              </div>
              <div>
                <label className="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">Дата окончания</label>
                <p className="text-sm text-gray-900">{formatDate(selectedProject.endDate)}</p>
              </div>
              <div>
                <label className="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">Компания-заказчик</label>
                <p className="text-sm text-gray-900">{selectedProject.customerCompanyName}</p>
              </div>
              <div>
                <label className="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">Компания-исполнитель</label>
                <p className="text-sm text-gray-900">{selectedProject.contractorCompanyName}</p>
              </div>
              <div>
                <label className="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">Руководитель</label>
                <p className="text-sm text-gray-900">
                  {selectedProject.manager.lastName} {selectedProject.manager.firstName} {selectedProject.manager.middleName || ''}
                </p>
              </div>
              <div>
                <label className="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">Email руководителя</label>
                <p className="text-sm text-gray-900">{selectedProject.manager.email}</p>
              </div>
            </div>

            <div>
              <div className="flex items-center justify-between mb-2">
                <label className="block text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Сотрудники проекта ({selectedProject.employes.length})
                </label>
                <button
                  onClick={() => openEditModal(selectedProject)}
                  className="inline-flex items-center gap-1 px-3 py-1.5 text-xs font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 transition-colors"
                >
                  <HiPlus className="w-3.5 h-3.5" />
                  Добавить сотрудника
                </button>
              </div>
              {selectedProject.employes.length === 0 ? (
                <p className="text-sm text-gray-400">Нет назначенных сотрудников</p>
              ) : (
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-2">
                  {selectedProject.employes.map(emp => (
                    <div key={emp.id} className="flex items-center gap-2 px-3 py-2 bg-gray-50 rounded-lg">
                      <div className="w-8 h-8 bg-blue-100 text-blue-700 rounded-full flex items-center justify-center text-xs font-medium">
                        {emp.firstName[0]}{emp.lastName[0]}
                      </div>
                      <div>
                        <p className="text-sm font-medium text-gray-900">
                          {emp.lastName} {emp.firstName} {emp.middleName || ''}
                        </p>
                        <p className="text-xs text-gray-500">{emp.email}</p>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>

            {/* Tasks Section */}
            <div className="pt-4 border-t border-gray-100">
              <div className="flex items-center justify-between mb-3">
                <label className="block text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Задачи проекта ({tasks.length})
                </label>
                <button
                  onClick={() => setIsCreateTaskModalOpen(true)}
                  className="inline-flex items-center gap-1 px-3 py-1.5 text-xs font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 transition-colors"
                >
                  <HiPlus className="w-3.5 h-3.5" />
                  Добавить задачу
                </button>
              </div>
              {tasks.length === 0 ? (
                <p className="text-sm text-gray-400">Нет задач</p>
              ) : (
                <div className="space-y-2">
                  {tasks.map(task => (
                    <div
                      key={task.id}
                      className="flex items-center justify-between px-3 py-2 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors cursor-pointer"
                      onClick={() => { setSelectedTask(task); setIsTaskViewModalOpen(true); }}
                    >
                      <div className="flex items-center gap-3 min-w-0">
                        <HiOutlineClipboardCheck className="w-4 h-4 text-gray-400 flex-shrink-0" />
                        <div className="min-w-0">
                          <p className="text-sm font-medium text-gray-900 truncate">{task.name}</p>
                          <p className="text-xs text-gray-500">
                            {task.author.lastName} {task.author.firstName[0]}. → {task.assigned.lastName} {task.assigned.firstName[0]}.
                          </p>
                        </div>
                      </div>
                      <div className="flex items-center gap-2 flex-shrink-0">
                        <span className={`inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium ${getStatusColor(task.status)}`}>
                          {getStatusLabel(task.status)}
                        </span>
                        <span className="text-xs text-gray-400">#{task.priority}</span>
                      </div>
                    </div>
                  ))}
                </div>
              )}
            </div>

            <div className="flex justify-end gap-3 pt-2 border-t border-gray-100">
              <button
                onClick={() => openEditModal(selectedProject)}
                className="inline-flex items-center gap-2 px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 transition-colors"
              >
                <HiPencil className="w-4 h-4" />
                Редактировать
              </button>
            </div>
          </div>
        )}
      </Modal>

      {/* Delete Confirmation Modal */}
      <Modal isOpen={isDeleteModalOpen} onClose={() => { setIsDeleteModalOpen(false); setProjectToDelete(null); }} title="Подтверждение удаления" size="sm">
        {projectToDelete && (
          <div className="space-y-4">
            <div className="flex items-start gap-3">
              <div className="flex-shrink-0 w-10 h-10 bg-red-100 rounded-full flex items-center justify-center">
                <HiExclamation className="w-5 h-5 text-red-600" />
              </div>
              <div>
                <p className="text-sm text-gray-700">
                  Вы уверены, что хотите удалить проект <strong>«{projectToDelete.name}»</strong>?
                </p>
                <p className="text-xs text-gray-500 mt-1">Это действие нельзя отменить.</p>
              </div>
            </div>
            <div className="flex justify-end gap-3 pt-2">
              <button
                onClick={() => { setIsDeleteModalOpen(false); setProjectToDelete(null); }}
                disabled={isSubmitting}
                className="px-4 py-2 text-sm font-medium text-gray-600 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors disabled:opacity-50"
              >
                Отмена
              </button>
              <button
                onClick={handleDelete}
                disabled={isSubmitting}
                className="px-4 py-2 text-sm font-medium text-white bg-red-600 rounded-lg hover:bg-red-700 transition-colors disabled:opacity-50 flex items-center gap-2"
              >
                {isSubmitting && (
                  <svg className="animate-spin h-4 w-4" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" />
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
                  </svg>
                )}
                <HiTrash className="w-4 h-4" />
                Удалить
              </button>
            </div>
          </div>
        )}
      </Modal>

      {/* Create Employee Modal */}
      <Modal isOpen={isEmployeeModalOpen} onClose={() => setIsEmployeeModalOpen(false)} title="Добавление сотрудника" size="md">
        <EmployeeForm
          onSubmit={handleCreateEmployee}
          onCancel={() => setIsEmployeeModalOpen(false)}
          isSubmitting={isSubmitting}
        />
      </Modal>

      {/* Create Task Modal */}
      <Modal isOpen={isCreateTaskModalOpen} onClose={() => setIsCreateTaskModalOpen(false)} title="Создание задачи" size="lg">
        {selectedProject && (
          <TaskForm
            employees={selectedProject.employes}
            projectId={selectedProject.id}
            onSubmit={handleCreateTask}
            onCancel={() => setIsCreateTaskModalOpen(false)}
            isSubmitting={isSubmitting}
          />
        )}
      </Modal>

      {/* Edit Task Modal */}
      <Modal isOpen={isEditTaskModalOpen} onClose={() => { setIsEditTaskModalOpen(false); setSelectedTask(null); }} title="Редактирование задачи" size="lg">
        {selectedTask && selectedProject && (
          <TaskForm
            initialData={getEditTaskFormData(selectedTask)}
            employees={selectedProject.employes}
            projectId={selectedTask.projectId}
            onSubmit={handleUpdateTask}
            onCancel={() => { setIsEditTaskModalOpen(false); setSelectedTask(null); }}
            isSubmitting={isSubmitting}
          />
        )}
      </Modal>

      {/* View Task Modal */}
      <Modal isOpen={isTaskViewModalOpen} onClose={() => { setIsTaskViewModalOpen(false); setSelectedTask(null); }} title="Детали задачи" size="md">
        {selectedTask && (
          <div className="space-y-4">
            <div>
              <label className="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">Название</label>
              <p className="text-sm font-medium text-gray-900">{selectedTask.name}</p>
            </div>
            {selectedTask.comment && (
              <div>
                <label className="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">Комментарий</label>
                <p className="text-sm text-gray-700">{selectedTask.comment}</p>
              </div>
            )}
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">Статус</label>
                <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${getStatusColor(selectedTask.status)}`}>
                  {getStatusLabel(selectedTask.status)}
                </span>
              </div>
              <div>
                <label className="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">Приоритет</label>
                <p className="text-sm text-gray-900">{selectedTask.priority}</p>
              </div>
              <div>
                <label className="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">Автор</label>
                <p className="text-sm text-gray-900">
                  {selectedTask.author.lastName} {selectedTask.author.firstName} {selectedTask.author.middleName || ''}
                </p>
              </div>
              <div>
                <label className="block text-xs font-medium text-gray-500 uppercase tracking-wider mb-1">Исполнитель</label>
                <p className="text-sm text-gray-900">
                  {selectedTask.assigned.lastName} {selectedTask.assigned.firstName} {selectedTask.assigned.middleName || ''}
                </p>
              </div>
            </div>
            <div className="flex justify-end gap-3 pt-2 border-t border-gray-100">
              <button
                onClick={() => openEditTaskModal(selectedTask)}
                className="inline-flex items-center gap-2 px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 transition-colors"
              >
                <HiPencil className="w-4 h-4" />
                Редактировать
              </button>
              <button
                onClick={() => openDeleteTaskConfirm(selectedTask)}
                className="inline-flex items-center gap-2 px-4 py-2 text-sm font-medium text-white bg-red-600 rounded-lg hover:bg-red-700 transition-colors"
              >
                <HiTrash className="w-4 h-4" />
                Удалить
              </button>
            </div>
          </div>
        )}
      </Modal>

      {/* Delete Task Confirmation Modal */}
      <Modal isOpen={isDeleteTaskModalOpen} onClose={() => { setIsDeleteTaskModalOpen(false); setTaskToDelete(null); }} title="Подтверждение удаления" size="sm">
        {taskToDelete && (
          <div className="space-y-4">
            <div className="flex items-start gap-3">
              <div className="flex-shrink-0 w-10 h-10 bg-red-100 rounded-full flex items-center justify-center">
                <HiExclamation className="w-5 h-5 text-red-600" />
              </div>
              <div>
                <p className="text-sm text-gray-700">
                  Вы уверены, что хотите удалить задачу <strong>«{taskToDelete.name}»</strong>?
                </p>
                <p className="text-xs text-gray-500 mt-1">Это действие нельзя отменить.</p>
              </div>
            </div>
            <div className="flex justify-end gap-3 pt-2">
              <button
                onClick={() => { setIsDeleteTaskModalOpen(false); setTaskToDelete(null); }}
                disabled={isSubmitting}
                className="px-4 py-2 text-sm font-medium text-gray-600 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 transition-colors disabled:opacity-50"
              >
                Отмена
              </button>
              <button
                onClick={handleDeleteTask}
                disabled={isSubmitting}
                className="px-4 py-2 text-sm font-medium text-white bg-red-600 rounded-lg hover:bg-red-700 transition-colors disabled:opacity-50 flex items-center gap-2"
              >
                {isSubmitting && (
                  <svg className="animate-spin h-4 w-4" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" />
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
                  </svg>
                )}
                <HiTrash className="w-4 h-4" />
                Удалить
              </button>
            </div>
          </div>
        )}
      </Modal>
    </div>
  );
}