using Game.Units;
using Game.Units.Services;
using Kernel.UI.GameViews;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Kernel.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class GameViewsManager : MonoBehaviour
    {
        [Required]
        [SerializeField] private UnitTypesView _unitTypesView;
        
        private UnitTypeCounts _unitTypeCounts;

        [Inject]
        public void Construct(UnitTypeCounts unitTypeCounts)
        {
            _unitTypeCounts = unitTypeCounts;
        }

        private void OnEnable()
        {
            _unitTypeCounts.UnitTypeCountChange += ChangeUnitTypeCount;
        }

        private void OnDisable()
        {
            _unitTypeCounts.UnitTypeCountChange -= ChangeUnitTypeCount;
        }

        private void ChangeUnitTypeCount(UnitType unitType, float count)
        {
            _unitTypesView.ChangeUnitTypeCount(unitType, count);
        }
    }
}