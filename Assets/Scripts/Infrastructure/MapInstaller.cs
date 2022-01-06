using MapGeneration;
using MapGeneration.Generators;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class MapInstaller : MonoInstaller
    {
        [SerializeField] private bool _tryLoadFromSaved;

        [Space]
        [Required]
        [SerializeField] private MapGenerator _mapGeneratorPrefab;
        [Required]
        [SerializeField] private Transform _mapParent;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<Map>().AsSingle();

            Container.BindFactory<MapGenerator, MapGenerator.Factory>().FromSubContainerResolve()
                .ByNewPrefabMethod(GetMapGeneratorPrefab, InstallerMethod).UnderTransform(_mapParent);
        }

        private Object GetMapGeneratorPrefab(InjectContext context)
        {
            if (_tryLoadFromSaved)
            {
                return _mapGeneratorPrefab;
            }
            else
            {
                return _mapGeneratorPrefab;
            }
        }

        private void InstallerMethod(DiContainer subContainer)
        {
            subContainer.Bind<MapGenerator>().FromComponentOnRoot();
        }
    }
}