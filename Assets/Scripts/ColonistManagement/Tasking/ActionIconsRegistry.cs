using System;
using Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ColonistManagement.Tasking
{
    public class ActionIconsRegistry : MonoBehaviour
    {
        public Sprite this[ActionType actionType] => _taskIcons[actionType];

        [ValidateInput(nameof(EveryActionHaveIcon), "Not every task has icon")]
        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
        [SerializeField] private TaskIconsDictionary _taskIcons;

        private bool EveryActionHaveIcon(TaskIconsDictionary taskIcons, ref string errorMessage)
        {
            foreach (var actionType in (ActionType[])Enum.GetValues(typeof(ActionType)))
                if (!taskIcons.ContainsKey(actionType))
                {
                    errorMessage = $"{actionType} don't have icon";
                    return false;
                }

            return true;
        }

        [Serializable] public class TaskIconsDictionary : SerializableDictionary<ActionType, Sprite> { }
    }
}
