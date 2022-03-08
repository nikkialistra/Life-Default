using Units.Services.Selecting;
using Units.Unit;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace UI.Game.GameLook.Components
{
    [RequireComponent(typeof(InfoPanelView))]
    [RequireComponent(typeof(ChangeColorFractions))]
    public class ColonistInfoView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/ColonistInfo";

        private bool _shown;
        
        private UnitFacade _unit;

        private InfoPanelView _parent;
        private TemplateContainer _tree;

        private Label _name;

        private Button _next;
        private Button _focus;

        private VisualElement _picture;

        private ProgressBar _healthProgress;
        private Label _healthValue;
        private VisualElement _healthArrow;

        private UnitChoosing _unitChoosing;

        [Inject]
        public void Construct(UnitChoosing unitChoosing)
        {
            _unitChoosing = unitChoosing;
        }

        private void Awake()
        {
            _parent = GetComponent<InfoPanelView>();

            _tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();
            _tree.pickingMode = PickingMode.Ignore;

            _name = _tree.Q<Label>("name");

            _next = _tree.Q<Button>("next");
            _focus = _tree.Q<Button>("focus");

            _picture = _tree.Q<VisualElement>("picture");

            _healthProgress = _tree.Q<ProgressBar>("health-progress");
            _healthValue = _tree.Q<Label>("health-value");
            _healthArrow = _tree.Q<VisualElement>("health-arrow");
        }

        private void OnDestroy()
        {
            UnsubscribeFromLastUnit();
        }

        public void ShowSelf()
        {
            if (_shown)
            {
                return;
            }

            _shown = true;
            
            _parent.InfoPanel.Add(_tree);
            _next.clicked += OnNext;
        }

        public void HideSelf()
        {
            if (!_shown)
            {
                return;
            }

            _next.clicked -= OnNext;
            _parent.InfoPanel.Remove(_tree);

            _shown = false;
        }

        public void FillIn(UnitFacade unit)
        {
            FillInPreview(unit);
            FillInProperties(unit);
        }

        private void OnNext()
        {
           _unitChoosing.NextUnitTo(_unit);
        }

        private void HidePanel()
        {
            _parent.HideSelf();
        }

        private void FillInPreview(UnitFacade unit)
        {
            // _image.style.backgroundImage = ;
        }

        private void FillInProperties(UnitFacade unit)
        {
            UnsubscribeFromLastUnit();
            _unit = unit;
            
            _name.text = unit.Name;

            ChangeHealth();

            SubscribeToUnit(unit);
        }

        private void SubscribeToUnit(UnitFacade unit)
        {
            unit.HealthChange += ChangeHealth;
            unit.Die += HidePanel;
        }

        private void UnsubscribeFromLastUnit()
        {
            if (_unit != null)
            {
                _unit.HealthChange -= ChangeHealth;
                _unit.Die -= HidePanel;
            }
        }

        private void ChangeHealth()
        {
            _healthProgress.value = (float)_unit.Health / _unit.MaxHealth;
            _healthValue.text = $"{_healthProgress.value * 100}%";
        }
    }
}
