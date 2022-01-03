using System.Collections;
using Entities;
using Entities.Accessories;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Units.Unit
{
    public class UnitRenderer : MonoBehaviour, IHoverable
    {
        [Required]
        [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;
        [Required]
        [SerializeField] private Material _outline;
        [MinValue(0)]
        [SerializeField] private float _waitingTimeValue = 0.05f;

        private WaitForSeconds _waitingTime;

        private Coroutine _hideOutlineCoroutine;

        private void Awake()
        {
            _waitingTime = new WaitForSeconds(_waitingTimeValue);
        }

        private void Start()
        {
            if (_skinnedMeshRenderer.materials.Length != 2)
            {
                AddPlaceForOutlineMaterial();
            }
        }

        private void AddPlaceForOutlineMaterial()
        {
            var materials = _skinnedMeshRenderer.materials;
            var materialsArray = new Material[2];
            materialsArray[0] = materials[0];
            _skinnedMeshRenderer.materials = materialsArray;
        }

        public void OnHover()
        {
            if (_hideOutlineCoroutine != null)
            {
                StopCoroutine(_hideOutlineCoroutine);
            }

            var materials = _skinnedMeshRenderer.materials;
            materials[1] = _outline;
            _skinnedMeshRenderer.materials = materials;

            ShowAccessoriesOutline();

            _hideOutlineCoroutine = StartCoroutine(HideOutline());
        }

        private void ShowAccessoriesOutline()
        {
            var accessories = GetComponentsInChildren<Outlining>();
            foreach (var accessory in accessories)
            {
                accessory.Activate();
            }
        }

        private void HideAccessoriesOutline()
        {
            var accessories = GetComponentsInChildren<Outlining>();
            foreach (var accessory in accessories)
            {
                accessory.Deactivate();
            }
        }

        private IEnumerator HideOutline()
        {
            yield return _waitingTime;

            var materials = _skinnedMeshRenderer.materials;
            materials[1] = null;
            _skinnedMeshRenderer.materials = materials;

            HideAccessoriesOutline();
        }
    }
}