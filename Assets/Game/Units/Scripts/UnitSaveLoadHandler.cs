using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Units
{
    [RequireComponent(typeof(UnitFacade))]
    public class UnitSaveLoadHandler : MonoBehaviour
    {
        private UnitFacade _unit;

        private UnitData _unitData;

        private void Awake()
        {
            _unit = GetComponent<UnitFacade>();
        }

        private void Start()
        {
            if (!string.IsNullOrEmpty(_unitData.Id))
            {
                return;
            }

            _unitData.Id = DateTimeOffset.Now.ToUnixTimeMilliseconds().ToString() +
                           Random.Range(0, int.MaxValue);

            _unitData.Type = _unit.UnitType;
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }

        public UnitData GetUnitData()
        {
            _unitData.Position = transform.position;
            _unitData.Rotation = transform.rotation;
            return _unitData;
        }

        public void SetUnitData(UnitData unitData)
        {
            _unitData = unitData;
            transform.position = _unitData.Position;
            transform.rotation = _unitData.Rotation;
        }
    }
}