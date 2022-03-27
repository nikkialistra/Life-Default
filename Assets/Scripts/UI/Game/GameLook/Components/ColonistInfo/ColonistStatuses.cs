using ColonistManagement.Statuses;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.ColonistInfo
{
    public class ColonistStatuses
    {
        private const int MaxStatuses = 6;
        
        private readonly StatusType[] _statusTypes = new StatusType[MaxStatuses];
        private readonly Label[] _statuses = new Label[MaxStatuses];

        private int _freeIndex;
        
        public ColonistStatuses(VisualElement tree)
        {
            for (var i = 0; i < _statusTypes.Length; i++)
            {
                _statusTypes[i] = StatusType.None;
            }
            
            _statuses[0] = tree.Q<Label>("status-one");
            _statuses[1] = tree.Q<Label>("status-two");
            _statuses[2] = tree.Q<Label>("status-three");
            _statuses[3] = tree.Q<Label>("status-four");
            _statuses[4] = tree.Q<Label>("status-five");
            _statuses[5] = tree.Q<Label>("status-six");
        }

        public void AddStatus(StatusType statusType)
        {
            if (_freeIndex >= _statuses.Length - 1 || HasStatus(statusType))
            {
                return;
            }

            _statusTypes[_freeIndex] = statusType;
            _statuses[_freeIndex].style.display = DisplayStyle.Flex;
            _statuses[_freeIndex].text = statusType.GetString();

            _freeIndex++;
        }

        private bool HasStatus(StatusType statusType)
        {
            for (var i = 0; i < _freeIndex; i++)
            {
                if (_statusTypes[i] == statusType)
                {
                    return true;
                }
            }

            return false;
        }

        public void RemoveStatus(StatusType statusType)
        {
            for (var i = 0; i < _freeIndex; i++)
            {
                if (_statusTypes[i] == statusType)
                {
                    _statusTypes[i] = StatusType.None;
                }
            }

            UpdateShownStatuses();
        }

        private void UpdateShownStatuses()
        {
            ShiftWhenNoneStatuses();
            UpdateStatusTexts();
        }

        private void ShiftWhenNoneStatuses()
        {
            for (var i = 0; i < MaxStatuses; i++)
            {
                if (TryFinishShifting(i))
                {
                    break;
                }
            }
        }

        private bool TryFinishShifting(int i)
        {
            if (_statusTypes[i] == StatusType.None)
            {
                if (_statusTypes[i + 1] != StatusType.None)
                {
                    Shift(i);
                }
                else
                {
                    _freeIndex = i;
                    return true;
                }
            }

            return false;
        }

        private void Shift(int i)
        {
            _statusTypes[i] = _statusTypes[i + 1];
            _statusTypes[i + 1] = StatusType.None;
        }

        private void UpdateStatusTexts()
        {
            for (var i = 0; i < _freeIndex; i++)
            {
                _statuses[i].text = _statusTypes[i].GetString();
            }

            for (var i = _freeIndex; i < MaxStatuses; i++)
            {
                _statuses[_freeIndex].style.display = DisplayStyle.None;
            }
        }
    }
}
