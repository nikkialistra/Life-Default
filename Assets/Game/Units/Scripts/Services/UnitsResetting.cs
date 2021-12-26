using System;
using Game.Units.Selecting;
using Kernel.Saving;
using Kernel.Targeting;

namespace Game.Units.Services
{
    public class UnitsResetting : IDisposable
    {
        private SavingLoadingGame _savingLoadingGame;
        private UnitsRepository _unitsRepository;
        private UnitsSelection _unitsSelection;
        private TargetPool _targetPool;

        public UnitsResetting(SavingLoadingGame savingLoadingGame, UnitsRepository unitsRepository, UnitsSelection unitsSelection, TargetPool targetPool)
        {
            _unitsSelection = unitsSelection;
            _targetPool = targetPool;
            _unitsRepository = unitsRepository;
            _savingLoadingGame = savingLoadingGame;

            _savingLoadingGame.Loading += Reset;
        }

        private void Reset()
        {
            _targetPool.OffAll();
            _unitsSelection.ClearSelection();
            _unitsRepository.ResetObjects();
        }

        public void Dispose()
        {
            _savingLoadingGame.Loading -= Reset;
        }
    }
}