using System;
using Colonists.Colonist;
using Colonists.Services.Selecting;
using Game;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace UI.Game.GameLook.Components.ColonistInfo
{
    [RequireComponent(typeof(InfoPanelView))]
    [RequireComponent(typeof(CommandsView))]
    public class ColonistInfoView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/ColonistInfo";

        [SerializeField] private Sprite _singleArrowUp;
        [SerializeField] private Sprite _singleArrowDown;
        [SerializeField] private Sprite _doubleArrowUp;
        [SerializeField] private Sprite _doubleArrowDown;
        [SerializeField] private Sprite _tripleArrowUp;
        [SerializeField] private Sprite _tripleArrowDown;

        private bool _shown;
        
        private ColonistFacade _colonist;
        
        private InfoPanelView _parent;
        private TemplateContainer _tree;

        private ColonistDetails _colonistDetails;
        
        private Label _name;

        private Button _next;
        private Button _focus;

        private VisualElement _picture;

        private ProgressBar _vitalityProgress;
        private Label _vitalityValue;
        private VisualElement _vitalityArrow;

        private ProgressBar _bloodProgress;
        private Label _bloodValue;
        private VisualElement _bloodArrow;

        private ProgressBar _satietyProgress;
        private Label _satietyValue;
        private VisualElement _satietyArrow;

        private ProgressBar _consciousnessProgress;
        private Label _consciousnessValue;
        private VisualElement _consciousnessArrow;

        private ProgressBar _sleepProgress;
        private Label _sleepValue;
        private VisualElement _sleepArrow;

        private ProgressBar _happinessProgress;
        private Label _happinessValue;
        private VisualElement _happinessArrow;

        private ProgressBar _entertainmentProgress;
        private Label _entertainmentValue;
        private VisualElement _entertainmentArrow;

        private VisualElement _currentActionIcon;
        private Label _currentAction;

        private VisualElement _actionTypeIcon;

        private readonly Label[] _statuses = new Label[6];

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

            _commandsView = GetComponent<CommandsView>();

            _tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();
            _tree.pickingMode = PickingMode.Ignore;

            _colonistDetails = new ColonistDetails(_tree);
            
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
            BindHeader();
            BindBaseInfo();
            BindIndicators();
            BindActions();
            BindStatuses();
            BindCommands();
        }

        private void BindHeader()
        {
            _name = _tree.Q<Label>("name");

            _next = _tree.Q<Button>("next");
            _focus = _tree.Q<Button>("focus");
        }

        private void BindBaseInfo()
        {
            _picture = _tree.Q<VisualElement>("picture");

            _vitalityProgress = _tree.Q<ProgressBar>("vitality-progress");
            _vitalityValue = _tree.Q<Label>("vitality-value");
            _vitalityArrow = _tree.Q<VisualElement>("vitality-arrow");

            _bloodProgress = _tree.Q<ProgressBar>("blood-progress");
            _bloodValue = _tree.Q<Label>("blood-value");
            _bloodArrow = _tree.Q<VisualElement>("blood-arrow");
        }

        private void BindIndicators()
        {
            _satietyProgress = _tree.Q<ProgressBar>("satiety-progress");
            _satietyValue = _tree.Q<Label>("satiety-value");
            _satietyArrow = _tree.Q<VisualElement>("satiety-arrow");

            _sleepProgress = _tree.Q<ProgressBar>("sleep-progress");
            _sleepValue = _tree.Q<Label>("sleep-value");
            _sleepArrow = _tree.Q<VisualElement>("sleep-arrow");

            _happinessProgress = _tree.Q<ProgressBar>("happiness-progress");
            _happinessValue = _tree.Q<Label>("happiness-value");
            _happinessArrow = _tree.Q<VisualElement>("happiness-arrow");

            _consciousnessProgress = _tree.Q<ProgressBar>("consciousness-progress");
            _consciousnessValue = _tree.Q<Label>("consciousness-value");
            _consciousnessArrow = _tree.Q<VisualElement>("consciousness-arrow");

            _entertainmentProgress = _tree.Q<ProgressBar>("entertainment-progress");
            _entertainmentValue = _tree.Q<Label>("entertainment-value");
            _entertainmentArrow = _tree.Q<VisualElement>("entertainment-arrow");
        }

        private void BindActions()
        {
            _currentActionIcon = _tree.Q<VisualElement>("current-action__icon");
            _currentAction = _tree.Q<Label>("current-action__text");

            _actionTypeIcon = _tree.Q<VisualElement>("action-type__icon");
        }

        private void BindStatuses()
        {
            _statuses[0] = _tree.Q<Label>("status-one");
            _statuses[1] = _tree.Q<Label>("status-two");
            _statuses[2] = _tree.Q<Label>("status-three");
            _statuses[3] = _tree.Q<Label>("status-four");
            _statuses[4] = _tree.Q<Label>("status-five");
            _statuses[5] = _tree.Q<Label>("status-six");
        }

        private void BindCommands()
        {
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
            _vitalityProgress.value = _colonist.Health.Vitality;
            _vitalityValue.text = $"{_colonist.Health.VitalityPercent}%";
        }

        private void UpdateBlood()
        {
            _bloodProgress.value = _colonist.Health.Blood;
            _bloodValue.text = $"{_colonist.Health.BloodPercent}%";
        }

        private void UpdateSatiety()
        {
            _satietyProgress.value = _colonist.Indicators.Satiety;
            _satietyValue.text = $"{_colonist.Indicators.Satiety}%";
        }

        private void UpdateSleep()
        {
            _sleepProgress.value = _colonist.Indicators.Sleep;
            _sleepValue.text = $"{_colonist.Indicators.Sleep}%";
        }

        private void UpdateHappiness()
        {
            _happinessProgress.value = _colonist.Indicators.Happiness;
            _happinessValue.text = $"{_colonist.Indicators.Happiness}%";
        }

        private void UpdateConsciousness()
        {
            _consciousnessProgress.value = _colonist.Indicators.Consciousness;
            _consciousnessValue.text = $"{_colonist.Indicators.Consciousness}%";
        }

        private void UpdateEntertainment()
        {
            _entertainmentProgress.value = _colonist.Indicators.Entertainment;
            _entertainmentValue.text = $"{_colonist.Indicators.Entertainment}%";
        }
    }
}
