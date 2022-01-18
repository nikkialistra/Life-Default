using Entities.Entity;
using Units.Services;
using UnityEngine;
using Zenject;

namespace Units.Unit.UnitType
{
    public class UnitClass : MonoBehaviour
    {
        private UnitClassSpecsRepository _unitClassSpecsRepository;
        public UnitClassSpecs UnitClassSpecs { get; private set; }

        [Inject]
        public void Construct(UnitClassSpecsRepository unitClassSpecsRepository)
        {
            _unitClassSpecsRepository = unitClassSpecsRepository;
        }

        public void ChangeUnitType(UnitType unitType)
        {
            UnitClassSpecs = _unitClassSpecsRepository.GetFor(unitType, UnitTypeLevel.FirstLevel);
        }

        public bool CanInteractWith(Entity entity)
        {
            var entityType = entity.EntityType;

            return UnitClassSpecs.ContainsSpecsFor(entityType);
        }
    }
}
