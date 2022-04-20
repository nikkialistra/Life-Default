using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Services.Appearance.ItemVariants.Item
{
    [Serializable]
    public class ItemObject<T> : IItem<T> where T : UnityEngine.Object
    {
        [HorizontalGroup("Split", 100)]
        [VerticalGroup("Split/Left")]
        [HideLabel]
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [SerializeField] private T _value;

        [VerticalGroup("Split/Right")]
        [SerializeField] private int _chance = 1;
        
        [VerticalGroup("Split/Right")]
        [ReadOnly]
        [SerializeField] private float _relativeChance;

        public T Value => _value;
        public int Chance => _chance;
        public float RelativeChance
        {
            get => _relativeChance;
            set => _relativeChance = value;
        }
    }
}
