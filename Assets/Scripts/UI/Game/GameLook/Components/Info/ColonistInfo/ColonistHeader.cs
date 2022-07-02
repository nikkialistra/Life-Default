using System.Collections;
using Colonists.Services.Selecting;
using General;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Zenject;

namespace UI.Game.GameLook.Components.Info.ColonistInfo
{
    public class ColonistHeader : MonoBehaviour
    {
        private const string InputMap = "Input";
        private const string ManagementMap = "Management";

        private TextField _name;
        private Button _rename;
        private Button _focus;
        private Button _next;

        private Colonists.Colonist _colonist;

        private bool _inputFinished;

        private ColonistChoosing _colonistChoosing;
        private CameraMovement _cameraMovement;

        private PlayerInput _playerInput;

        private InputAction _enterAction;
        private InputAction _leftClickAction;

        [Inject]
        public void Construct(ColonistChoosing colonistChoosing, CameraMovement cameraMovement, PlayerInput playerInput)
        {
            _colonistChoosing = colonistChoosing;
            _cameraMovement = cameraMovement;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _enterAction = _playerInput.actions.FindAction("Enter");
            _leftClickAction = _playerInput.actions.FindAction("LeftClick");
        }

        public void Initialize(VisualElement tree)
        {
            _name = tree.Q<TextField>("name-field");
            _rename = tree.Q<Button>("rename");
            _focus = tree.Q<Button>("focus");
            _next = tree.Q<Button>("next");
        }

        public void FillInName(string name)
        {
            _name.value = name;
        }

        public void FillInColonist(Colonists.Colonist colonist)
        {
            _colonist = colonist;
        }

        public void BindPanelActions()
        {
            _name.RegisterCallback<MouseDownEvent>(OnNameMouseDown);
            _enterAction.started += FinishInput;
            _leftClickAction.started += OnLeftClick;

            _rename.clicked += OnRename;
            _focus.clicked += OnFocus;
            _next.clicked += OnNext;
        }

        public void UnbindPanelActions()
        {
            _name.UnregisterCallback<MouseDownEvent>(OnNameMouseDown);
            _enterAction.started -= FinishInput;
            _leftClickAction.started -= OnLeftClick;

            _rename.clicked -= OnRename;
            _focus.clicked -= OnFocus;
            _next.clicked -= OnNext;
        }

        private void OnNameMouseDown(MouseDownEvent evt)
        {
            _inputFinished = false;
        }

        private void OnLeftClick(InputAction.CallbackContext context)
        {
            _inputFinished = true;

            // cancel finish only if a user doesn't click on the name field, its event can be checked a frame later
            StartCoroutine(CTryFinishInput());
        }

        private IEnumerator CTryFinishInput()
        {
            yield return new WaitForSeconds(0.1f);

            if (_inputFinished)
                FinishInput();
        }

        private void FinishInput(InputAction.CallbackContext context)
        {
            FinishInput();
        }

        private void OnRename()
        {
            if (_playerInput.currentActionMap.name == ManagementMap)
                StartInput();
            else
                FinishInput();
        }

        private void OnFocus()
        {
            _cameraMovement.FocusOn(_colonist);
        }

        private void OnNext()
        {
            _colonistChoosing.NextColonistTo(_colonist);
        }

        private void StartInput()
        {
            _name.focusable = true;
            _name.Focus();
            _name.focusable = false;

            _playerInput.SwitchCurrentActionMap(InputMap);
            _cameraMovement.DeactivateMovement();
        }

        private void FinishInput()
        {
            _colonist.Name = _name.value;

            _playerInput.SwitchCurrentActionMap(ManagementMap);
            _cameraMovement.ActivateMovement();
        }
    }
}
