using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Infrastructure
{
    public class BootstrapInstaller : MonoInstaller
    {
        [Required]
        [SerializeField] private PlayerInput _playerInput;

        public override void InstallBindings()
        {
            Container.BindInstance(_playerInput);
        }
    }
}
