using System;
using Game.Units.Selecting;
using Kernel.Saving;
using Kernel.Targeting;

namespace Game.Units.Services
{
    public class UnitsResetting : IDisposable
    {
        private readonly SavingLoadingGame _savingLoadingGame;
        private readonly UnitsSelection _unitsSelection;
        private readonly TargetPool _targetPool;

        public UnitsResetting(SavingLoadingGame savingLoadingGame, UnitsSelection unitsSelection, TargetPool targetPool)
        {
            _unitsSelection = unitsSelection;
            _targetPool = targetPool;
            _savingLoadingGame = savingLoadingGame;

            _savingLoadingGame.Loading += Reset;
        }

        private void Reset()
        {
            _targetPool.OffAll();
            _unitsSelection.ClearSelection();
        }

        public void Dispose()
        {
            _savingLoadingGame.Loading -= Reset;
        }
    }
}