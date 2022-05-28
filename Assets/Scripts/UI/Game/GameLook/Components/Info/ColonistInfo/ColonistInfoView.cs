using ColonistManagement.Statuses;
using Colonists;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.Info.ColonistInfo
{
    [RequireComponent(typeof(InfoPanelView))]
    [RequireComponent(typeof(ColonistDetailTabs))]
    [RequireComponent(typeof(ColonistIndicators))]
    [RequireComponent(typeof(ColonistHeader))]
    [RequireComponent(typeof(ColonistActivityPanel))]
    [RequireComponent(typeof(CommandsView))]
    public class ColonistInfoView : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset _asset;

        private bool _shown;

        private InfoPanelView _parent;
        private TemplateContainer _tree;

        private ColonistDetailTabs _colonistDetailTabs;
        private ColonistHeader _colonistHeader;
        private ColonistIndicators _colonistIndicators;
        private ColonistActivityPanel _colonistActivityPanel;
        private ColonistStatuses _colonistStatuses;

        private VisualElement _picture;

        private VisualElement _commands;

        private CommandsView _commandsView;

        private void Awake()
        {
            _parent = GetComponent<InfoPanelView>();

            _colonistDetailTabs = GetComponent<ColonistDetailTabs>();
            _colonistIndicators = GetComponent<ColonistIndicators>();
            _colonistHeader = GetComponent<ColonistHeader>();
            _colonistActivityPanel = GetComponent<ColonistActivityPanel>();
            _commandsView = GetComponent<CommandsView>();

            _tree = _asset.CloneTree();
            _tree.pickingMode = PickingMode.Ignore;
            
            TabContent = _tree.Q<VisualElement>("tab-content");

            BindElements();
        }

        public Colonist Colonist { get; private set; }
        public VisualElement TabContent { get; private set; }

        private void OnEnable()
        {
            _colonistDetailTabs.BindSelf();
            _colonistActivityPanel.BindSelf();
        }

        private void OnDisable()
        {
            HideSelf();
            
            _colonistDetailTabs.UnbindSelf();
            _colonistActivityPanel.UnbindSelf();
        }

        private void BindElements()
        {
            _colonistDetailTabs.Initialize(_tree);
            _colonistHeader.Initialize(_tree);
            _colonistIndicators.Initialize(_tree);
            _colonistActivityPanel.Initialize(_tree);
            _colonistStatuses = new ColonistStatuses(_tree);

            _picture = _tree.Q<VisualElement>("picture");

            _commands = _tree.Q<VisualElement>("commands");
        }

        private void OnDestroy()
        {
            UnsubscribeFromColonist();
        }

        public void ShowSelf()
        {
            if (_shown)
            {
                return;
            }

            _parent.InfoPanel.Add(_tree);
            _colonistHeader.BindPanelActions();
            _commandsView.BindSelf(_commands);
            _shown = true;
        }

        public void HideSelf()
        {
            if (!_shown)
            {
                return;
            }
            
            UnsubscribeFromColonist();

            _parent.InfoPanel.Remove(_tree);
            _colonistHeader.UnbindPanelActions();
            _commandsView.UnbindSelf();
            _shown = false;
        }

        public void FillIn(Colonist colonist)
        {
            FillInPreview(colonist);
            FillInProperties(colonist);

            _colonistDetailTabs.FillIn(colonist);
            _colonistActivityPanel.FillIn(colonist);
        }

        private void HidePanel()
        {
            _parent.UnsetColonist(Colonist);
        }

        private void FillInPreview(Colonist colonist)
        {
            _colonistHeader.FillInName(colonist.Name);
        }

        private void FillInProperties(Colonist colonist)
        {
            UnsubscribeFromColonist();
            Colonist = colonist;
            _colonistHeader.FillInColonist(Colonist);
            SubscribeToColonist();

            UpdateVitality();
            UpdateIndicators();
        }

        private void UnsubscribeFromColonist()
        {
            if (Colonist != null)
            {
                Colonist.VitalityChange -= UpdateVitality;
                Colonist.Dying -= HidePanel;
            }
        }

        private void SubscribeToColonist()
        {
            Colonist.VitalityChange += UpdateVitality;
            Colonist.Dying += HidePanel;
        }

        private void UpdateVitality()
        {
            _colonistIndicators.UpdateVitality(Colonist.Unit.UnitVitality);
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
