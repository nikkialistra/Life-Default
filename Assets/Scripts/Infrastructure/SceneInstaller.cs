using Cameras;
using ResourceManagement;
using Saving;
using Saving.Serialization;
using Sirenix.OdinInspector;
using Testing;
using UI;
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
        [SerializeField] private GameCursors _gameCursors;
        [Required]
        [SerializeField] private Camera _camera;
        [Required]
        [SerializeField] private CameraMovement _cameraMovement;
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
        [SerializeField] private ResourceCounts _resourceCounts;

        [Title("Saving")]
        [Required]
        [SerializeField] private UnitSaveLoadHandler _unitSaveLoadHandler;
        [Required]
        [SerializeField] private SavingLoadingGame _savingLoadingGame;

        public override void InstallBindings()
        {
            BindInput();
            BindUi();
            BindResources();
            BindSaving();
        }

        private void BindInput()
        {
            Container.BindInstance(_gameCursors);

            Container.BindInstance(_camera);

            Container.BindInstance(_cameraMovement);
            Container.BindInstance(_isSetUpSession).WhenInjectedInto<CameraMovement>();

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
            Container.BindInstance(_resourceCounts);
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
