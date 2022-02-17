using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI.Menus
{
    [RequireComponent(typeof(UIDocument))]
    public class MainMenuView : MonoBehaviour
    {
        private VisualElement _tree;

        private Button _newGame;
        private Button _loadGame;
        private Button _settings;
        private Button _exitGame;

        private void Awake()
        {
            _tree = GetComponent<UIDocument>().rootVisualElement;

            _newGame = _tree.Q<Button>("new-game");
            _loadGame = _tree.Q<Button>("load-game");
            _settings = _tree.Q<Button>("settings");
            _exitGame = _tree.Q<Button>("exit-game");
        }

        private void OnEnable()
        {
            _newGame.clicked += NewGame;
            _loadGame.clicked += LoadGame;
            _settings.clicked += Settings;
            _exitGame.clicked += ExitGame;
        }

        private void OnDisable()
        {
            _newGame.clicked -= NewGame;
            _loadGame.clicked -= LoadGame;
            _settings.clicked -= Settings;
            _exitGame.clicked -= ExitGame;
        }

        private static void NewGame()
        {
            SceneManager.LoadScene("Forest", LoadSceneMode.Single);
        }

        private static void LoadGame() { }

        private static void Settings() { }

        private void ExitGame()
        {
            Application.Quit();
        }
    }
}
