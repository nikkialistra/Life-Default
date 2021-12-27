using System;
using Game.Units.Unit;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Game.UI.Game
{
    public class UnitDescriptionView : MonoBehaviour
    {
        [Title("Previews")]
        [Required]
        [SerializeField] private Texture2D _travelerPreview;
        [Required]
        [SerializeField] private Texture2D _lumberjackPreview;
        [Required]
        [SerializeField] private Texture2D _masonPreview;
        [Required]
        [SerializeField] private Texture2D _meleePreview;
        [Required]
        [SerializeField] private Texture2D _archerPreview;

        private InfoPanelView _parent;
        private TemplateContainer _tree;

        private VisualElement _image;

        private Label _nominationType;
        private Label _nominationName;
        private ProgressBar _health;

        [Inject]
        public void Construct(InfoPanelView infoPanelView)
        {
            _parent = infoPanelView;
        }

        private void Awake()
        {
            _tree = Resources.Load<VisualTreeAsset>("UI/Markup/Components/UnitInfo").CloneTree();

            _image = _tree.Q<VisualElement>("image");
            
            _nominationType = _tree.Q<Label>("unit-nomination__type");
            _nominationName = _tree.Q<Label>("unit-nomination__name");
            _health = _tree.Q<ProgressBar>("unit-health__progress-bar");
        }

        public void ShowSelf()
        {
            _parent.Root.Add(_tree);
        }

        public void HideSelf()
        {
            _parent.Root.Remove(_tree);
        }

        public void FillIn(UnitFacade unit)
        {
            FillInPreview(unit);
            FillInProperties(unit);
        }

        private void FillInPreview(UnitFacade unit)
        {
            _image.style.backgroundImage = unit.UnitType switch
            {
                UnitType.Traveler => new StyleBackground(_travelerPreview),
                UnitType.Lumberjack => new StyleBackground(_lumberjackPreview),
                UnitType.Mason => new StyleBackground(_masonPreview),
                UnitType.Melee => new StyleBackground(_meleePreview),
                UnitType.Archer => new StyleBackground(_archerPreview),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void FillInProperties(UnitFacade unit)
        {
            _nominationType.text = unit.UnitType.ToString();
            _nominationName.text = unit.Name;
            _health.value = unit.Health;
        }
    }
}