using UnityEngine;
using Zenject;

namespace Colonists.Services
{
    [RequireComponent(typeof(Colonist))]
    public class ColonistCounter : MonoBehaviour
    {
        private Colonist _colonist;

        private ColonistRepository _colonistRepository;

        [Inject]
        public void Construct(ColonistRepository colonistRepository)
        {
            _colonistRepository = colonistRepository;
        }

        private void Awake()
        {
            _colonist = GetComponent<Colonist>();
        }

        private void OnEnable()
        {
            _colonist.Spawn += OnSpawn;
            _colonist.Dying += OnDying;
        }

        private void OnDisable()
        {
            _colonist.Spawn -= OnSpawn;
            _colonist.Dying -= OnDying;
        }

        private void OnSpawn()
        {
            _colonistRepository.AddColonist(_colonist);
        }

        private void OnDying()
        {
            _colonistRepository.RemoveColonist(_colonist);
        }
    }
}
