using System;
using Colonists.Activities;
using UnityEngine;

namespace Colonists
{
    [RequireComponent(typeof(ColonistSkills))]
    public class ColonistActivities : MonoBehaviour
    {
        private ActivityType _currentActivity;
        
        private ColonistSkills _colonistSkills;

        public event Action<ActivityType> ActivityChange;

        private void Awake()
        {
            _colonistSkills = GetComponent<ColonistSkills>();
        }

        public void SwitchTo(ActivityType activityType)
        {
            _currentActivity = activityType;
            
            ActivityChange?.Invoke(_currentActivity);
        }

        public void Advance(ActivityType activityType, float duration)
        {
            if (_currentActivity != activityType)
            {
                throw new InvalidOperationException(
                    $"Colonist advanced in {activityType} while current activity was {_currentActivity}");
            }

            _colonistSkills.Advance(activityType, duration);
        }
    }
}
