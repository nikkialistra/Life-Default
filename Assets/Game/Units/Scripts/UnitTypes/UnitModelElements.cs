using UnityEngine;

namespace Game.Units.UnitTypes
{
    public class UnitModelElements : MonoBehaviour
    {
        [SerializeField] private Transform _headEnd;
        [SerializeField] private Transform _head;
        [SerializeField] private Transform _rightHand;
        [SerializeField] private Transform _upperChest;

        [SerializeField] private SkinnedMeshRenderer _skinnedMeshRenderer;

        public Transform HeadEnd => _headEnd;
        public Transform Head => _head;
        public Transform RightHand => _rightHand;
        public Transform UpperChest => _upperChest;

        public void SetSkin(Material skin)
        {
            var materials = _skinnedMeshRenderer.materials;
            materials[0] = skin;
            _skinnedMeshRenderer.materials = materials;
        }
    }
}