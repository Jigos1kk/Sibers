import { useState, useRef, useCallback } from 'react';
import { HiUpload, HiX, HiDocument, HiPhotograph, HiDocumentText } from 'react-icons/hi';

interface FileUploaderProps {
  files: File[];
  onFilesChange: (files: File[]) => void;
  maxFiles?: number;
  maxSizeMB?: number;
  accept?: string;
}

const FILE_ICONS: Record<string, React.ReactNode> = {
  pdf: <HiDocumentText className="w-5 h-5 text-red-500" />,
  doc: <HiDocumentText className="w-5 h-5 text-blue-500" />,
  docx: <HiDocumentText className="w-5 h-5 text-blue-500" />,
  xls: <HiDocumentText className="w-5 h-5 text-green-500" />,
  xlsx: <HiDocumentText className="w-5 h-5 text-green-500" />,
  txt: <HiDocumentText className="w-5 h-5 text-gray-500" />,
  jpg: <HiPhotograph className="w-5 h-5 text-purple-500" />,
  jpeg: <HiPhotograph className="w-5 h-5 text-purple-500" />,
  png: <HiPhotograph className="w-5 h-5 text-purple-500" />,
  gif: <HiPhotograph className="w-5 h-5 text-purple-500" />,
};

function getFileIcon(fileName: string): React.ReactNode {
  const ext = fileName.split('.').pop()?.toLowerCase() || '';
  return FILE_ICONS[ext] || <HiDocument className="w-5 h-5 text-gray-500" />;
}

function formatFileSize(bytes: number): string {
  if (bytes < 1024) return `${bytes} Б`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} КБ`;
  return `${(bytes / (1024 * 1024)).toFixed(1)} МБ`;
}

export default function FileUploader({
  files,
  onFilesChange,
  maxFiles = 10,
  maxSizeMB = 10,
  accept = '.pdf,.doc,.docx,.xls,.xlsx,.txt,.jpg,.jpeg,.png,.gif',
}: FileUploaderProps) {
  const [isDragOver, setIsDragOver] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const inputRef = useRef<HTMLInputElement>(null);

  const maxSizeBytes = maxSizeMB * 1024 * 1024;

  const validateAndAddFiles = useCallback((newFiles: FileList | File[]) => {
    setError(null);
    const fileArray = Array.from(newFiles);

    if (files.length + fileArray.length > maxFiles) {
      setError(`Максимальное количество файлов: ${maxFiles}`);
      return;
    }

    const validFiles: File[] = [];
    const allowedExtensions = accept.split(',').map(ext => ext.trim().toLowerCase());

    for (const file of fileArray) {
      const ext = '.' + (file.name.split('.').pop()?.toLowerCase() || '');
      if (!allowedExtensions.includes(ext) && accept !== '*') {
        setError(`Файл "${file.name}" имеет недопустимый формат`);
        continue;
      }

      if (file.size > maxSizeBytes) {
        setError(`Файл "${file.name}" превышает максимальный размер ${maxSizeMB} МБ`);
        continue;
      }

      validFiles.push(file);
    }

    if (validFiles.length > 0) {
      onFilesChange([...files, ...validFiles]);
    }
  }, [files, onFilesChange, maxFiles, maxSizeBytes, maxSizeMB, accept]);

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragOver(true);
  };

  const handleDragLeave = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragOver(false);
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragOver(false);

    if (e.dataTransfer.files && e.dataTransfer.files.length > 0) {
      validateAndAddFiles(e.dataTransfer.files);
    }
  };

  const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      validateAndAddFiles(e.target.files);
      e.target.value = '';
    }
  };

  const removeFile = (index: number) => {
    onFilesChange(files.filter((_, i) => i !== index));
    setError(null);
  };

  return (
    <div className="space-y-3">
      <label className="block text-sm font-medium text-gray-700 mb-1">
        Документы проекта
      </label>

      {/* Drop Zone */}
      <div
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        onDrop={handleDrop}
        onClick={() => inputRef.current?.click()}
        className={`relative border-2 border-dashed rounded-xl p-8 text-center cursor-pointer transition-all ${
          isDragOver
            ? 'border-blue-400 bg-blue-50'
            : 'border-gray-300 hover:border-blue-300 hover:bg-gray-50'
        }`}
      >
        <input
          ref={inputRef}
          type="file"
          multiple
          accept={accept}
          onChange={handleFileSelect}
          className="hidden"
        />

        <div className="flex flex-col items-center gap-2">
          <div className={`p-3 rounded-full transition-colors ${
            isDragOver ? 'bg-blue-100' : 'bg-gray-100'
          }`}>
            <HiUpload className={`w-8 h-8 transition-colors ${
              isDragOver ? 'text-blue-500' : 'text-gray-400'
            }`} />
          </div>
          <div>
            <p className="text-sm font-medium text-gray-700">
              {isDragOver ? 'Отпустите файлы для загрузки' : 'Перетащите файлы сюда или нажмите для выбора'}
            </p>
            <p className="text-xs text-gray-400 mt-1">
              {accept !== '*' ? `Допустимые форматы: ${accept}` : 'Любые форматы'}
              {' · '}Макс. {maxSizeMB} МБ · До {maxFiles} файлов
            </p>
          </div>
        </div>
      </div>

      {/* Error */}
      {error && (
        <div className="flex items-center gap-2 px-3 py-2 bg-red-50 border border-red-200 rounded-lg text-sm text-red-600">
          <HiX className="w-4 h-4 flex-shrink-0" />
          {error}
        </div>
      )}

      {/* File List */}
      {files.length > 0 && (
        <div className="space-y-2">
          <p className="text-xs font-medium text-gray-500 uppercase tracking-wider">
            Выбрано файлов: {files.length}
          </p>
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-2">
            {files.map((file, index) => (
              <div
                key={`${file.name}-${index}`}
                className="flex items-center gap-3 px-3 py-2.5 bg-gray-50 rounded-lg border border-gray-200 group"
              >
                {getFileIcon(file.name)}
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium text-gray-700 truncate">{file.name}</p>
                  <p className="text-xs text-gray-400">{formatFileSize(file.size)}</p>
                </div>
                <button
                  type="button"
                  onClick={(e) => {
                    e.stopPropagation();
                    removeFile(index);
                  }}
                  className="p-1 text-gray-400 hover:text-red-500 hover:bg-red-50 rounded transition-colors opacity-0 group-hover:opacity-100"
                >
                  <HiX className="w-4 h-4" />
                </button>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}