using Sirenix.OdinInspector;
using UI.Game;
using Units.Services;
using Units.Unit.UnitTypes;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace UI
{
    [RequireComponent(typeof(UIDocument))]
    public class GameViews : MonoBehaviour
    {
        [Required]
        [SerializeField] private UnitTypesView _unitTypesView;

        public bool MouseOverUi { get; private set; }

        private VisualElement _root;

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
        }

        private void OnEnable()
        {
            _root.RegisterCallback<MouseOverEvent, bool>(SetMouseOverUi, true);
            _root.RegisterCallback<MouseLeaveEvent, bool>(SetMouseOverUi, false);
        }

        private void OnDisable()
        {
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