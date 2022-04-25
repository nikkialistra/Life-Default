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
        [SerializeField] private EntityType _entityType;

        [ShowIf(nameof(_entityType), EntityType.Resource)]
        [ValidateInput(nameof(ResourceEntityShouldHaveResource), "Resource entity should have resource")]
        [SerializeField] private Resource _resource;
        
        [ShowIf(nameof(_entityType), EntityType.Building)]
        [ValidateInput(nameof(BuildingEntityShouldHaveBuilding), "Building entity should have resource")]
        [SerializeField] private Building _building;
        
        private bool _died;

        public event Action<Entity> EntityDie;

        public EntityType EntityType => _entityType;
        public bool Alive => !_died;

        public Resource Resource => _resource;
        public Building Building => _building;
        
        private void OnEnable()
        {
            switch (_entityType)
            {
                case EntityType.Resource:
                    _resource.Die += Dying;
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
                    _resource.Die -= Dying;
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
                    _resource.Select();
                    break;
                case EntityType.Building:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void Dying()
        {
            _died = true;
            
            EntityDie?.Invoke(this);
        }

        private bool ResourceEntityShouldHaveResource()
        {
            if (_entityType == EntityType.Resource && _resource == null)
            {
                return false;
            }

            return true;
        }

        private bool BuildingEntityShouldHaveBuilding()
        {
            if (_entityType == EntityType.Building && _building == null)
            {
                return false;
            }

            return true;
        }
    }
}
