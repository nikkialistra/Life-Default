using Colonists.Colonist;
using UnityEngine;
using Zenject;

namespace Colonists.Services
{
    [RequireComponent(typeof(ColonistFacade))]
    public class ColonistCounter : MonoBehaviour
    {
        private ColonistFacade _colonist;

        private ColonistRepository _colonistRepository;

        [Inject]
        public void Construct(ColonistRepository colonistRepository)
        {
            _colonistRepository = colonistRepository;
        }

        private void Awake()
        {
            _colonist = GetComponent<ColonistFacade>();
        }

        private void OnEnable()
        {
            _colonist.Spawn += OnSpawn;
            _colonist.Die += OnDie;
        }

        private void OnDisable()
        {
            _colonist.Spawn -= OnSpawn;
            _colonist.Die -= OnDie;
        }

        private void OnSpawn()
        {
            _colonistRepository.AddColonist(_colonist);
        }

        private void OnDie()
        {
            _colonistRepository.RemoveColonist(_colonist);
        }
    }
}
