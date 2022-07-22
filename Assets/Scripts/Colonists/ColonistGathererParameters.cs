using System;
using Common;
using ResourceManagement;
using Sirenix.OdinInspector;
using Units.Stats;
using UnityEngine;

namespace Colonists
{
    public class ColonistGathererParameters : MonoBehaviour
    {
        public float InteractionDistanceFor(ResourceType resourceType) =>
            _resourceInteractionDistances[resourceType];

        public float InteractionDistanceFromColonistCenterFor(ResourceType resourceType) =>
            _resourceInteractionDistances[resourceType] + _distanceCorrectionFromCenter;

        public float ResourceDestructionSpeed => _resourceDestructionSpeed;
        public float ResourceExtractionEfficiency => _resourceExtractionEfficiency;

        [ValidateInput(nameof(EveryResourceHasDistanceInteraction))]
        [SerializeField] private ResourceInteractionDistanceDictionary _resourceInteractionDistances;

        [SerializeField] private float _distanceCorrectionFromCenter = 2f;

        private float _resourceDestructionSpeed;
        private float _resourceExtractionEfficiency;

        public void BindStats(Stat<ColonistStat> resourceDestructionSpeed,
            Stat<ColonistStat> resourceExtractionEfficiency)
        {
            _resourceDestructionSpeed = resourceDestructionSpeed.Value;
            _resourceExtractionEfficiency = resourceExtractionEfficiency.Value;

            resourceDestructionSpeed.ValueChange += OnResourceDestructionSpeedChange;
            resourceExtractionEfficiency.ValueChange += OnResourceExtractionEfficiencyChange;
        }

        public void UnbindStats(Stat<ColonistStat> resourceDestructionSpeed,
            Stat<ColonistStat> resourceExtractionEfficiency)
        {
            resourceDestructionSpeed.ValueChange -= OnResourceDestructionSpeedChange;
            resourceExtractionEfficiency.ValueChange -= OnResourceExtractionEfficiencyChange;
        }

        private void OnResourceDestructionSpeedChange(float value)
        {
            _resourceDestructionSpeed = value;
        }

        private void OnResourceExtractionEfficiencyChange(float value)
        {
            _resourceExtractionEfficiency = value;
        }

        private bool EveryResourceHasDistanceInteraction(ResourceInteractionDistanceDictionary distances,
            ref string errorMessage)
        {
            foreach (var resourceType in (ResourceType[])Enum.GetValues(typeof(ResourceType)))
                if (!distances.ContainsKey(resourceType))
                {
                    errorMessage = $"{resourceType} don't have distance";
                    return false;
                }

            return true;
        }

        [Serializable]
        public class ResourceInteractionDistanceDictionary : SerializableDictionary<ResourceType, float> { }
    }
}
