using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Colonists.Colonist.Appearance
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
        public int Chance;
        [VerticalGroup("Split/Right")]
        [ReadOnly]
        public float RelativeChance;
    }
}
