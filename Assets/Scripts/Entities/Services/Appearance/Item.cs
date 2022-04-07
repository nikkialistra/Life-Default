using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Entities.Services.Appearance
{
    [Serializable]
    public class Item
    {
        [HideLabel]
        [PreviewField(70, ObjectFieldAlignment.Left)]
        [HorizontalGroup("Split", 100)]
        [VerticalGroup("Split/Left")]
        public Mesh Mesh;

        [VerticalGroup("Split/Right")]
        public int Chance = 1;
        [VerticalGroup("Split/Right")]
        [ReadOnly]
        public float RelativeChance;
    }
}
