using ColonistManagement.Statuses;
using Colonists.Colonist;
using Colonists.Services.Selecting;
using Game;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Zenject;

namespace UI.Game.GameLook.Components.ColonistInfo
{
    [RequireComponent(typeof(InfoPanelView))]
    [RequireComponent(typeof(ColonistIndicators))]
    [RequireComponent(typeof(ColonistActions))]
    [RequireComponent(typeof(CommandsView))]
    public class ColonistInfoView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/ColonistInfo";

        private bool _shown;
        
        private ColonistFacade _colonist;
        
        private InfoPanelView _parent;
        private TemplateContainer _tree;

        private ColonistDetails _colonistDetails;
        private ColonistIndicators _colonistIndicators;
        private ColonistActions _colonistActions;
        private ColonistStatuses _colonistStatuses;
        
        private TextField _name;

        private Button _rename;
        private Button _focus;
        private Button _next;

        private VisualElement _picture;

        private VisualElement _commands;

        private CommandsView _commandsView;

        private ColonistChoosing _colonistChoosing;
        private CameraMovement _cameraMovement;
        
        private PlayerInput _playerInput;
        
        private InputAction _enterAction;

        [Inject]
        public void Construct(ColonistChoosing colonistChoosing, CameraMovement cameraMovement, PlayerInput playerInput)
        {
            _colonistChoosing = colonistChoosing;
            _cameraMovement = cameraMovement;
            _playerInput = playerInput;
        }

        private void Awake()
        {
            _parent = GetComponent<InfoPanelView>();

            _colonistIndicators = GetComponent<ColonistIndicators>();
            _colonistActions = GetComponent<ColonistActions>();
            _commandsView = GetComponent<CommandsView>();

            _tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();
            _tree.pickingMode = PickingMode.Ignore;

            BindElements();

            _enterAction = _playerInput.actions.FindAction("Enter");
        }

        private void OnEnable()
        {
            _colonistDetails.BindSelf();
            _colonistActions.BindSelf();

            _enterAction.started += FinishInput;
        }

        private void OnDisable()
        {
            _colonistDetails.UnbindSelf();
            _colonistActions.UnbindSelf();

            _enterAction.started -= FinishInput;
        }

        private void BindElements()
        {
            _colonistDetails = new ColonistDetails(_tree);
            _colonistIndicators.Initialize(_tree);
            _colonistActions.Initialize(_tree);
            _colonistStatuses = new ColonistStatuses(_tree);

            _name = _tree.Q<TextField>("name-field");

            _rename = _tree.Q<Button>("rename");
            _focus = _tree.Q<Button>("focus");
            _next = _tree.Q<Button>("next");
            
            _picture = _tree.Q<VisualElement>("picture");

            _commands = _tree.Q<VisualElement>("commands");
        }

        private void OnDestroy()
        {
            UnsubscribeFromUnit();
        }

        public void ShowSelf()
        {
            if (_shown)
            {
                return;
            }

            _parent.InfoPanel.Add(_tree);
            _commandsView.BindSelf(_commands);
            _shown = true;

            BindPanelActions();
        }

        public void HideSelf()
        {
            if (!_shown)
            {
                return;
            }
            
            UnsubscribeFromUnit();
            UnbindPanelActions();

            _parent.InfoPanel.Remove(_tree);
            _commandsView.UnbindSelf();
            _shown = false;
        }

        public void FillIn(ColonistFacade colonist)
        {
            FillInPreview(colonist);
            FillInProperties(colonist);
        }

        private void FinishInput(InputAction.CallbackContext context)
        {
            FinishInput();
        }

        private void OnNext()
        {
           _colonistChoosing.NextColonistTo(_colonist);
        }
        
        private void BindPanelActions()
        {
            _rename.clicked += OnRename;
            _focus.clicked += OnFocus;
            _next.clicked += OnNext;
        }

        private void UnbindPanelActions()
        {
            _rename.clicked -= OnRename;
            _focus.clicked -= OnFocus;
            _next.clicked -= OnNext;
        }

        private void OnRename()
        {
            if (_playerInput.inputIsActive)
            {
                StartInput();
            }
            else
            {
                FinishInput();
            }
        }

        private void StartInput()
        {
            _name.focusable = true;
            _name.Focus();
            _name.focusable = false;

            _playerInput.DeactivateInput();
        }

        private void FinishInput()
        {
            _playerInput.ActivateInput();
        }

        private void OnFocus()
        {
            _cameraMovement.FocusOn(_colonist);
        }

        private void HidePanel()
        {
            _parent.HideSelf();
        }

        private void FillInPreview(ColonistFacade colonist)
        {
            _name.value = colonist.Name;
        }

        private void FillInProperties(ColonistFacade colonist)
        {
            UnsubscribeFromUnit();
            _colonist = colonist;
            SubscribeToUnit();

            UpdateVitality();
            UpdateIndicators();
        }

        private void UnsubscribeFromUnit()
        {
            if (_colonist != null)
            {
                _colonist.HealthChange -= UpdateVitality;
                _colonist.Die -= HidePanel;
            }
        }

        private void SubscribeToUnit()
        {
            _colonist.HealthChange += UpdateVitality;
            _colonist.Die += HidePanel;
        }

        private void UpdateVitality()
        {
            _colonistIndicators.UpdateHealth(_colonist.Vitality);
            _colonistIndicators.UpdateRecoverySpeed(_colonist.Vitality);
        }

        private void UpdateIndicators()
        {
            UpdateSatiety();
            UpdateSleep();
            UpdateHappiness();
            UpdateConsciousness();
            UpdateEntertainment();
        }

        private void UpdateSatiety()
        {
            _colonistIndicators.UpdateSatiety(100);
        }

        private void UpdateSleep()
        {
            _colonistIndicators.UpdateSleep(100);
        }

        private void UpdateHappiness()
        {
            _colonistIndicators.UpdateHappiness(100);
        }

        private void UpdateConsciousness()
        {
            _colonistIndicators.UpdateConsciousness(100);
        }

        private void UpdateEntertainment()
        {
            _colonistIndicators.UpdateEntertainment(100);
        }

        [Button(ButtonSizes.Medium)]
        private void AddStatus(StatusType statusType)
        {
            _colonistStatuses.AddStatus(statusType);
        }

        [Button(ButtonSizes.Medium)]
        private void RemoveStatus(StatusType statusType)
        {
            _colonistStatuses.RemoveStatus(statusType);
        }
    }
}
