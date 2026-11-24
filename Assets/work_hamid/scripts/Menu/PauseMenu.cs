using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

// In-game pause: the controller's Menu button (three lines) pauses and opens the Options panel
// as "PAUSED" with RESUME + QUIT TO MENU. Reads the button directly, so PlayerControls isn't changed.
public class PauseMenu : MonoBehaviour
{
    public OptionsPanel options;

    [Tooltip("Scene QUIT TO MENU loads")]
    public string menuSceneName = "MainMenu_Full";

    bool paused;
    InputManager[] playerInputs;

    void Awake()
    {
        // The game scene may not have an EventSystem yet; menus need one for the controller
        if (EventSystem.current == null && FindAnyObjectByType<EventSystem>() == null)
        {
            GameObject es = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            es.GetComponent<InputSystemUIInputModule>().deselectOnBackgroundClick = false;
        }

        if (options != null)
        {
            options.Hide();
            options.onClose = Resume;
            options.onQuit = QuitToMenu;
        }
    }

    void Update()
    {
        Gamepad pad = Gamepad.current;
        if (pad == null || !pad.startButton.wasPressedThisFrame) return;

        if (paused) Resume();
        else if (Time.timeScale > 0f) Pause(); // don't pause on top of the Game Over screen
    }

    public void Pause()
    {
        paused = true;
        Time.timeScale = 0f;

        // Stop the pigeon reacting to buttons while paused (RB would toggle poop mode otherwise)
        playerInputs = FindObjectsByType<InputManager>(FindObjectsSortMode.None);
        foreach (InputManager input in playerInputs) input.enabled = false;

        options.Open(OptionsPanel.Mode.Pause, true);
    }

    public void Resume()
    {
        options.Hide();
        Time.timeScale = 1f;

        if (playerInputs != null)
            foreach (InputManager input in playerInputs)
                if (input != null) input.enabled = true;

        paused = false;
        if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
    }

    void QuitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}
