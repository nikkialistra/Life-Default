using Game.UI.Game;
using Game.Units.Services;
using Game.Units.Unit;
using Game.Units.UnitTypes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Game.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class GameViews : MonoBehaviour
    {
        [Required]
        [SerializeField] private UnitTypesView _unitTypesView;

        public bool MouseOverUi { get; private set; }

        private UnitsTypeCounts _unitsTypeCounts;

        private VisualElement _root;

        [Inject]
        public void Construct(UnitsTypeCounts unitsTypeCounts)
        {
            _unitsTypeCounts = unitsTypeCounts;
        }

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
        }

        private void OnEnable()
        {
            _unitsTypeCounts.UnitTypeCountChange += ChangeUnitsTypeCount;
            
            _root.RegisterCallback<MouseOverEvent, bool>(SetMouseOverUi, true);
            _root.RegisterCallback<MouseLeaveEvent, bool>(SetMouseOverUi, false);
        }

        private void OnDisable()
        {
            _unitsTypeCounts.UnitTypeCountChange -= ChangeUnitsTypeCount;
            
            _root.UnregisterCallback<MouseOverEvent, bool>(SetMouseOverUi);
            _root.UnregisterCallback<MouseLeaveEvent, bool>(SetMouseOverUi);
        }

        private void SetMouseOverUi(IMouseEvent mouseEvent, bool value)
        {
            MouseOverUi = value;
        }

        private void ChangeUnitsTypeCount(UnitType unitType, float count)
        {
            _unitTypesView.ChangeUnitTypeCount(unitType, count);
        }
    }
}