using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    public class TimeWeatherView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/TimeWeather";
        
        private void Awake()
        {
            Tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();
        }

        public VisualElement Tree { get; private set; }
    }
}
