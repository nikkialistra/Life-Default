using System;
using Saving;
using UnitManagement.Targeting;
using Units.Services.Selecting;

namespace Units.Services
{
    public class UnitResetting : IDisposable
    {
        private readonly SavingLoadingGame _savingLoadingGame;
        private readonly SelectedUnits _selectedUnits;
        private readonly TargetMarkPool _targetMarkPool;

        public UnitResetting(SavingLoadingGame savingLoadingGame, SelectedUnits selectedUnits, TargetMarkPool targetMarkPool)
        {
            _selectedUnits = selectedUnits;
            _targetMarkPool = targetMarkPool;
            _savingLoadingGame = savingLoadingGame;

            _savingLoadingGame.Loading += Reset;
        }

        private void Reset()
        {
            _targetMarkPool.OffAll();
            _selectedUnits.Clear();
        }

        public void Dispose()
        {
            _savingLoadingGame.Loading -= Reset;
        }
    }
}
