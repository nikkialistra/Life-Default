using System;
using System.Collections;
using Colonists;
using DG.Tweening;
using General.Map;
using General.Selection;
using Saving;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace General
{
    [RequireComponent(typeof(Camera))]
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private bool _deactivateAtStartup = true;

        [Title("Movement")]
        [SerializeField] private float _moveSpeed;

        [Space]
        [Range(0.5f, 1f)]
        [SerializeField] private float _positionMoveXThreshold;
        [Range(0.5f, 1f)]
        [SerializeField] private float _positionMoveYThreshold;

        [Title("Rotation")]
        [SerializeField] private float _rotateHorizontalSpeed;
        [SerializeField] private float _rotateVerticalSpeed;

        [SerializeField] private float _minVerticalRotation;
        [SerializeField] private float _maxVerticalRotation;

        [Title("Zoom")]
        [SerializeField] private float _zoomScrollSensitivity;
        [SerializeField] private float _zoomButtonSensitivity;

        [SerializeField] private float _minFov;
        [SerializeField] private float _maxFov;

        [Title("Boundaries")]
        [SerializeField] private float _minimumPositionX;
        [SerializeField] private float _maximumPositionX;

        [Space]
        [SerializeField] private float _minimumPositionZ;
        [SerializeField] private float _maximumPositionZ;
        
        [Title("Focusing")]
        [SerializeField] private float _focusFov = 40f;
        [SerializeField] private float _focusDistance = 30f;
        [SerializeField] private float _focusRotation = 45f;
        [SerializeField] private float _focusDuration = .3f;
        [Space]
        [SerializeField] private float _minDistanceForTeleporation;

        [Title("Smoothing")]
        [SerializeField] private float _positionSmoothing;
        [SerializeField] private float _rotationSmoothing;
        [SerializeField] private float _zoomSmoothing;

        private float _horizontalRotation;
        private float _verticalRotation;
        private float _heightAboveTerrain;

        private Camera _camera;

        private float _lastMousePositionX;
        private float _lastMousePositionY;

        private float _deltaMousePositionX;
        private float _deltaMousePositionY;

        private Vector3 _newPosition;
        private Quaternion _newRotation;
        private float _newFieldOfView;

        private MapInitialization _mapInitialization;
        private bool _activated;
        private bool _canMouseScroll;

        private Colonist _colonist;
        private Transform _followTransform;
        private Vector3 _offset;
        private bool _following;

        private bool _focusing;

        private float _cameraSensitivity;
        private bool _screenEdgeMouseScroll;
        private bool _isSelectingInput;

        private LayerMask _terrainMask;
        private float _raiseDistance;

        private GameSettings _gameSettings;

        private Coroutine _focusingCoroutine;

        private SelectionInput _selectionInput;

        private Vector3 _originPositionCorrection;
        
        private PlayerInput _playerInput;

        private InputAction _movementAction;
        private InputAction _dragAction;
        private InputAction _mousePositionAction;
        private InputAction _zoomScrollAction;
        private InputAction _zoomButtonAction;

        private InputAction _setFollowAction;

        private InputAction _toggleCameraMovementAction;

        [Inject]
        public void Construct(bool isSetUpSession, MapInitialization mapInitialization, GameSettings gameSettings, SelectionInput selectionInput, PlayerInput playerInput)
        {
            _mapInitialization = mapInitialization;

            _gameSettings = gameSettings;
            _selectionInput = selectionInput;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            
            _terrainMask = LayerMask.GetMask("Terrain");

            _movementAction = _playerInput.actions.FindAction("Movement");
            _dragAction = _playerInput.actions.FindAction("Drag");
            _mousePositionAction = _playerInput.actions.FindAction("Mouse Position");
            _zoomScrollAction = _playerInput.actions.FindAction("Zoom Scroll");
            _zoomButtonAction = _playerInput.actions.FindAction("Zoom Button");

            _setFollowAction = _playerInput.actions.FindAction("Set Follow");

            _toggleCameraMovementAction = _playerInput.actions.FindAction("Toggle Camera Movement");
        }

        private void OnEnable()
        {
            _toggleCameraMovementAction.started += ToggleCameraMovement;

            _selectionInput.Selecting += OnSelecting;
            _selectionInput.SelectingEnd += OnSelectingEnd;
        }

        private void OnDisable()
        {
            _toggleCameraMovementAction.started -= ToggleCameraMovement;
            
            _selectionInput.Selecting -= OnSelecting;
            _selectionInput.SelectingEnd -= OnSelectingEnd;
        }

        private void Start()
        {
            _mapInitialization.Load += OnMapInitializationLoad;

            _heightAboveTerrain = GetDistanceAboveTerrain();

            _newPosition = transform.position;
            _newRotation = transform.rotation;
            _newFieldOfView = _camera.fieldOfView;
            
            _originPositionCorrection = GlobalParameters.Instance.OriginPositionCorrection;

            LoadSettings();
            Activate();
        }

        private void LateUpdate()
        {
            if (!_activated)
            {
                return;
            }

            CalculateDeltas();

            UpdateFollowing();
            if (_following)
            {
                UpdateFollow();
            }
            else
            {
                UpdatePosition();
                UpdatePositionFromMouseThresholdMovement();
                UpdateRotation();
            }

            UpdateZoom();

            ClampPositionByConstraints();

            SmoothUpdate();
        }

        public void DeactivateMovement()
        {
            _activated = false;
            _canMouseScroll = false;
            Deactivate();
        }

        public void ActivateMovement()
        {
            _activated = true;
            Activate();
            StartCoroutine(AllowMouseScrollALittleLater());
        }

        public void FocusOn(Colonist colonist)
        {
            ResetFollow();
            
            var yRotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, 0));
            var forward = yRotation * Vector3.forward;

            var position = colonist.Center + (forward * -_focusDistance);
            position = RaiseAboveTerrain(position);

            var eulerAngles = new Vector3(_focusRotation, _newRotation.eulerAngles.y, _newRotation.eulerAngles.z);

            StartFocusing(colonist, position, eulerAngles);
        }

        private void StartFocusing(Colonist colonist, Vector3 position, Vector3 eulerAngles)
        {
            if (_focusingCoroutine != null)
            {
                StopCoroutine(_focusingCoroutine);
                _focusingCoroutine = null;
            }

            if (Vector3.Distance(transform.position, colonist.Center) > _minDistanceForTeleporation)
            {
                transform.position = position;
                _newPosition = position;
                SetFocusRotation(eulerAngles);
            }
            else
            {
                _focusingCoroutine = StartCoroutine(Focusing(position, eulerAngles, colonist));
            }
        }

        private float GetDistanceAboveTerrain()
        {
            if (Physics.Raycast(new Ray(transform.position + _originPositionCorrection, Vector3.down), out var hit,
                100f, _terrainMask))
            {
                return hit.distance;
            }
            else
            {
                throw new InvalidOperationException("Camera is not above terrain");
            }
        }

        private IEnumerator Focusing(Vector3 position, Vector3 eulerAngles, Colonist colonist)
        {
            _focusing = true;

            transform.DOMove(position, _focusDuration * Time.timeScale);
            
            yield return new WaitForSecondsRealtime(_focusDuration / 2f);
            
            // Start to change fov and rotation in the middle of movement
            SetFocusRotation(eulerAngles);

            yield return new WaitForSecondsRealtime(_focusDuration / 2f);
            
            _newPosition = transform.position;
            
            SetFollow(colonist);

            _focusing = false;
        }

        private void SetFocusRotation(Vector3 eulerAngles)
        {
            _newFieldOfView = _focusFov;
            _newRotation.eulerAngles = eulerAngles;
        }

        private void OnMapInitializationLoad()
        {
            _mapInitialization.Load -= OnMapInitializationLoad;

            _activated = Application.isEditor && _deactivateAtStartup ? false : true;

            StartCoroutine(AllowMouseScrollALittleLater());
        }

        private IEnumerator AllowMouseScrollALittleLater()
        {
            yield return new WaitForSeconds(0.1f);
            _canMouseScroll = true;
        }

        private void ToggleCameraMovement(InputAction.CallbackContext context)
        {
            _activated = !_activated;

            if (_activated)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }
        
        private void OnSelecting(Rect _)
        {
            _isSelectingInput = true;
        }

        private void OnSelectingEnd(Rect _)
        {
            _isSelectingInput = false;
        }

        private void LoadSettings()
        {
            _cameraSensitivity = _gameSettings.CameraSensitivity.Value;
            _screenEdgeMouseScroll = _gameSettings.ScreenEdgeMouseScroll.Value;

            _gameSettings.CameraSensitivity.Subscribe(value => _cameraSensitivity = value);
            _gameSettings.ScreenEdgeMouseScroll.Subscribe(value => _screenEdgeMouseScroll = value);
        }

        private void Deactivate()
        {
            _setFollowAction.started -= TryFollow;
        }

        private void Activate()
        {
            _setFollowAction.started += TryFollow;

            var mousePosition = _mousePositionAction.ReadValue<Vector2>();

            _lastMousePositionX = mousePosition.x;
            _lastMousePositionY = mousePosition.y;
        }

        private void UpdateFollowing()
        {
            var keyboardMoved = _movementAction.ReadValue<Vector2>() != Vector2.zero;

            var position = _mousePositionAction.ReadValue<Vector2>();
            var normalisedPosition = GetNormalisedPosition(position);
            var mouseMoved = Mathf.Abs(normalisedPosition.x) > _positionMoveXThreshold ||
                             Mathf.Abs(normalisedPosition.y) > _positionMoveYThreshold;

            if (keyboardMoved || mouseMoved || _dragAction.IsPressed())
            {
                _following = false;
            }
        }

        private void UpdateFollow()
        {
            transform.position = RaiseAboveTerrain(_followTransform.position + _offset);
            _newPosition = transform.position;
        }

        private void TryFollow(InputAction.CallbackContext context)
        {
            var screenPoint = _mousePositionAction.ReadValue<Vector2>();
            var ray = _camera.ScreenPointToRay(screenPoint);

            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.gameObject.TryGetComponent<Colonist>(out var colonist))
                {
                    SetFollow(colonist);
                }
                else
                {
                    ResetFollow();
                }
            }
        }

        private void SetFollow(Colonist colonist)
        {
            UnsubscribeFromLastUnit();

            _colonist = colonist;
            _followTransform = colonist.transform;
            _offset = _newPosition - _followTransform.position;

            colonist.Die += ResetFollow;

            _following = true;
        }

        private void ResetFollow()
        {
            _followTransform = null;
            _following = false;
        }

        private void UnsubscribeFromLastUnit()
        {
            if (_colonist != null)
            {
                _colonist.Die -= ResetFollow;
            }
        }

        private void CalculateDeltas()
        {
            var mousePosition = _mousePositionAction.ReadValue<Vector2>();

            _deltaMousePositionX = mousePosition.x - _lastMousePositionX;
            _deltaMousePositionY = mousePosition.y - _lastMousePositionY;

            _lastMousePositionX = mousePosition.x;
            _lastMousePositionY = mousePosition.y;
        }

        private void UpdatePosition()
        {
            var movement = _movementAction.ReadValue<Vector2>();
            
            _newPosition += transform.right * (_moveSpeed * Time.unscaledDeltaTime * movement.x);
            _newPosition += transform.forward * (_moveSpeed * Time.unscaledDeltaTime * movement.y);
        }

        private void UpdateRotation()
        {
            if (!_dragAction.IsPressed())
            {
                return;
            }

            _verticalRotation = _newRotation.eulerAngles.x;
            _horizontalRotation = _newRotation.eulerAngles.y;

            _verticalRotation -= _rotateVerticalSpeed * _cameraSensitivity * _deltaMousePositionY;
            _horizontalRotation += _rotateHorizontalSpeed * _cameraSensitivity * _deltaMousePositionX;
            _newRotation.eulerAngles =
                new Vector3(Mathf.Clamp(_verticalRotation, _minVerticalRotation, _maxVerticalRotation),
                    _horizontalRotation, 0.0f);
        }

        private void UpdateZoom()
        {
            var zoomScroll = _zoomScrollAction.ReadValue<Vector2>().y;
            _newFieldOfView = Mathf.Clamp(_newFieldOfView - zoomScroll * _zoomScrollSensitivity,
                _minFov, _maxFov);

            var zoomButton = _zoomButtonAction.ReadValue<float>();
            _newFieldOfView = Mathf.Clamp(_newFieldOfView - zoomButton * _zoomButtonSensitivity,
                _minFov, _maxFov);
        }

        private void UpdatePositionFromMouseThresholdMovement()
        {
            if (!_canMouseScroll || !_screenEdgeMouseScroll || _isSelectingInput)
            {
                return;
            }

            var position = _mousePositionAction.ReadValue<Vector2>();
            var normalisedPosition = GetNormalisedPosition(position);
            var movement = Vector2.zero;

            if (Mathf.Abs(normalisedPosition.x) > _positionMoveXThreshold)
            {
                movement.x = Mathf.Sign(normalisedPosition.x) * _moveSpeed * Time.unscaledDeltaTime;
            }

            if (Mathf.Abs(normalisedPosition.y) > _positionMoveYThreshold)
            {
                movement.y = Mathf.Sign(normalisedPosition.y) * _moveSpeed * Time.unscaledDeltaTime;
            }

            if (movement != Vector2.zero)
            {
                UpdatePositionFromMouseMovement(movement);
            }
        }

        //Result between -1 and 1 is the result between 0 and 1 subtracted with 0.5 and multiplied by 2
        private Vector2 GetNormalisedPosition(Vector2 position)
        {
            var result = new Vector2(((position.x / Screen.width) - 0.5f) * 2f,
                ((position.y / Screen.height) - 0.5f) * 2f);
            return result;
        }

        private void UpdatePositionFromMouseMovement(Vector2 movement)
        {
            _newPosition += transform.right * movement.x;
            _newPosition += transform.up * movement.y + transform.forward * movement.y;
        }

        private void ClampPositionByConstraints()
        {
            var position = _newPosition;
            
            position = RaiseAboveTerrain(position);

            if (position.x < _minimumPositionX)
            {
                position.x = _minimumPositionX;
            }

            if (position.x > _maximumPositionX)
            {
                position.x = _maximumPositionX;
            }

            if (position.z < _minimumPositionZ)
            {
                position.z = _minimumPositionZ;
            }

            if (position.z > _maximumPositionZ)
            {
                position.z = _maximumPositionZ;
            }

            _newPosition = position;
        }

        private Vector3 RaiseAboveTerrain(Vector3 position)
        {
            if (Physics.Raycast(new Ray(position + _originPositionCorrection, Vector3.down), out var hit,
                100f, _terrainMask))
            {
                position.y = hit.point.y + _heightAboveTerrain;
            }

            return position;
        }

        private void SmoothUpdate()
        {
            if (!_focusing)
            {
                transform.position = Vector3.Lerp(transform.position, _newPosition,
                    _positionSmoothing * Time.unscaledDeltaTime) + new Vector3(0, _raiseDistance, 0);
            }
            
            transform.rotation = Quaternion.Lerp(transform.rotation, _newRotation,
                _rotationSmoothing * Time.unscaledDeltaTime);
            
            _camera.fieldOfView =
                Mathf.Lerp(_camera.fieldOfView, _newFieldOfView, _zoomSmoothing * Time.unscaledDeltaTime);
        }
    }
}
