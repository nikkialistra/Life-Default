using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.ColonistInfo
{
    public class ColonistStatuses
    {
        private readonly Label[] _statuses = new Label[6];
        
        public ColonistStatuses(VisualElement tree)
        {
            _statuses[0] = tree.Q<Label>("status-one");
            _statuses[1] = tree.Q<Label>("status-two");
            _statuses[2] = tree.Q<Label>("status-three");
            _statuses[3] = tree.Q<Label>("status-four");
            _statuses[4] = tree.Q<Label>("status-five");
            _statuses[5] = tree.Q<Label>("status-six");
        }
    }
}
