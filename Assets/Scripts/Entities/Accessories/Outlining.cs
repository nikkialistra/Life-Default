using UnityEngine;

namespace Entities.Accessories
{
    public class Outlining : MonoBehaviour
    {
        [SerializeField] private Material _outline;

        private GameObject _outlineCopy;

        public void Activate()
        {
            if (_outlineCopy != null)
            {
                _outlineCopy.SetActive(true);
                return;
            }

            CreateOutlineCopy();
            ChangeOutlineCopyMaterials();
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
            Destroy(_outlineCopy.GetComponent<Outlining>());
        }

        private void ChangeOutlineCopyMaterials()
        {
            var renderers = _outlineCopy.GetComponents<Renderer>();

            foreach (var renderer in renderers)
            {
                var materials = renderer.materials;
                for (var i = 0; i < materials.Length; i++)
                {
                    materials[i] = _outline;
                }

                renderer.materials = materials;
            }
        }
    }
}