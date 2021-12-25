using Game.Units;
using Game.Units.Services;
using Kernel.UI.Game;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Kernel.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class GameViews : MonoBehaviour
    {
        [Required]
        [SerializeField] private UnitTypesView _unitTypesView;

        public bool MouseOverUi { get; private set; }

        private UnitTypeCounts _unitTypeCounts;

        private VisualElement _root;

        [Inject]
        public void Construct(UnitTypeCounts unitTypeCounts)
        {
            _unitTypeCounts = unitTypeCounts;
        }

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
        }

        private void OnEnable()
        {
            _unitTypeCounts.UnitTypeCountChange += ChangeUnitTypeCount;
            
            _root.RegisterCallback<MouseOverEvent, bool>(SetMouseOverUi, true);
            _root.RegisterCallback<MouseLeaveEvent, bool>(SetMouseOverUi, false);
        }

        private void OnDisable()
        {
            _unitTypeCounts.UnitTypeCountChange -= ChangeUnitTypeCount;
            
            _root.UnregisterCallback<MouseOverEvent, bool>(SetMouseOverUi);
            _root.UnregisterCallback<MouseLeaveEvent, bool>(SetMouseOverUi);
        }

        private void SetMouseOverUi(IMouseEvent mouseEvent, bool value)
        {
            MouseOverUi = value;
        }

        private void ChangeUnitTypeCount(UnitType unitType, float count)
        {
            _unitTypesView.ChangeUnitTypeCount(unitType, count);
        }
    }
}