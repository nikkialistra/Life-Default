using MapGeneration.Generators;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace UI.Game
{
    public class LoadingScreen : MonoBehaviour
    {
        private VisualElement _root;

        private VisualElement _loadingScreen;

        private MapGenerator _mapGenerator;

        [Inject]
        public void Construct(MapGenerator mapGenerator)
        {
            _mapGenerator = mapGenerator;
        }

        private void Awake()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            _loadingScreen = _root.Q<VisualElement>("loading-screen");
        }

        private void OnEnable()
        {
            _mapGenerator.Load += Hide;
        }

        private void OnDisable()
        {
            _mapGenerator.Load -= Hide;
        }

        private void Hide()
        {
            _loadingScreen.AddToClassList("not-displayed");
        }
    }
}