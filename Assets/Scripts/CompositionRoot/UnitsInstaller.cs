using Aborigines;
using Aborigines.Services;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace CompositionRoot
{
    public class UnitsInstaller : MonoInstaller
    {
        [Required]
        [SerializeField] private GameObject _aboriginePrefab;
        [Required]
        [SerializeField] private Transform _aboriginesParent;
        [Required]
        [SerializeField] private AborigineRepository _aborigineRepository;

        public override void InstallBindings()
        {
            BindSpawning();

            Container.BindInstance(_aborigineRepository);
        }

        private void BindSpawning()
        {
            Container.BindFactory<Aborigine, Aborigine.Factory>()
                .FromComponentInNewPrefab(_aboriginePrefab)
                .UnderTransform(_aboriginesParent);
        }
    }
}
