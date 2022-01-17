using ResourceManagement;
using Units.Unit.UnitTypes;

namespace Entities
{
    public interface IInteractable
    {
        int Health { get; }
        bool CanInteractWith(UnitType unitType);
        ResourceOutput TakeDamage(int value);
    }
}
