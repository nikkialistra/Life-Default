using Game.Units.Scripts.Services;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class SceneInstaller : MonoInstaller
    {
        [Header("Base")]
        [SerializeField] private Camera _camera;
        
        [Header("Unit Handling")]
        [SerializeField] private UnitRepository _unitRepository;
        // [SerializeField] private PointObjectPool _pool;
        // [SerializeField] private GameObject _template;
        
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);

            Container.BindInstance(_camera).AsSingle();
            
            BindUnitHandling();
        }

        private void BindUnitHandling()
        {
            Container.BindInstance(_unitRepository).AsSingle();
            // Container.BindInstance(_pool).AsSingle();
            // Container.BindInstance(_template).WhenInjectedInto<PointObjectPool>();
        }
    }
}