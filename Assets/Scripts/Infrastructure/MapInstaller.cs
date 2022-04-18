using General;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class MapInstaller : MonoInstaller
    {
        [Required]
        [SerializeField] private Map _map;

        [Space]
        [Required]
        [SerializeField] private AstarPath _astarPath;

        public override void InstallBindings()
        {
            Container.BindInstance(_map);
            
            Container.BindInstance(_astarPath);
        }
    }
}
