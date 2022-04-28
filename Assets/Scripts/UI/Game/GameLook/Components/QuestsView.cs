using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    [RequireComponent(typeof(StockView))]
    public class QuestsView : MonoBehaviour
    {
        [SerializeField] private VisualTreeAsset _asset;

        private StockView _parent;

        private void Awake()
        {
            _parent = GetComponent<StockView>();
            
            Tree = _asset.CloneTree();
        }
        
        public bool Shown { get; private set; }
        private VisualElement Tree { get; set; }

        public void ShowSelf()
        {
            if (Shown)
            {
                return;
            }

            _parent.Content.Add(Tree);
            Shown = true;
        }

        public void HideSelf()
        {
            if (!Shown)
            {
                return;
            }
            
            _parent.Content.Remove(Tree);
            Shown = false;
        }
    }
}
