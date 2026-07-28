import { useState, useEffect, useRef, useCallback } from 'react';
import { HiSearch, HiX } from 'react-icons/hi';
import type { EmployeeResponse } from '../types/project';
import { employeesApi } from '../services/api';

interface EmployeeAutocompleteProps {
  value: number | null;
  onChange: (employeeId: number | null) => void;
  placeholder?: string;
  label: string;
  required?: boolean;
  error?: string;
  excludeIds?: number[];
}

export default function EmployeeAutocomplete({
  value,
  onChange,
  placeholder = 'Начните вводить имя сотрудника...',
  label,
  required = false,
  error,
  excludeIds = [],
}: EmployeeAutocompleteProps) {
  const [query, setQuery] = useState('');
  const [results, setResults] = useState<EmployeeResponse[]>([]);
  const [isOpen, setIsOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [selectedEmployee, setSelectedEmployee] = useState<EmployeeResponse | null>(null);
  const [debounceTimer, setDebounceTimer] = useState<ReturnType<typeof setTimeout> | null>(null);
  const wrapperRef = useRef<HTMLDivElement>(null);
  const inputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    if (value && !selectedEmployee) {
      employeesApi.getAll().then(all => {
        const found = all.find(e => e.id === value);
        if (found) {
          setSelectedEmployee(found);
          setQuery(`${found.lastName} ${found.firstName} ${found.middleName || ''}`.trim());
        }
      }).catch(() => {});
    }
  }, [value, selectedEmployee]);

  useEffect(() => {
    const handleClickOutside = (e: MouseEvent) => {
      if (wrapperRef.current && !wrapperRef.current.contains(e.target as Node)) {
        setIsOpen(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  const searchEmployees = useCallback(async (term: string) => {
    if (term.length < 1) {
      setResults([]);
      setIsOpen(false);
      return;
    }

    setIsLoading(true);
    try {
      const data = await employeesApi.search(term);
      const filtered = data.filter(e => !excludeIds.includes(e.id));
      setResults(filtered);
      setIsOpen(filtered.length > 0 || term.length > 0);
    } catch {
      setResults([]);
    } finally {
      setIsLoading(false);
    }
  }, [excludeIds]);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const val = e.target.value;
    setQuery(val);

    if (debounceTimer) clearTimeout(debounceTimer);

    if (selectedEmployee) {
      setSelectedEmployee(null);
      onChange(null);
    }

    const timer = setTimeout(() => {
      searchEmployees(val);
    }, 300);
    setDebounceTimer(timer);
  };

  const handleSelect = (employee: EmployeeResponse) => {
    setSelectedEmployee(employee);
    setQuery(`${employee.lastName} ${employee.firstName} ${employee.middleName || ''}`.trim());
    onChange(employee.id);
    setIsOpen(false);
  };

  const handleClear = () => {
    setQuery('');
    setSelectedEmployee(null);
    onChange(null);
    setResults([]);
    setIsOpen(false);
    inputRef.current?.focus();
  };

  const handleFocus = () => {
    if (query.length >= 1) {
      setIsOpen(true);
    }
  };

  const inputClass = `w-full pl-10 pr-10 py-2 border rounded-lg text-sm transition-colors focus:outline-none focus:ring-2 ${
    error
      ? 'border-red-400 focus:ring-red-200 bg-red-50'
      : 'border-gray-300 focus:ring-blue-200 focus:border-blue-400'
  }`;

  return (
    <div ref={wrapperRef} className="relative">
      <label className="block text-sm font-medium text-gray-700 mb-1">
        {label} {required && <span className="text-red-500">*</span>}
      </label>
      <div className="relative">
        <HiSearch className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
        <input
          ref={inputRef}
          type="text"
          value={query}
          onChange={handleInputChange}
          onFocus={handleFocus}
          placeholder={placeholder}
          className={inputClass}
          autoComplete="off"
        />
        {query && (
          <button
            type="button"
            onClick={handleClear}
            className="absolute right-3 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600 transition-colors"
          >
            <HiX className="w-4 h-4" />
          </button>
        )}
      </div>
      {error && <p className="mt-1 text-xs text-red-500">{error}</p>}

      {/* Dropdown */}
      {isOpen && (
        <div className="absolute z-50 mt-1 w-full bg-white border border-gray-200 rounded-lg shadow-lg max-h-60 overflow-y-auto">
          {isLoading ? (
            <div className="flex items-center justify-center py-4">
              <svg className="animate-spin h-5 w-5 text-blue-600" viewBox="0 0 24 24">
                <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" />
                <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
              </svg>
            </div>
          ) : results.length === 0 ? (
            <div className="px-4 py-3 text-sm text-gray-500 text-center">
              {query.length < 1 ? 'Введите минимум 1 символ' : 'Сотрудники не найдены'}
            </div>
          ) : (
            results.map(emp => (
              <button
                key={emp.id}
                type="button"
                onClick={() => handleSelect(emp)}
                className={`w-full text-left px-4 py-2.5 text-sm hover:bg-blue-50 transition-colors flex items-center gap-3 ${
                  value === emp.id ? 'bg-blue-50 text-blue-700' : 'text-gray-700'
                }`}
              >
                <div className="w-8 h-8 bg-gray-100 rounded-full flex items-center justify-center text-xs font-medium text-gray-600 flex-shrink-0">
                  {emp.firstName[0]}{emp.lastName[0]}
                </div>
                <div>
                  <p className="font-medium">
                    {emp.lastName} {emp.firstName} {emp.middleName || ''}
                  </p>
                  <p className="text-xs text-gray-400">{emp.email}</p>
                </div>
              </button>
            ))
          )}
        </div>
      )}
    </div>
  );
}