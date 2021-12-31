using Units.Services;
using UnityEngine;
using Zenject;

namespace Units.Unit
{
    [RequireComponent(typeof(UnitFacade))]
    public class UnitCounter : MonoBehaviour
    {
        private UnitFacade _unitFacade;
        
        private UnitsRepository _unitsRepository;

        [Inject]
        public void Construct(UnitsRepository unitsRepository)
        {
            _unitsRepository = unitsRepository;
        }

        private void Awake()
        {
            _unitFacade = GetComponent<UnitFacade>();
        }

        private void OnEnable()
        {
            _unitFacade.Spawn += OnSpawn;
            _unitFacade.Die += OnDie;
        }

        private void OnDisable()
        {
            _unitFacade.Spawn -= OnSpawn;
            _unitFacade.Die -= OnDie;
        }

        private void OnSpawn()
        {
            _unitsRepository.AddUnit(_unitFacade);
        }

        private void OnDie()
        {
            _unitsRepository.RemoveUnit(_unitFacade);
        }
    }
}