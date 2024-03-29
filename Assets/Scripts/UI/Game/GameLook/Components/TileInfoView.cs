﻿using Medium.TileManagement.Tiles;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Game.GameLook.Components
{
    public class TileInfoView : MonoBehaviour
    {
        public VisualElement Tree { get; private set; }

        [Required]
        [SerializeField] private VisualTreeAsset _asset;

        private VisualElement _tileInfo;

        private Label _position;
        private Label _temperature;
        private Label _light;
        private Label _beauty;

        private void Awake()
        {
            Tree = _asset.CloneTree();

            _tileInfo = Tree.Q<VisualElement>("tile-info");

            _position = Tree.Q<Label>("position");
            _temperature = Tree.Q<Label>("temperature");
            _light = Tree.Q<Label>("light");
            _beauty = Tree.Q<Label>("beauty");
        }

        public void ShowFor(Tile tile)
        {
            _tileInfo.RemoveFromClassList("not-displayed");

            _position.text = $"Position: {tile.Position}";
            _temperature.text = $"Temperature: {tile.Temperature} °C";
            _light.text = $"Light: {tile.Light}%";
            _beauty.text = $"Beauty: {tile.Beauty}";
        }

        public void Hide()
        {
            _tileInfo.AddToClassList("not-displayed");
        }
    }
}
