using System;
using Saving;
using UnitManagement.OrderMarks;
using Units.Services.Selecting;

namespace Units.Services
{
    public class UnitResetting : IDisposable
    {
        private readonly SavingLoadingGame _savingLoadingGame;
        private readonly SelectedUnits _selectedUnits;
        private readonly OrderMarkPool _orderMarkPool;

        public UnitResetting(SavingLoadingGame savingLoadingGame, SelectedUnits selectedUnits,
            OrderMarkPool orderMarkPool)
        {
            _selectedUnits = selectedUnits;
            _orderMarkPool = orderMarkPool;
            _savingLoadingGame = savingLoadingGame;

            _savingLoadingGame.Loading += Reset;
        }

        private void Reset()
        {
            _orderMarkPool.OffAll();
            _selectedUnits.Clear();
        }

        public void Dispose()
        {
            _savingLoadingGame.Loading -= Reset;
        }
    }
}
