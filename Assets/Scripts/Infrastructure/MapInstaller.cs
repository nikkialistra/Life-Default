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
        [SerializeField] private bool _loadSavedGraphData;
        [SerializeField] private TextAsset _graphData;

        [Space]
        [Required]
        [SerializeField] private AstarPath _astarPath;
        [Required]
        [SerializeField] private MapGenerator _mapGeneratorPrefab;
        [Required]
        [SerializeField] private Transform _mapParent;

        public override void InstallBindings()
        {
            Container.BindInstance(_astarPath);
            Container.BindInstance(_graphData);

            Container.BindInterfacesAndSelfTo<Map>().AsSingle().WithArguments(_loadSavedGraphData);

            Container.BindFactory<MapGenerator, MapGenerator.Factory>().FromSubContainerResolve()
                .ByNewPrefabMethod(GetMapGeneratorPrefab, MapGeneratorInstaller).UnderTransform(_mapParent);
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

        private static void MapGeneratorInstaller(DiContainer subContainer)
        {
            subContainer.Bind<MapGenerator>().FromComponentOnRoot();
        }
    }
}
