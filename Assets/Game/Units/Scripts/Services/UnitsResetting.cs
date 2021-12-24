using System;
using Kernel.Saving;
using Kernel.Targeting;

namespace Game.Units.Services
{
    public class UnitsResetting : IDisposable
    {
        private SavingLoadingGame _savingLoadingGame;
        private UnitRepository _unitRepository;
        private UnitSelection _unitSelection;
        private TargetPool _targetPool;

        public UnitsResetting(SavingLoadingGame savingLoadingGame, UnitRepository unitRepository, UnitSelection unitSelection, TargetPool targetPool)
        {
            _unitSelection = unitSelection;
            _targetPool = targetPool;
            _unitRepository = unitRepository;
            _savingLoadingGame = savingLoadingGame;

            _savingLoadingGame.Loading += Reset;
        }

        private void Reset()
        {
            _targetPool.OffAll();
            _unitSelection.ClearSelection();
            _unitRepository.ResetObjects();
        }

        public void Dispose()
        {
            _savingLoadingGame.Loading -= Reset;
        }
    }
}