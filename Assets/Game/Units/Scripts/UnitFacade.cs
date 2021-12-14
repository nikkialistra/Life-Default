using Kernel.Types;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Game.Units
{
    public class UnitFacade : MonoBehaviour, ISelectable
    {
        [Required]
        [SerializeField] private GameObject _selectionIndicator;

        public GameObject GameObject => gameObject;

        public void OnSelect()
        {
            _selectionIndicator.SetActive(true);
        }

        public void OnDeselect() => _selectionIndicator.SetActive(false);
        
        public class Factory : PlaceholderFactory<UnitFacade>
        {
        }
    }
}