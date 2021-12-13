using Game.Units.Scripts;
using Kernel.Selection;
using Kernel.Targeting;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class UnitsInstaller : MonoInstaller
    {
        [Header("Selection")] 
        [SerializeField] private SelectionInput _selectionInput;

        [SerializeField] private RectTransform _selectionRect;
        [SerializeField] private Canvas _uiCanvas;
        
        [Header("Targeting")]
        [SerializeField] private MovementCommand _movementCommand;
        
        [Header("Unit")] [SerializeField]
        private GameObject _unitPrefab;
        
        public override void InstallBindings()
        {
            BindUnitSelectionSystem();
            
            Container.Bind<ProjectionSelector>().AsSingle();
            
            Container.BindInstance(_movementCommand);

            BindUnit();
        }

        private void BindUnitSelectionSystem()
        {
            Container.BindInterfacesAndSelfTo<UnitSelection>().AsSingle().NonLazy();
            Container.BindInstance(_selectionInput);
            Container.Bind<SelectionArea>().AsSingle().WithArguments(_selectionRect, _uiCanvas);
        }

        private void BindUnit()
        {
            Container.Bind<Unit>().FromSubContainerResolve().ByMethod(InstallUnit).AsSingle();
            
            Container.BindFactory<Unit, Unit.Factory>().FromSubContainerResolve()
                .ByNewContextPrefab(_unitPrefab);
        
            Container.BindInterfacesTo<UnitGenerator>().AsSingle();
        }

        private void InstallUnit(DiContainer subContainer) => subContainer.Bind<Unit>().AsSingle();
    }
}