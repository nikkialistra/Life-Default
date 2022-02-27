using Environment.TileManagement.Tiles;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    public class TileInfoView : MonoBehaviour
    {
        private const string VisualTreePath = "UI/Markup/GameLook/Components/TileInfo";

        private Label _position;
        private Label _temperature;
        private Label _light;
        private Label _beauty;

        private void Awake()
        {
            Tree = Resources.Load<VisualTreeAsset>(VisualTreePath).CloneTree();

            _position = Tree.Q<Label>("position");
            _temperature = Tree.Q<Label>("temperature");
            _light = Tree.Q<Label>("light");
            _beauty = Tree.Q<Label>("beauty");
        }

        public VisualElement Tree { get; private set; }

        public void ShowFor(Tile tile)
        {
            _position.text = $"Position: {tile.Position}";
            _temperature.text = $"Temperature: {tile.Temperature} °C";
            _light.text = $"Light: {tile.Light}%";
            _beauty.text = $"Beauty: {tile.Beauty}";
        }
    }
}
