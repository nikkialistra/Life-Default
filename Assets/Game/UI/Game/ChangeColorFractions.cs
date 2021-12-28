using UnityEngine;

namespace Game.UI.Game
{
    public class ChangeColorFractions : MonoBehaviour
    {
        [Range(0, 1)]
        public float Middle;
        [Range(0, 1)]
        public float Low;
    }
}