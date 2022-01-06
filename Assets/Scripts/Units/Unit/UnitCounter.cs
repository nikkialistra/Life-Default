using Units.Services;
using UnityEngine;
using Zenject;

namespace Units.Unit
{
    [RequireComponent(typeof(UnitFacade))]
    public class UnitCounter : MonoBehaviour
    {
        private UnitFacade _unitFacade;

        private UnitRepository _unitRepository;

        [Inject]
        public void Construct(UnitRepository unitRepository)
        {
            _unitRepository = unitRepository;
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
            _unitRepository.AddUnit(_unitFacade);
        }

        private void OnDie()
        {
            _unitRepository.RemoveUnit(_unitFacade);
        }
    }
}