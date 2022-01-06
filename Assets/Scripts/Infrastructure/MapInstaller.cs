using MapGeneration.Generators;
using MapGeneration.Map;
using MapGeneration.Saving;
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
            Container.BindInterfacesAndSelfTo<Map>().AsSingle();

            Container.BindFactory<MapGenerator, MapGenerator.Factory>().FromSubContainerResolve()
                .ByNewPrefabMethod(GetMapGeneratorPrefab, InstallerMethod).UnderTransform(_mapParent);
        }

        private Object GetMapGeneratorPrefab(InjectContext context)
        {
            if (_tryLoadFromSaved)
            {
                var prefab = TryLoad();
                if (prefab != null)
                {
                    return prefab;
                }
            }

            return _mapGeneratorPrefab;
        }

        private static GameObject TryLoad()
        {
            var savedPrefab = MapSaving.GetSavedPrefab("Map");

            if (savedPrefab != null)
            {
                return savedPrefab;
            }

            return null;
        }

        private static void InstallerMethod(DiContainer subContainer)
        {
            subContainer.Bind<MapGenerator>().FromComponentOnRoot();
        }
    }
}