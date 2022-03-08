using System;
using Sirenix.OdinInspector;

namespace Colonists.Colonist
{
    [Serializable]
    public class ColonistIndicators
    {
        [ProgressBar(0, 1, r: 0.929f, g: 0.145f, b: 0.145f, Height = 20)]
        public float Health;
        [ProgressBar(0, 1, r: 0.929f, g: 0.145f, b: 0.145f, Height = 20)]
        public float Blood;

        [ProgressBar(0, 1, r: 0.0941f, g: 0.854f, b: 0.854f, Height = 20)]
        public float Satiety;
        [ProgressBar(0, 1, r: 0.0941f, g: 0.854f, b: 0.854f, Height = 20)]
        public float Consciousness;
        [ProgressBar(0, 1, r: 0.0941f, g: 0.854f, b: 0.854f, Height = 20)]
        public float Sleep;
        [ProgressBar(0, 1, r: 0.0941f, g: 0.854f, b: 0.854f, Height = 20)]
        public float Happiness;
        [ProgressBar(0, 1, r: 0.0941f, g: 0.854f, b: 0.854f, Height = 20)]
        public float Entertainment;
    }
}
