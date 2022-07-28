using Aborigines.Services;
using UnityEngine;
using Zenject;

namespace Aborigines
{
    [RequireComponent(typeof(Aborigine))]
    public class AborigineCounter : MonoBehaviour
    {
        private Aborigine _aborigine;

        private AborigineRepository _aborigineRepository;

        [Inject]
        public void Construct(AborigineRepository aborigineRepository)
        {
            _aborigineRepository = aborigineRepository;
        }

        private void Awake()
        {
            _aborigine = GetComponent<Aborigine>();
        }

        private void OnEnable()
        {
            _aborigine.Spawn += OnSpawn;
            _aborigine.Dying += OnDying;
        }

        private void OnDisable()
        {
            _aborigine.Spawn -= OnSpawn;
            _aborigine.Dying -= OnDying;
        }

        private void OnSpawn()
        {
            _aborigineRepository.AddAborigine(_aborigine);
        }

        private void OnDying()
        {
            _aborigineRepository.RemoveAborigine(_aborigine);
        }
    }
}
