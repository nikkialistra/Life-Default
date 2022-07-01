using System;
using Buildings;
using Entities.Types;
using ResourceManagement;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities
{
    public class Entity : MonoBehaviour
    {
        public event Action<Entity> EntityDestroying;

        public EntityType EntityType => _entityType;
        public bool Alive => !_destroyed;

        public Resource Resource => _resource;
        public ResourceChunk ResourceChunk => _resourceChunk;
        public Building Building => _building;

        [SerializeField] private EntityType _entityType;

        [ShowIf(nameof(_entityType), EntityType.Resource)]
        [ValidateInput(nameof(ResourceEntityShouldHaveResource), "Resource entity should have resource")]
        [SerializeField] private Resource _resource;

        [ShowIf(nameof(_entityType), EntityType.ResourceChunk)]
        [ValidateInput(nameof(ResourceChunkEntityShouldHaveResourceChunk),
            "Resource chunk entity should have resource chunk")]
        [SerializeField] private ResourceChunk _resourceChunk;

        [ShowIf(nameof(_entityType), EntityType.Building)]
        [ValidateInput(nameof(BuildingEntityShouldHaveBuilding), "Building entity should have resource")]
        [SerializeField] private Building _building;

        private bool _destroyed;

        private void OnEnable()
        {
            switch (_entityType)
            {
                case EntityType.Resource:
                    _resource.Destroying += OnDestroying;
                    break;
                case EntityType.ResourceChunk:
                    _resourceChunk.Destroying += OnDestroying;
                    break;
                case EntityType.Building:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDisable()
        {
            switch (_entityType)
            {
                case EntityType.Resource:
                    _resource.Destroying -= OnDestroying;
                    break;
                case EntityType.ResourceChunk:
                    _resourceChunk.Destroying -= OnDestroying;
                    break;
                case EntityType.Building:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Select()
        {
            switch (_entityType)
            {
                case EntityType.Resource:
                    _resource.Select();
                    break;
                case EntityType.ResourceChunk:
                    _resourceChunk.Select();
                    break;
                case EntityType.Building:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Deselect()
        {
            switch (_entityType)
            {
                case EntityType.Resource:
                    _resource.Deselect();
                    break;
                case EntityType.ResourceChunk:
                    _resourceChunk.Deselect();
                    break;
                case EntityType.Building:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Destroy()
        {
            if (_destroyed) return;

            switch (_entityType)
            {
                case EntityType.Resource:
                    _resource.Destroy();
                    break;
                case EntityType.ResourceChunk:
                    _resourceChunk.Destroy();
                    break;
                case EntityType.Building:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDestroying()
        {
            _destroyed = true;

            EntityDestroying?.Invoke(this);
        }

        private bool ResourceEntityShouldHaveResource()
        {
            if (_entityType == EntityType.Resource && _resource == null)
                return false;

            return true;
        }

        private bool ResourceChunkEntityShouldHaveResourceChunk()
        {
            if (_entityType == EntityType.ResourceChunk && _resourceChunk == null)
                return false;

            return true;
        }

        private bool BuildingEntityShouldHaveBuilding()
        {
            if (_entityType == EntityType.Building && _building == null)
                return false;

            return true;
        }
    }
}
