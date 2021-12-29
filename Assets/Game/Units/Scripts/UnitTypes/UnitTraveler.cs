using UnityEngine;

namespace Game.Units.UnitTypes
{
    [RequireComponent(typeof(UnitModelElements))]
    public class UnitTraveler : MonoBehaviour
    {
        [SerializeField] private GameObject _cap;
        [SerializeField] private Material _skin;
        
        private UnitModelElements _unitModelElements;

        private void Awake()
        {
            _unitModelElements = GetComponent<UnitModelElements>();
        }

        public void Initialize()
        {
            Instantiate(_cap, _unitModelElements.HeadEnd);
            _unitModelElements.SetSkin(_skin);
        }
    }
}