using Cameras;
using ResourceManagement;
using Saving;
using Saving.Serialization;
using Sirenix.OdinInspector;
using Testing;
using UI.Game;
using Units.Services;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Infrastructure
{
    public class SceneInstaller : MonoInstaller
    {
        [Title("Set Up")]
        [SerializeField] private bool _isSetUpSession;

        [Title("Input")]
        [Required]
        [SerializeField] private Camera _camera;
        [Required]
        [SerializeField] private CameraInputCombination _cameraInputCombination;
        [Required]
        [SerializeField] private FlyCamera _flyCamera;
        [Required]
        [SerializeField] private PlayerInput _playerInput;

        [Title("UI")]
        [Required]
        [SerializeField] private GameViews _gameViews;
        [Required]
        [SerializeField] private UnitTypesView _unitTypesView;
        [Required]
        [SerializeField] private InfoPanelView _infoPanelView;
        [Required]
        [SerializeField] private UnitInfoView _unitInfoView;
        [Required]
        [SerializeField] private UnitsInfoView _unitsInfoView;
        [Required]
        [SerializeField] private ResourcesView _resourcesView;

        [Title("Resources")]
        [Required]
        [SerializeField] private ResourcesCounts _resourcesCounts;

        [Title("Saving")]
        [Required]
        [SerializeField] private UnitSaveLoadHandler _unitSaveLoadHandler;
        [Required]
        [SerializeField] private SavingLoadingGame _savingLoadingGame;

        public override void InstallBindings()
        {
            BindTesting();
            BindInput();
            BindUi();
            BindResources();
            BindSaving();
        }

        private void BindTesting()
        {
            Container.BindInterfacesTo<UnitsGenerator>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<TogglingCameraMovement>().AsSingle().NonLazy();
        }

        private void BindInput()
        {
            Container.BindInstance(_camera);

            Container.BindInstance(_cameraInputCombination);
            Container.BindInstance(_isSetUpSession).WhenInjectedInto<CameraInputCombination>();

            Container.BindInstance(_flyCamera);
            Container.BindInstance(_isSetUpSession).WhenInjectedInto<FlyCamera>();

            Container.BindInstance(_playerInput);
        }

        private void BindUi()
        {
            Container.BindInstance(_gameViews);
            Container.BindInstance(_unitTypesView);
            Container.BindInstance(_infoPanelView);
            Container.BindInstance(_unitInfoView);
            Container.BindInstance(_unitsInfoView);
            Container.BindInstance(_resourcesView);
        }

        private void BindResources()
        {
            Container.BindInstance(_resourcesCounts);
        }

        private void BindSaving()
        {
            Container.Bind<SaveData>().AsSingle();
            Container.BindInstance(_unitSaveLoadHandler);
            Container.Bind<Serialization>().AsSingle();
            Container.BindInstance(_savingLoadingGame);
            Container.BindInterfacesTo<UnitResetting>().AsSingle().NonLazy();
        }
    }
}
