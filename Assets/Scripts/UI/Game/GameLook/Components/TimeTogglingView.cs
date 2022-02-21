using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    public class TimeTogglingView : MonoBehaviour
    {
        private Toggle _pause;
        private Toggle _x1;
        private Toggle _x2;
        private Toggle _x3;

        private void Awake()
        {
            Tree = Resources.Load<VisualTreeAsset>("UI/Markup/GameLook/Components/TimeToggling").CloneTree();

            _pause = Tree.Q<Toggle>("pause");
            _x1 = Tree.Q<Toggle>("x1");
            _x2 = Tree.Q<Toggle>("x2");
            _x3 = Tree.Q<Toggle>("x3");
        }

        public VisualElement Tree { get; private set; }
    }
}
