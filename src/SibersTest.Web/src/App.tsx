import { useState } from 'react';
import { HiPlus } from 'react-icons/hi';
import ProjectList from './components/ProjectList'
import Modal from './components/Modal';
import ProjectWizard from './components/ProjectWizard';

function App() {
  const [isWizardOpen, setIsWizardOpen] = useState(false);
  const [refreshKey, setRefreshKey] = useState(0);

  const handleWizardSuccess = () => {
    setIsWizardOpen(false);
    setRefreshKey(prev => prev + 1);
  };

  return (
    <>
      <div className="fixed bottom-6 right-6 z-40">
        <button
          onClick={() => setIsWizardOpen(true)}
          className="w-14 h-14 bg-blue-600 text-white rounded-full shadow-lg hover:bg-blue-700 transition-colors flex items-center justify-center"
          title="Создать проект"
        >
          <HiPlus className="w-6 h-6" />
        </button>
      </div>

      <ProjectList key={refreshKey} />

      <Modal isOpen={isWizardOpen} onClose={() => setIsWizardOpen(false)} title="Новый проект" size="lg">
        <ProjectWizard
          onClose={() => setIsWizardOpen(false)}
          onSuccess={handleWizardSuccess}
        />
      </Modal>
    </>
  )
}

export default App