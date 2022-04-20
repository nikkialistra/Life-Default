using System;
using Units;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.ColonistInfo
{
    public class ColonistIndicators : MonoBehaviour
    {
        [SerializeField] private Sprite _singleArrowUp;
        [SerializeField] private Sprite _singleArrowDown;
        [SerializeField] private Sprite _doubleArrowUp;
        [SerializeField] private Sprite _doubleArrowDown;
        [SerializeField] private Sprite _tripleArrowUp;
        [SerializeField] private Sprite _tripleArrowDown;
        
        private ProgressBar _healthProgress;
        private Label _healthValue;
        private VisualElement _healthArrow;

        private ProgressBar _recoverySpeedProgress;
        private Label _recoverySpeedValue;
        private VisualElement _recoverySpeedArrow;

        private ProgressBar _satietyProgress;
        private Label _satietyValue;
        private VisualElement _satietyArrow;

        private ProgressBar _consciousnessProgress;
        private Label _consciousnessValue;
        private VisualElement _consciousnessArrow;

        private ProgressBar _sleepProgress;
        private Label _sleepValue;
        private VisualElement _sleepArrow;

        private ProgressBar _happinessProgress;
        private Label _happinessValue;
        private VisualElement _happinessArrow;

        private ProgressBar _entertainmentProgress;
        private Label _entertainmentValue;
        private VisualElement _entertainmentArrow;
        
        public void Initialize(VisualElement tree)
        { 
            _healthProgress = tree.Q<ProgressBar>("health-progress");
            _healthValue = tree.Q<Label>("health-value");
            _healthArrow = tree.Q<VisualElement>("health-arrow");

            _recoverySpeedProgress = tree.Q<ProgressBar>("recovery-speed-progress");
            _recoverySpeedValue = tree.Q<Label>("recovery-speed-value");
            _recoverySpeedArrow = tree.Q<VisualElement>("recovery-speed-arrow");
            
            _satietyProgress = tree.Q<ProgressBar>("satiety-progress");
            _satietyValue = tree.Q<Label>("satiety-value");
            _satietyArrow = tree.Q<VisualElement>("satiety-arrow");

            _sleepProgress = tree.Q<ProgressBar>("sleep-progress");
            _sleepValue = tree.Q<Label>("sleep-value");
            _sleepArrow = tree.Q<VisualElement>("sleep-arrow");

            _happinessProgress = tree.Q<ProgressBar>("happiness-progress");
            _happinessValue = tree.Q<Label>("happiness-value");
            _happinessArrow = tree.Q<VisualElement>("happiness-arrow");

            _consciousnessProgress = tree.Q<ProgressBar>("consciousness-progress");
            _consciousnessValue = tree.Q<Label>("consciousness-value");
            _consciousnessArrow = tree.Q<VisualElement>("consciousness-arrow");

            _entertainmentProgress = tree.Q<ProgressBar>("entertainment-progress");
            _entertainmentValue = tree.Q<Label>("entertainment-value");
            _entertainmentArrow = tree.Q<VisualElement>("entertainment-arrow");
        }
        
        public void UpdateVitalityMaxValues(UnitVitality vitality)
        {
            _healthProgress.highValue = vitality.MaxHealth;
            
            _recoverySpeedProgress.lowValue = -vitality.MaxRecoverySpeed;
            _recoverySpeedProgress.highValue = vitality.MaxRecoverySpeed;
        }
        
        public void UpdateHealth(UnitVitality vitality)
        {
            _healthProgress.value = vitality.Health;
            _healthProgress.title = $"{(int)vitality.Health}/{vitality.MaxHealth}";
            _healthValue.text = $"{vitality.HealthPercent}%";
        }

        public void UpdateRecoverySpeed(UnitVitality vitality)
        {
            _recoverySpeedProgress.value = vitality.RecoverySpeed;
            _recoverySpeedProgress.title = $"{Math.Round(vitality.RecoverySpeed, 1)}/{Math.Round(vitality.MaxRecoverySpeed, 1)}";
            _recoverySpeedValue.text = $"{vitality.RecoverySpeedPercent}%";
        }

        public void UpdateSatiety(float satiety)
        {
            _satietyProgress.value = satiety;
            _satietyValue.text = $"{satiety}%";
        }

        public void UpdateSleep(float sleep)
        {
            _sleepProgress.value = sleep;
            _sleepValue.text = $"{sleep}%";
        }

        public void UpdateHappiness(float happiness)
        {
            _happinessProgress.value = happiness;
            _happinessValue.text = $"{happiness}%";
        }

        public void UpdateConsciousness(float consciousness)
        {
            _consciousnessProgress.value = consciousness;
            _consciousnessValue.text = $"{consciousness}%";
        }

        public void UpdateEntertainment(float entertainment)
        {
            _entertainmentProgress.value = entertainment;
            _entertainmentValue.text = $"{entertainment}%";
        }
    }
}
