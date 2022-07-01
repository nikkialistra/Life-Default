using Colonists;
using Sirenix.OdinInspector;
using UI.Game.GameLook.Components.Info.ColonistInfo;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components.Info.ColonistTabs
{
    [RequireComponent(typeof(ColonistInfoView))]
    public class ColonistEquipmentTab : MonoBehaviour
    {
        public bool Shown { get; private set; }
        private VisualElement Tree { get; set; }

        [Required]
        [SerializeField] private VisualTreeAsset _asset;

        private ColonistInfoView _parent;

        private Colonist _colonist;

        private void Awake()
        {
            _parent = GetComponent<ColonistInfoView>();

            Tree = _asset.CloneTree();

            BindElements();
        }

        public void FillIn(Colonist colonist)
        {
            UnsubscribeFromChanges();

            _colonist = colonist;

            SubscribeToChanges();
        }

        public void ShowSelf()
        {
            if (Shown) return;

            _parent.TabContent.Add(Tree);
            Shown = true;
        }

        public void HideSelf()
        {
            if (!Shown) return;

            UnsubscribeFromChanges();

            _parent.TabContent.Remove(Tree);
            Shown = false;
        }

        private void SubscribeToChanges()
        {

        }

        private void UnsubscribeFromChanges()
        {
            _colonist = null;
        }

        private void BindElements()
        {

        }
    }
}
