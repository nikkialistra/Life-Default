using Kernel.Types;
using UnityEngine;
using Zenject;

namespace Game.Units
{
    public class UnitFacade : MonoBehaviour, ISelectable
    {
        private GameObject _selectionIndicator;

        [Inject]
        public void Construct(GameObject selectionIndicator) => _selectionIndicator = selectionIndicator;

        public GameObject GameObject => gameObject;

        public void OnSelect()
        {
            if (_selectionIndicator == null)
                return;
            
            _selectionIndicator.SetActive(true);
        }

        public void OnDeselect() => _selectionIndicator.SetActive(false);
        
        public class Factory : PlaceholderFactory<UnitFacade>
        {
        }
    }
}