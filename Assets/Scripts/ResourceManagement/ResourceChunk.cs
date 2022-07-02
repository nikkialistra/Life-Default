using System;
using System.Collections;
using Entities;
using General.Interfaces;
using UI.Game.GameLook.Components.Info;
using UnityEngine;

namespace ResourceManagement
{
    [RequireComponent(typeof(Entity))]
    [RequireComponent(typeof(EntitySelection))]
    [RequireComponent(typeof(Rigidbody))]
    public class ResourceChunk : MonoBehaviour, ISelectable
    {
        public event Action<ResourceChunk> ResourceChunkDestroying;
        public event Action Destroying;

        public Entity Entity { get; private set; }

        public ResourceType ResourceType { get; private set; }
        public int Quantity { get; private set; }

        public string Name => _name;


        [SerializeField] private string _name;

        private Rigidbody _rigidbody;
        private EntitySelection _entitySelection;

        private InfoPanelView _infoPanelView;

        private void Awake()
        {
            Entity = GetComponent<Entity>();
            _entitySelection = GetComponent<EntitySelection>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Initialize(ResourceType resourceType, int quantity, float sizeMultiplier,
            InfoPanelView infoPanelView)
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

            StartCoroutine(CFreezeAfter(timeToFreeze));
        }

        public void Destroy()
        {
            ResourceChunkDestroying?.Invoke(this);
            Destroying?.Invoke();

            Destroy(gameObject);
        }

        private IEnumerator CFreezeAfter(float timeToFreeze)
        {
            yield return new WaitForSeconds(timeToFreeze);

            _rigidbody.isKinematic = true;
        }
    }
}
