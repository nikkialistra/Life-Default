using MapGeneration.Map;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace UI.Game
{
    public class LoadingScreen : MonoBehaviour
    {
        private VisualElement _root;

        private VisualElement _loadingScreen;

        private Map _map;

        [Inject]
        public void Construct(Map map)
        {
            _map = map;
        }

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            _loadingScreen = _root.Q<VisualElement>("loading-screen");
        }

        private void OnEnable()
        {
            _map.Load += Hide;
        }

        private void OnDisable()
        {
            _map.Load -= Hide;
        }

        private void Hide()
        {
            _loadingScreen.AddToClassList("not-displayed");
        }
    }
}
