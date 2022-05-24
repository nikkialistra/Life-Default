using UnityEngine;
using Zenject;

namespace Infrastructure.Settings
{
    [CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Installers/Game Settings Installer")]
    public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        [SerializeField] private RaycastingSettings _raycastingSettings;
        [SerializeField] private UnitsSettings _unitsSettings;
        [SerializeField] private SelectionSettings _selectionSettings;
        [SerializeField] private AttackSettings _attackSettings;
        [SerializeField] private AnimationSettings _animationSettings;
        [SerializeField] private VisibilityFieldsSettings _visibilityFieldsSettings;

        public override void InstallBindings()
        {
            Container.BindInstance(_raycastingSettings);
            Container.BindInstance(_unitsSettings);
            Container.BindInstance(_selectionSettings);
            Container.BindInstance(_attackSettings);
            Container.BindInstance(_animationSettings);
            Container.BindInstance(_visibilityFieldsSettings);
        }
    }
}
