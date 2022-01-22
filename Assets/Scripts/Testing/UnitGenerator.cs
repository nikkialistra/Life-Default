using Common;
using Units.Unit;
using Units.Unit.UnitTypes;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Testing
{
    public class UnitGenerator : MonoBehaviour
    {
        private UnitType? _generationType = UnitType.Lumberjack;
        private LayerMask _terrainMask;
        private UnitFacade.Factory _factory;

        private Camera _camera;

        private PlayerInput _playerInput;

        private InputAction _digitAction;
        private InputAction _selectAction;
        private InputAction _positionAction;

        [Inject]
        public void Construct(UnitFacade.Factory factory, Camera camera, PlayerInput playerInput)
        {
            _factory = factory;
            _camera = camera;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _terrainMask = LayerMask.GetMask("Terrain");

            _selectAction = _playerInput.actions.FindAction("Select");
            _positionAction = _playerInput.actions.FindAction("Position");

            _digitAction = _playerInput.actions.FindAction("Digit");
        }

        private void OnEnable()
        {
            _selectAction.started += GenerateUnit;
            _digitAction.started += CheckForSwitchingGenerationTypeCommand;
        }

        private void OnDisable()
        {
            _selectAction.started -= GenerateUnit;
            _digitAction.started -= CheckForSwitchingGenerationTypeCommand;
        }

        private void CheckForSwitchingGenerationTypeCommand(InputAction.CallbackContext context)
        {
            var digit = Mathf.RoundToInt(context.ReadValue<float>());

            _generationType = digit switch
            {
                1 => UnitType.Scout,
                2 => UnitType.Lumberjack,
                3 => UnitType.Mason,
                4 => UnitType.Melee,
                5 => UnitType.Archer,
                _ => _generationType
            };
        }

        private void GenerateUnit(InputAction.CallbackContext context)
        {
            if (!Keyboard.current.altKey.isPressed)
            {
                return;
            }

            var position = _positionAction.ReadValue<Vector2>();

            var ray = _camera.ScreenPointToRay(new Vector3(position.x, position.y, _camera.nearClipPlane));
            if (Physics.Raycast(ray, out var hit, float.PositiveInfinity, _terrainMask))
            {
                var unitType = GetUnitType();
                _factory.Create(unitType, hit.point);
            }
        }

        private UnitType GetUnitType()
        {
            if (_generationType != null)
            {
                return (UnitType)_generationType;
            }
            else
            {
                return EnumUtils.RandomEnumValue<UnitType>();
            }
        }
    }
}
