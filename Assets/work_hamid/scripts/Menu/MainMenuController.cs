using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Start screen: PLAY loads the game, HOW TO PLAY / SETTINGS open the shared Options panel.
public class MainMenuController : MonoBehaviour
{
    [Tooltip("Scene PLAY loads. Change this to the final game scene's name after the merge.")]
    public string gameSceneName = "BetaTest_Base";

    public Button playButton;
    public Button howToPlayButton;
    public Button settingsButton;
    public OptionsPanel options;

    Button lastButton;   // which start button opened the panel, so we can go back to it

    void Start()
    {
        Time.timeScale = 1f; // in case we came here from the pause menu

        playButton.onClick.AddListener(Play);
        howToPlayButton.onClick.AddListener(() => OpenOptions(false, howToPlayButton));
        settingsButton.onClick.AddListener(() => OpenOptions(true, settingsButton));

        if (options != null)
        {
            options.Hide();
            options.onClose = () => Select(lastButton != null ? lastButton : playButton);
        }

        Select(playButton);
    }

    void Play()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    void OpenOptions(bool settingsTab, Button from)
    {
        lastButton = from;
        options.Open(OptionsPanel.Mode.StartMenu, settingsTab);
    }

    void Update()
    {
        // Keep a start button selected while the panel is closed (controller-only game)
        if (options != null && options.gameObject.activeSelf) return;
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
            Select(lastButton != null ? lastButton : playButton);
    }

    static void Select(Button b)
    {
        if (EventSystem.current == null || b == null) return;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(b.gameObject);
    }
}
