using System;
using Units.Stats;
using UnityEngine;

namespace Colonists
{
    public class ColonistStats : MonoBehaviour
    {
        public Stat<ColonistStat> ResourceDestructionSpeed => _resourceDestructionSpeed;
        public Stat<ColonistStat> ResourceExtractionEfficiency => _resourceExtractionEfficiency;

        [SerializeField] private Stat<ColonistStat> _resourceDestructionSpeed;
        [SerializeField] private Stat<ColonistStat> _resourceExtractionEfficiency;

        private void Awake()
        {
            InitializeStats();
        }

        public void AddStatModifier(StatModifier<ColonistStat> statModifier)
        {
            var stat = ChooseStat(statModifier);

            stat.AddModifier(statModifier);
        }

        public void RemoveStatModifier(StatModifier<ColonistStat> statModifier)
        {
            var stat = ChooseStat(statModifier);

            stat.RemoveModifier(statModifier);
        }

        private void InitializeStats()
        {
            _resourceDestructionSpeed.Initialize();
            _resourceExtractionEfficiency.Initialize();
        }

        private Stat<ColonistStat> ChooseStat(StatModifier<ColonistStat> statModifier)
        {
            var stat = statModifier.StatType switch
            {
                ColonistStat.ResourceDestructionSpeed => _resourceDestructionSpeed,
                ColonistStat.ResourceExtractionEfficiency => _resourceExtractionEfficiency,
                _ => throw new ArgumentOutOfRangeException()
            };
            return stat;
        }
    }
}
