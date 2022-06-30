using System.Collections.Generic;
using System.Linq;
using ColonistManagement.Statuses;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.Info.ColonistInfo
{
    public class ColonistStatuses
    {
        private const int MaxStatuses = 6;

        private readonly LinkedList<StatusType> _statusTypes = new();
        private readonly Label[] _statuses = new Label[MaxStatuses];

        public ColonistStatuses(VisualElement tree)
        {
            _statuses[0] = tree.Q<Label>("status-one");
            _statuses[1] = tree.Q<Label>("status-two");
            _statuses[2] = tree.Q<Label>("status-three");
            _statuses[3] = tree.Q<Label>("status-four");
            _statuses[4] = tree.Q<Label>("status-five");
            _statuses[5] = tree.Q<Label>("status-six");
        }

        public void AddStatus(StatusType statusType)
        {
            if (_statusTypes.Count >= MaxStatuses || HasStatus(statusType))
            {
                return;
            }
            
            _statuses[_statusTypes.Count].style.display = DisplayStyle.Flex;
            _statuses[_statusTypes.Count].text = statusType.GetString();

            _statusTypes.AddLast(statusType);
        }

        private bool HasStatus(StatusType statusType)
        {
            return _statusTypes.Any(value => value == statusType);
        }

        public void RemoveStatus(StatusType statusType)
        {
            _statusTypes.Remove(statusType);
            
            UpdateShownStatuses();
        }

        private void UpdateShownStatuses()
        {
            for (int i = 0; i < _statusTypes.Count; i++)
            {
                _statuses[i].text = _statusTypes.ElementAt(i).GetString();
            }

            for (int i = _statusTypes.Count; i < MaxStatuses; i++)
            {
                _statuses[i].style.display = DisplayStyle.None;
            }
        }
    }
}
