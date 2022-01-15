using UnityEngine;

namespace Entities.Accessories
{
    public class Outlining : MonoBehaviour
    {
        private GameObject _outlineCopy;

        public void Activate()
        {
            if (_outlineCopy != null)
            {
                _outlineCopy.SetActive(true);
                return;
            }

            CreateOutlineCopy();
        }

        public void Deactivate()
        {
            if (_outlineCopy == null)
            {
                return;
            }

            _outlineCopy.SetActive(false);
        }

        private void CreateOutlineCopy()
        {
            _outlineCopy = Instantiate(gameObject, transform.position, transform.rotation, transform);
            _outlineCopy.layer = LayerMask.NameToLayer("Outlines");
            Destroy(_outlineCopy.GetComponent<Outlining>());
        }
    }
}
