using MapGeneration;
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
        [SerializeField] private Transform _mapParent;

        public override void InstallBindings()
        {
            Container.BindInstance(_astarPath);
            Container.BindInstance(_graphData);

            Container.BindInterfacesAndSelfTo<Map>().AsSingle().WithArguments(_loadSavedGraphData);
        }
    }
}
