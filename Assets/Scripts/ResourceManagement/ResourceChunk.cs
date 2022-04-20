using System.Collections;
using Entities;
using Entities.Interfaces;
using UI.Game.GameLook.Components;
using UnityEngine;

namespace ResourceManagement
{
    [RequireComponent(typeof(EntitySelection))]
    [RequireComponent(typeof(Rigidbody))]
    public class ResourceChunk : MonoBehaviour, ISelectable
    {
        [SerializeField] private string _name;

        private Rigidbody _rigidbody;
        private EntitySelection _entitySelection;
        
        private InfoPanelView _infoPanelView;
        
        private void Awake()
        {
            _entitySelection = GetComponent<EntitySelection>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        public string Name => _name;
        public ResourceType ResourceType { get; private set; }
        public int Quantity { get; private set; }
        
        public void Initialize(ResourceType resourceType, int quantity, float sizeMultiplier, InfoPanelView infoPanelView)
        {
            ResourceType = resourceType;
            Quantity = quantity;

            _infoPanelView = infoPanelView;

            transform.localScale *= sizeMultiplier;
        }

        public void Hover()
        {
            _entitySelection.Hover();
        }

        public void Flash()
        {
            _entitySelection.Flash();
        }

        public void Select()
        {
            _entitySelection.Select();
            _infoPanelView.SetResourceChunk(this);
        }

        public void Deselect()
        {
            _entitySelection.Deselect();
            _infoPanelView.UnsetResourceChunk(this);
        }

        public void StopDisplay()
        {
            _entitySelection.StopDisplay();
        }

        public void BurstOutTo(Vector3 randomForce, float timeToFreeze)
        {
            _rigidbody.velocity = randomForce;
            
            StartCoroutine(FreezeAfter(timeToFreeze));
        }

        private IEnumerator FreezeAfter(float timeToFreeze)
        {
            yield return new WaitForSeconds(timeToFreeze);

            _rigidbody.isKinematic = true;
        }
    }
}
