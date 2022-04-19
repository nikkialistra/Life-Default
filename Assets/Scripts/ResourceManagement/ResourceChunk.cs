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

        [Space]
        [SerializeField] private float _minScale = 0.9f;
        [SerializeField] private float _maxScale = 1.1f;

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
        
        public void Initialize(ResourceType resourceType, int quantity, InfoPanelView infoPanelView)
        {
            ResourceType = resourceType;
            Quantity = quantity;

            _infoPanelView = infoPanelView;

            var scale = Random.Range(_minScale, _maxScale);

            transform.localScale = new Vector3(Random.Range(_minScale, _maxScale),
                Random.Range(_minScale, _maxScale),
                Random.Range(_minScale, _maxScale));
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
