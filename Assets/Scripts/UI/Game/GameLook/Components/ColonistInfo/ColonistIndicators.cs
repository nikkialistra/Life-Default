using Entities.Creature;
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
        
        private ProgressBar _vitalityProgress;
        private Label _vitalityValue;
        private VisualElement _vitalityArrow;

        private ProgressBar _bloodProgress;
        private Label _bloodValue;
        private VisualElement _bloodArrow;

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
            _vitalityProgress = tree.Q<ProgressBar>("vitality-progress");
            _vitalityValue = tree.Q<Label>("vitality-value");
            _vitalityArrow = tree.Q<VisualElement>("vitality-arrow");

            _bloodProgress = tree.Q<ProgressBar>("blood-progress");
            _bloodValue = tree.Q<Label>("blood-value");
            _bloodArrow = tree.Q<VisualElement>("blood-arrow");
            
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
        
        public void UpdateVitality(EntityHealth health)
        {
            _vitalityProgress.value = health.Vitality;
            _vitalityValue.text = $"{health.VitalityPercent}%";
        }

        public void UpdateBlood(EntityHealth health)
        {
            _bloodProgress.value = health.Blood;
            _bloodValue.text = $"{health.BloodPercent}%";
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
