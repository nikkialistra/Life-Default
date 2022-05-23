using ColonistManagement.Statuses;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.Info.ColonistInfo
{
    [RequireComponent(typeof(InfoPanelView))]
    [RequireComponent(typeof(ColonistIndicators))]
    [RequireComponent(typeof(ColonistHeader))]
    [RequireComponent(typeof(ColonistActions))]
    [RequireComponent(typeof(CommandsView))]
    public class ColonistInfoView : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset _asset;

        private bool _shown;

        private Colonists.Colonist _colonist;
        
        private InfoPanelView _parent;
        private TemplateContainer _tree;

        private ColonistDetails _colonistDetails;
        private ColonistHeader _colonistHeader;
        private ColonistIndicators _colonistIndicators;
        private ColonistActions _colonistActions;
        private ColonistStatuses _colonistStatuses;

        private VisualElement _picture;

        private VisualElement _commands;

        private CommandsView _commandsView;

        private void Awake()
        {
            _parent = GetComponent<InfoPanelView>();

            _colonistIndicators = GetComponent<ColonistIndicators>();
            _colonistHeader = GetComponent<ColonistHeader>();
            _colonistActions = GetComponent<ColonistActions>();
            _commandsView = GetComponent<CommandsView>();

            _tree = _asset.CloneTree();
            _tree.pickingMode = PickingMode.Ignore;

            BindElements();
        }

        private void OnEnable()
        {
            _colonistDetails.BindSelf();
            _colonistActions.BindSelf();
        }

        private void OnDisable()
        {
            HideSelf();
            
            _colonistDetails.UnbindSelf();
            _colonistActions.UnbindSelf();
        }

        private void BindElements()
        {
            _colonistDetails = new ColonistDetails(_tree);
            _colonistHeader.Initialize(_tree);
            _colonistIndicators.Initialize(_tree);
            _colonistActions.Initialize(_tree);
            _colonistStatuses = new ColonistStatuses(_tree);

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
            
            UnsubscribeFromUnit();

            _parent.InfoPanel.Remove(_tree);
            _colonistHeader.UnbindPanelActions();
            _commandsView.UnbindSelf();
            _shown = false;
        }

        public void FillIn(Colonists.Colonist colonist)
        {
            FillInPreview(colonist);
            FillInProperties(colonist);
        }

        private void HidePanel()
        {
            _parent.UnsetColonist(_colonist);
        }

        private void FillInPreview(Colonists.Colonist colonist)
        {
            _colonistHeader.FillInName(colonist.Name);
        }

        private void FillInProperties(Colonists.Colonist colonist)
        {
            UnsubscribeFromUnit();
            _colonist = colonist;
            _colonistHeader.FillInColonist(_colonist);
            SubscribeToUnit();

            UpdateVitality();
            UpdateIndicators();
        }

        private void UnsubscribeFromUnit()
        {
            if (_colonist != null)
            {
                _colonist.VitalityChange -= UpdateVitality;
                _colonist.Dying -= HidePanel;
            }
        }

        private void SubscribeToUnit()
        {
            _colonist.VitalityChange += UpdateVitality;
            _colonist.Dying += HidePanel;
        }

        private void UpdateVitality()
        {
            _colonistIndicators.UpdateVitality(_colonist.Unit.UnitVitality);
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
