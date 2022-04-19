using Entities;
using Entities.Interfaces;
using UI.Game.GameLook.Components;
using UnityEngine;
using Zenject;

namespace ResourceManagement
{
    [RequireComponent(typeof(EntitySelection))]
    [RequireComponent(typeof(Rigidbody))]
    public class ResourceChunk : MonoBehaviour, ISelectable
    {
        [SerializeField] private string _name;
        
        private Rigidbody _ridigbody;
        private EntitySelection _entitySelection;
        
        private InfoPanelView _infoPanelView;

        [Inject]
        public void Construct(InfoPanelView infoPanelView)
        {
            _infoPanelView = infoPanelView;
        }

        private void Awake()
        {
            _entitySelection = GetComponent<EntitySelection>();
            _ridigbody = GetComponent<Rigidbody>();
        }

        public string Name => _name;
        public ResourceType ResourceType { get; private set; }
        public int Quantity { get; private set; }
        
        public void Initialize(ResourceType resourceType, int quantity)
        {
            ResourceType = resourceType;
            Quantity = quantity;
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

        public void BurstOutTo(Vector3 randomForce)
        {
            _ridigbody.velocity = randomForce;
        }
    }
}
