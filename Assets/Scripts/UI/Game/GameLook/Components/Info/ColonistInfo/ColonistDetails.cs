using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.Info.ColonistInfo
{
    public class ColonistDetails
    {
        private readonly Toggle _equipment;
        private readonly Toggle _stats;
        private readonly Toggle _skills;
        private readonly Toggle _info;

        public ColonistDetails(VisualElement tree)
        {
            _equipment = tree.Q<Toggle>("equipment");
            _stats = tree.Q<Toggle>("stats");
            _skills = tree.Q<Toggle>("skills");
            _info = tree.Q<Toggle>("info");
        }

        public void BindSelf()
        {
            _equipment.RegisterValueChangedCallback(OnEquipmentToggle);
            _stats.RegisterValueChangedCallback(OnStatsToggle);
            _skills.RegisterValueChangedCallback(OnSkillsToggle);
            _info.RegisterValueChangedCallback(OnInfoToggle);
        }

        public void UnbindSelf()
        {
            _equipment.UnregisterValueChangedCallback(OnEquipmentToggle);
            _stats.UnregisterValueChangedCallback(OnStatsToggle);
            _skills.UnregisterValueChangedCallback(OnSkillsToggle);
            _info.UnregisterValueChangedCallback(OnInfoToggle);
        }

        private void OnEquipmentToggle(ChangeEvent<bool> changeEvent)
        {
            UncheckToggles();
            _equipment.SetValueWithoutNotify(changeEvent.newValue);
        }

        private void OnStatsToggle(ChangeEvent<bool> changeEvent)
        {
            UncheckToggles();
            _stats.SetValueWithoutNotify(changeEvent.newValue);
        }

        private void OnSkillsToggle(ChangeEvent<bool> changeEvent)
        {
            UncheckToggles();
            _skills.SetValueWithoutNotify(changeEvent.newValue);
        }

        private void OnInfoToggle(ChangeEvent<bool> changeEvent)
        {
            UncheckToggles();
            _info.SetValueWithoutNotify(changeEvent.newValue);
        }

        private void UncheckToggles()
        {
            _equipment.SetValueWithoutNotify(false);
            _stats.SetValueWithoutNotify(false);
            _skills.SetValueWithoutNotify(false);
            _info.SetValueWithoutNotify(false);
        }
    }
}
