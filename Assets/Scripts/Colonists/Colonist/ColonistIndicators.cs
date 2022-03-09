using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Colonists.Colonist
{
    [Serializable]
    public class ColonistIndicators : MonoBehaviour
    {
        [ProgressBar(0, 100, r: 0.0941f, g: 0.854f, b: 0.854f, Height = 20)]
        public float Satiety;
        [ProgressBar(0, 100, r: 0.0941f, g: 0.854f, b: 0.854f, Height = 20)]
        public float Sleep;
        [ProgressBar(0, 100, r: 0.0941f, g: 0.854f, b: 0.854f, Height = 20)]
        public float Happiness;
        [ProgressBar(0, 100, r: 0.0941f, g: 0.854f, b: 0.854f, Height = 20)]
        public float Consciousness;
        [ProgressBar(0, 100, r: 0.0941f, g: 0.854f, b: 0.854f, Height = 20)]
        public float Entertainment;
    }
}
