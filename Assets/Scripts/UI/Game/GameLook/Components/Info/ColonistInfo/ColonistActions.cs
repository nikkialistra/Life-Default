using ColonistManagement.Tasking;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace UI.Game.GameLook.Components.Info.ColonistInfo
{
    public class ColonistActions : MonoBehaviour
    {
        [Required]
        [SerializeField] private Sprite _iconTasks;
        [Required]
        [SerializeField] private Sprite _iconOrders;
        
        private VisualElement _currentActionIcon;
        private Label _currentAction;

        private Button _actionType;
        private VisualElement _actionTypeIcon;

        private bool _isOrdering;
        
        private ActionIconsRegistry _actionIconsRegistry;

        [Inject]
        public void Construct(ActionIconsRegistry actionIconsRegistry)
        {
            _actionIconsRegistry = actionIconsRegistry;
        }

        public void Initialize(VisualElement tree)
        {
            _currentActionIcon = tree.Q<VisualElement>("current-action__icon");
            _currentAction = tree.Q<Label>("current-action__text");

            _actionType = tree.Q<Button>("action-type");
            _actionTypeIcon = tree.Q<VisualElement>("action-type__icon");
        }

        public void BindSelf()
        {
            _actionType.clicked += ToggleActionType;
        }

        public void UnbindSelf()
        {
            _actionType.clicked -= ToggleActionType;
        }

        private void ToggleActionType()
        {
            _isOrdering = !_isOrdering;

            if (_isOrdering)
            {
                SwitchToOrdering();
            }
            else
            {
                SwitchToTasking();
            }
        }

        private void SwitchToOrdering()
        {
            _actionTypeIcon.style.backgroundImage = new StyleBackground(_iconOrders);

            _currentActionIcon.style.backgroundImage =
                new StyleBackground(_actionIconsRegistry[ActionType.FollowingOrders]);
            _currentAction.text = ActionType.FollowingOrders.GetString();
        }

        private void SwitchToTasking()
        {
            _actionTypeIcon.style.backgroundImage = new StyleBackground(_iconTasks);
            
            _currentActionIcon.style.backgroundImage =
                new StyleBackground(_actionIconsRegistry[ActionType.Relaxing]);
            _currentAction.text = ActionType.Relaxing.GetString();
        }
    }
}
