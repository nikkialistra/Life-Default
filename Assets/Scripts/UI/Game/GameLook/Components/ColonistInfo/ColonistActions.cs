using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.ColonistInfo
{
    public class ColonistActions : MonoBehaviour
    {
        [SerializeField] private Sprite _iconTasks;
        [SerializeField] private Sprite _iconOrders;
        
        private VisualElement _currentActionIcon;
        private Label _currentAction;

        private Button _actionType;
        private VisualElement _actionTypeIcon;

        private bool _isOrdering;

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

            _actionTypeIcon.style.backgroundImage =
                _isOrdering ? new StyleBackground(_iconOrders) : new StyleBackground(_iconTasks);
        }
    }
}
