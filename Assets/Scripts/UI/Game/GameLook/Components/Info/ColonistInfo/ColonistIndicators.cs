using System;
using Sirenix.OdinInspector;
using Units;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.Info.ColonistInfo
{
    public class ColonistIndicators : MonoBehaviour
    {
        [Required]
        [SerializeField] private Sprite _singleArrowUp;
        [Required]
        [SerializeField] private Sprite _singleArrowDown;
        [Required]
        [SerializeField] private Sprite _doubleArrowUp;
        [Required]
        [SerializeField] private Sprite _doubleArrowDown;
        [Required]
        [SerializeField] private Sprite _tripleArrowUp;
        [Required]
        [SerializeField] private Sprite _tripleArrowDown;

        [SerializeField] private float _changeSpeedForSingleArrow = 0.005f;
        [SerializeField] private float _changeSpeedForDoubleArrow = 0.0625f;
        [SerializeField] private float _changeSpeedForTripleArrow = 0.125f;

        private Sprite _noneArrow;

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

        public void UpdateVitality(UnitVitality vitality)
        {
            _healthProgress.highValue = vitality.MaxHealth;

            _recoverySpeedProgress.lowValue = -vitality.MaxRecoverySpeed;
            _recoverySpeedProgress.highValue = vitality.MaxRecoverySpeed;

            UpdateHealth(vitality);
            UpdateRecoverySpeed(vitality);
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

        private void UpdateHealth(UnitVitality vitality)
        {
            _healthProgress.value = vitality.Health;
            _healthProgress.title = $"{(int)vitality.Health}/{vitality.MaxHealth}";
            _healthValue.text = $"{vitality.HealthPercent}%";

            SetArrow(vitality);
        }

        private void SetArrow(UnitVitality vitality)
        {
            var arrowConstraint = vitality.IsFullHealth ? ArrowConstraint.DownArrows : ArrowConstraint.None;
            var arrow = ChooseArrow(vitality.RecoverySpeed / vitality.MaxHealth, arrowConstraint);

            _healthArrow.style.backgroundImage = new StyleBackground(arrow);
        }

        private void UpdateRecoverySpeed(UnitVitality vitality)
        {
            _recoverySpeedProgress.value = vitality.RecoverySpeed;
            _recoverySpeedProgress.title =
                $"{Math.Round(vitality.RecoverySpeed, 1)}/{Math.Round(vitality.MaxRecoverySpeed, 1)}";
            _recoverySpeedValue.text = $"{vitality.RecoverySpeedPercent}%";
        }

        private Sprite ChooseArrow(float changeSpeedFraction, ArrowConstraint constraint)
        {
            if (Mathf.Abs(changeSpeedFraction) <= _changeSpeedForSingleArrow ||
                (constraint == ArrowConstraint.DownArrows && changeSpeedFraction > 0))
            {
                return _noneArrow;
            }

            return changeSpeedFraction > 0
                ? ChooseArrowUp(changeSpeedFraction)
                : ChooseArrowDown(Mathf.Abs(changeSpeedFraction));
        }

        private Sprite ChooseArrowUp(float changeSpeedFraction)
        {
            if (changeSpeedFraction > _changeSpeedForTripleArrow)
                return _tripleArrowUp;

            if (changeSpeedFraction > _changeSpeedForDoubleArrow)
                return _doubleArrowUp;

            return _singleArrowUp;
        }

        private Sprite ChooseArrowDown(float changeSpeedFraction)
        {
            if (changeSpeedFraction > _changeSpeedForTripleArrow)
                return _tripleArrowDown;

            if (changeSpeedFraction > _changeSpeedForDoubleArrow)
                return _doubleArrowDown;

            return _singleArrowDown;
        }

        private enum ArrowConstraint
        {
            DownArrows,
            None
        }
    }
}
