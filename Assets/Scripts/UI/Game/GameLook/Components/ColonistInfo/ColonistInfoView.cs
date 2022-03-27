using Colonists.Colonist;
using Colonists.Services.Selecting;
using Game;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace UI.Game.GameLook.Components.ColonistInfo
{
    [RequireComponent(typeof(InfoPanelView))]
    [RequireComponent(typeof(ColonistIndicators))]
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
        
        private Label _name;

        private Button _next;
        private Button _focus;

        private VisualElement _picture;

        private VisualElement _commands;

        private CommandsView _commandsView;

        private ColonistChoosing _colonistChoosing;
        private CameraMovement _cameraMovement;

        [Inject]
        public void Construct(ColonistChoosing colonistChoosing, CameraMovement cameraMovement)
        {
            _colonistChoosing = colonistChoosing;
            _cameraMovement = cameraMovement;
        }

        private void Awake()
        {
            _parent = GetComponent<InfoPanelView>();

            _colonistIndicators = GetComponent<ColonistIndicators>();
            _commandsView = GetComponent<CommandsView>();

            _tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();
            _tree.pickingMode = PickingMode.Ignore;

            BindElements();
        }

        private void OnEnable()
        {
            _colonistDetails.OnEnable();
        }

        private void OnDisable()
        {
            _colonistDetails.OnDisable();
        }

        private void BindElements()
        {
            _colonistDetails = new ColonistDetails(_tree);
            _colonistIndicators.Initialize(_tree);
            _colonistActions = new ColonistActions(_tree);
            _colonistStatuses = new ColonistStatuses(_tree);
            
            _name = _tree.Q<Label>("name");

            _next = _tree.Q<Button>("next");
            _focus = _tree.Q<Button>("focus");
            
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
            _shown = false;
        }

        public void FillIn(ColonistFacade colonist)
        {
            FillInPreview(colonist);
            FillInProperties(colonist);
        }

        private void OnNext()
        {
           _colonistChoosing.NextColonistTo(_colonist);
        }
        
        private void BindPanelActions()
        {
            _next.clicked += OnNext;
            _focus.clicked += OnFocus;
        }

        private void UnbindPanelActions()
        {
            _next.clicked -= OnNext;
            _focus.clicked -= OnFocus;
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
            _name.text = colonist.Name;
        }

        private void FillInProperties(ColonistFacade colonist)
        {
            UnsubscribeFromUnit();
            _colonist = colonist;
            SubscribeToUnit();

            UpdateHealth();
            UpdateIndicators();
        }

        private void UnsubscribeFromUnit()
        {
            if (_colonist != null)
            {
                _colonist.HealthChange -= UpdateHealth;
                _colonist.Die -= HidePanel;
            }
        }

        private void SubscribeToUnit()
        {
            _colonist.HealthChange += UpdateHealth;
            _colonist.Die += HidePanel;
        }

        private void UpdateHealth()
        {
            UpdateVitality();
            UpdateBlood();
        }

        private void UpdateIndicators()
        {
            UpdateSatiety();
            UpdateSleep();
            UpdateHappiness();
            UpdateConsciousness();
            UpdateEntertainment();
        }

        private void UpdateVitality()
        {
            _colonistIndicators.UpdateVitality(_colonist.Health);
        }

        private void UpdateBlood()
        {
            _colonistIndicators.UpdateBlood(_colonist.Health);
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
    }
}
