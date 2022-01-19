using Entities.Entity;
using UnityEngine;

namespace Buildings
{
    public class Building : MonoBehaviour
    {
        [SerializeField] private BuildingType _buildingType;
        
        public BuildingType BuildingType => _buildingType;
    }
}
