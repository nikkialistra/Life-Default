using System;
using Game.Units;
using UnityEngine;
using UnityEngine.UIElements;

namespace Kernel.UI.Game
{
    [RequireComponent(typeof(UIDocument))]
    public class InfoPanelView : MonoBehaviour
    {
        [SerializeField] private Texture2D _travelerPreview;
        [SerializeField] private Texture2D _lumberjackPreview;
        [SerializeField] private Texture2D _masonPreview;
        [SerializeField] private Texture2D _meleePreview;
        [SerializeField] private Texture2D _archerPreview;

        private VisualElement _tree;

        private VisualElement _infoPanel;
        private VisualElement _infoImage;

        private Label _type;
        private Label _name;
        private ProgressBar _health;

        private UnitFacade _unit;

        private void Awake()
        {
            _tree = GetComponent<UIDocument>().rootVisualElement;

            _infoPanel = _tree.Q<VisualElement>("info-panel");
            _infoImage = _tree.Q<VisualElement>("info-image");

            _type = _tree.Q<Label>("nomination__type");
            _name = _tree.Q<Label>("nomination__name");
            _health = _tree.Q<ProgressBar>("health__progress-bar");
        }

        private void Start()
        {
            _infoPanel.AddToClassList("not-displayed");
        }

        public void SetUnit(UnitFacade unitFacade)
        {
            _unit = unitFacade;

            FillIn();
        }

        private void FillIn()
        {
            _infoPanel.RemoveFromClassList("not-displayed");
            FillInPreview();
            FillInProperties();
        }

        private void FillInPreview()
        {
            _infoImage.style.backgroundImage = _unit.UnitType switch
            {
                UnitType.Traveler => new StyleBackground(_travelerPreview),
                UnitType.Lumberjack => new StyleBackground(_lumberjackPreview),
                UnitType.Mason => new StyleBackground(_masonPreview),
                UnitType.Melee => new StyleBackground(_meleePreview),
                UnitType.Archer => new StyleBackground(_archerPreview),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void FillInProperties()
        {
            _type.text = _unit.UnitType.ToString();
            _name.text = _unit.Name;
            _health.value = _unit.Health;
        }
    }
}