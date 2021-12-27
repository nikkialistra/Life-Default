using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace Game.UI.Game
{
    [RequireComponent(typeof(WorldSpaceUIDocument))]
    public class HealthIndicatorView : MonoBehaviour
    {
        private VisualElement _tree;
        private ProgressBar _health;

        private bool _isInitialized;
        private bool _shown;

        private WorldSpaceUIDocument _worldSpaceUIDocument;
        
        private Camera _camera;

        [Inject]
        public void Construct(Camera camera)
        {
            _camera = camera;
        }

        private void Awake()
        {
             _worldSpaceUIDocument = GetComponent<WorldSpaceUIDocument>();
             _worldSpaceUIDocument.RebuildFinish += Initialize;
        }

        void Update() 
        {
            if (_shown)
            {
                transform.LookAt(transform.position + _camera.transform.rotation * Vector3.forward,
                    _camera.transform.rotation * Vector3.up);
            }
        }

        public void SetHealth(int value)
        {
            if (!_isInitialized)
            {
                return;
            }
            
            _health.value = value;
        }

        public void Show()
        {
            _tree.RemoveFromClassList("not-displayed");
            _shown = true;
        }

        public void Hide()
        {
            _tree.AddToClassList("not-displayed");
            _shown = false;
        }

        private void Initialize()
        {
            _tree = _worldSpaceUIDocument.UiDocument.rootVisualElement;
            _health = _tree.Q<ProgressBar>("world-health__progress-bar");
            
            Hide();
            
            _isInitialized = true;
        }
    }
}