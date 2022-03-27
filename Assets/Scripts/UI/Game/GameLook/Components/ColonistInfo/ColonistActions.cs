using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.ColonistInfo
{
    public class ColonistActions
    {
        private VisualElement _currentActionIcon;
        private Label _currentAction;

        private VisualElement _actionTypeIcon;
        
        public ColonistActions(VisualElement tree)
        {
            _currentActionIcon = tree.Q<VisualElement>("current-action__icon");
            _currentAction = tree.Q<Label>("current-action__text");

            _actionTypeIcon = tree.Q<VisualElement>("action-type__icon");
        }
    }
}
