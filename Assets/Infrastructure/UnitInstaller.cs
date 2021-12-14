using Game.Units;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class UnitInstaller : MonoInstaller
    {
        [SerializeField] private GameObject _selectionIndicator;
        [SerializeField] private float _distanceToGroup;

        [SerializeField] private UnitFacade _unitFacade;

        public override void InstallBindings()
        {
            Container.BindInstance(_unitFacade).AsSingle();
            Container.BindInstance(_selectionIndicator).WhenInjectedInto<UnitFacade>();
            Container.BindInstance(_distanceToGroup).WhenInjectedInto<UnitMeshAgent>();
        }
    }
}