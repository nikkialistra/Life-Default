using System;
using Kernel.UI.GameViews;
using UnityEngine;
using Zenject;

namespace Game.Units.Services
{
    public class UnitChoosing : MonoBehaviour
    {
        private UnitTypesView _unitTypesView;

        [Inject]
        public void Construct(UnitTypesView unitTypesView)
        {
            _unitTypesView = unitTypesView;
        }

        private void OnEnable()
        {
            _unitTypesView.LeftClick += ChooseUnit;
            _unitTypesView.RightClick += ChooseUnits;
        }

        private void OnDisable()
        {
            _unitTypesView.LeftClick -= ChooseUnit;
            _unitTypesView.RightClick -= ChooseUnits;
        }

        private void ChooseUnit(UnitType unitType)
        {
            Debug.Log("choosing " + unitType);
        }

        private void ChooseUnits(UnitType unitType)
        {
            Debug.Log("choosing all " + unitType);
        }
    }
}