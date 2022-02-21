using System;
using System.Collections;
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

        public event Action<bool> Pause;
        public event Action X1;
        public event Action X2;
        public event Action X3;

        private void Start()
        {
            _x1.value = true;
        }

        private void OnEnable()
        {
            RegisterCallbacks();
        }

        private void OnDisable()
        {
            UnregisterCallbacks();
        }

        private void OnPauseToggle(ChangeEvent<bool> _)
        {
            Pause?.Invoke(_pause.value);
        }

        private void OnX1Toggle(ChangeEvent<bool> _)
        {
            UnregisterCallbacks();
            UncheckToggles();

            _x1.value = true;
            StartCoroutine(RegisterCallbacksAtNextFrame());

            X1?.Invoke();
        }

        private void OnX2Toggle(ChangeEvent<bool> _)
        {
            UnregisterCallbacks();
            UncheckToggles();

            _x2.value = true;
            StartCoroutine(RegisterCallbacksAtNextFrame());

            X2?.Invoke();
        }

        private void OnX3Toggle(ChangeEvent<bool> _)
        {
            UnregisterCallbacks();
            UncheckToggles();

            _x3.value = true;
            StartCoroutine(RegisterCallbacksAtNextFrame());

            X3?.Invoke();
        }

        private void UncheckToggles()
        {
            _pause.value = false;
            _x1.value = false;
            _x2.value = false;
            _x3.value = false;
        }

        private void RegisterCallbacks()
        {
            _pause.RegisterValueChangedCallback(OnPauseToggle);
            _x1.RegisterValueChangedCallback(OnX1Toggle);
            _x2.RegisterValueChangedCallback(OnX2Toggle);
            _x3.RegisterValueChangedCallback(OnX3Toggle);
        }

        private void UnregisterCallbacks()
        {
            _pause.UnregisterValueChangedCallback(OnPauseToggle);
            _x1.UnregisterValueChangedCallback(OnX1Toggle);
            _x2.UnregisterValueChangedCallback(OnX2Toggle);
            _x3.UnregisterValueChangedCallback(OnX3Toggle);
        }

        private IEnumerator RegisterCallbacksAtNextFrame()
        {
            yield return null;
            RegisterCallbacks();
        }
    }
}
