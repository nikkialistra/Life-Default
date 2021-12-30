using System;
using Saving;
using UnitManagement.Targeting;
using Units.Services.Selecting;

namespace Units.Services
{
    public class UnitsResetting : IDisposable
    {
        private readonly SavingLoadingGame _savingLoadingGame;
        private readonly SelectedUnits _selectedUnits;
        private readonly TargetPool _targetPool;

        public UnitsResetting(SavingLoadingGame savingLoadingGame, SelectedUnits selectedUnits, TargetPool targetPool)
        {
            _selectedUnits = selectedUnits;
            _targetPool = targetPool;
            _savingLoadingGame = savingLoadingGame;

            _savingLoadingGame.Loading += Reset;
        }

        private void Reset()
        {
            _targetPool.OffAll();
            _selectedUnits.Clear();
        }

        public void Dispose()
        {
            _savingLoadingGame.Loading -= Reset;
        }
    }
}