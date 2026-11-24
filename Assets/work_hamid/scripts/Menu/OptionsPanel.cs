using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// The shared Options panel: SETTINGS tab (3 sliders) + HOW TO PLAY tab (6-page carousel).
// Used by the start menu (title OPTIONS, Back button) and the pause menu (title PAUSED, Resume + Quit).
// Controller: LB/RB switch tabs, D-pad up/down choose, D-pad left/right adjust sliders or flip pages,
// A presses buttons, B goes back.
public class OptionsPanel : MonoBehaviour
{
    public enum Mode { StartMenu, Pause }

    [Header("Title")]
    public TextMeshProUGUI title;

    [Header("Tabs")]
    public Image settingsTab;
    public Image howToTab;
    public Sprite tabActive;
    public Sprite tabInactive;
    public GameObject settingsPage;
    public GameObject howToPage;

    [Header("Settings sliders")]
    public Slider sfxSlider;
    public Slider musicSlider;
    public Slider renderSlider;
    public TextMeshProUGUI sfxValue;
    public TextMeshProUGUI musicValue;
    public TextMeshProUGUI renderValue;

    [Header("How to Play carousel")]
    public GameObject[] pages;
    public Image[] dots;
    public Sprite dotActive;
    public Sprite dotInactive;
    public Image prevArrow;
    public Image nextArrow;
    public Sprite arrowPrev;
    public Sprite arrowPrevDisabled;
    public Sprite arrowNext;
    public Sprite arrowNextDisabled;

    [Header("Buttons")]
    public GameObject startButtons;   // holds BACK TO START
    public GameObject pauseButtons;   // holds RESUME + QUIT TO MENU
    public Button backButton;
    public Button resumeButton;
    public Button quitButton;

    // Set by MainMenuController / PauseMenu
    public Action onClose;   // Back, Resume, or B pressed
    public Action onQuit;    // QUIT TO MENU pressed

    Mode mode = Mode.StartMenu;
    bool onSettingsTab = true;
    int page;

    void Awake()
    {
        // Sliders: volumes 0-10 steps, render 0-2 (LOW/MED/HIGH). One D-pad press = one step.
        SetupSlider(sfxSlider, GameSettings.VolumeSteps, GameSettings.Sfx, v => { GameSettings.Sfx = v; RefreshLabels(); });
        SetupSlider(musicSlider, GameSettings.VolumeSteps, GameSettings.Music, v => { GameSettings.Music = v; RefreshLabels(); });
        SetupSlider(renderSlider, GameSettings.RenderNames.Length - 1, GameSettings.Render, v => { GameSettings.Render = v; RefreshLabels(); });
        RefreshLabels();

        if (backButton != null) backButton.onClick.AddListener(Close);
        if (resumeButton != null) resumeButton.onClick.AddListener(Close);
        if (quitButton != null) quitButton.onClick.AddListener(() => onQuit?.Invoke());
    }

    void SetupSlider(Slider slider, int max, int startValue, Action<int> onChange)
    {
        if (slider == null) return;
        slider.minValue = 0;
        slider.maxValue = max;
        slider.wholeNumbers = true;
        slider.SetValueWithoutNotify(startValue);
        slider.onValueChanged.AddListener(v => onChange(Mathf.RoundToInt(v)));
    }

    void RefreshLabels()
    {
        if (sfxValue != null) sfxValue.text = (GameSettings.Sfx * 10) + "%";
        if (musicValue != null) musicValue.text = (GameSettings.Music * 10) + "%";
        if (renderValue != null) renderValue.text = GameSettings.RenderNames[GameSettings.Render];
    }

    // Show the panel. showSettingsTab = true opens on SETTINGS, false opens on HOW TO PLAY.
    public void Open(Mode openMode, bool showSettingsTab)
    {
        mode = openMode;
        gameObject.SetActive(true);

        if (title != null) title.text = mode == Mode.Pause ? "PAUSED" : "OPTIONS";
        if (startButtons != null) startButtons.SetActive(mode == Mode.StartMenu);
        if (pauseButtons != null) pauseButtons.SetActive(mode == Mode.Pause);

        // Make sure sliders show the saved values
        if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(GameSettings.Sfx);
        if (musicSlider != null) musicSlider.SetValueWithoutNotify(GameSettings.Music);
        if (renderSlider != null) renderSlider.SetValueWithoutNotify(GameSettings.Render);
        RefreshLabels();

        page = 0;
        ShowPage(0);
        ShowTab(showSettingsTab);
    }

    // Hide the panel and tell whoever opened it (start menu / pause menu)
    public void Close()
    {
        Hide();
        onClose?.Invoke();
    }

    // Hide without telling anyone (used by PauseMenu when it resumes)
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    void ShowTab(bool settings)
    {
        onSettingsTab = settings;
        if (settingsPage != null) settingsPage.SetActive(settings);
        if (howToPage != null) howToPage.SetActive(!settings);
        if (settingsTab != null) settingsTab.sprite = settings ? tabActive : tabInactive;
        if (howToTab != null) howToTab.sprite = settings ? tabInactive : tabActive;

        SetupNavigation();

        // Pick what the controller starts on: first slider on SETTINGS, the main button on HOW TO PLAY
        GameObject first = settings && sfxSlider != null ? sfxSlider.gameObject : MainButton()?.gameObject;
        Select(first);
    }

    Button MainButton()
    {
        return mode == Mode.Pause ? resumeButton : backButton;
    }

    // Wire D-pad up/down/left/right between the sliders and buttons so focus never leaves the panel
    void SetupNavigation()
    {
        Button main = MainButton();
        Selectable lastSlider = renderSlider;

        SetNav(sfxSlider, null, musicSlider, null, null);
        SetNav(musicSlider, sfxSlider, renderSlider, null, null);
        SetNav(renderSlider, musicSlider, main, null, null);

        // On HOW TO PLAY the sliders are hidden, so buttons don't go up anywhere
        Selectable up = onSettingsTab ? lastSlider : null;

        if (mode == Mode.Pause && onSettingsTab)
        {
            SetNav(resumeButton, up, null, null, quitButton);
            SetNav(quitButton, up, null, resumeButton, null);
        }
        else if (mode == Mode.Pause)
        {
            // On HOW TO PLAY, left/right flips pages, so RESUME <-> QUIT uses up/down instead
            SetNav(resumeButton, null, quitButton, null, null);
            SetNav(quitButton, resumeButton, null, null, null);
        }
        else
        {
            SetNav(backButton, up, null, null, null);
        }
    }

    static void SetNav(Selectable s, Selectable up, Selectable down, Selectable left, Selectable right)
    {
        if (s == null) return;
        Navigation nav = new Navigation
        {
            mode = Navigation.Mode.Explicit,
            selectOnUp = up,
            selectOnDown = down,
            selectOnLeft = left,
            selectOnRight = right
        };
        s.navigation = nav;
    }

    static void Select(GameObject go)
    {
        if (EventSystem.current == null) return;
        EventSystem.current.SetSelectedGameObject(null);
        if (go != null) EventSystem.current.SetSelectedGameObject(go);
    }

    void Update()
    {
        Gamepad pad = Gamepad.current;
        if (pad == null) return;

        // LB / RB switch tabs
        if (pad.leftShoulder.wasPressedThisFrame && !onSettingsTab) ShowTab(true);
        if (pad.rightShoulder.wasPressedThisFrame && onSettingsTab) ShowTab(false);

        // B goes back (start menu) or resumes (pause menu)
        if (pad.buttonEast.wasPressedThisFrame)
        {
            Close();
            return;
        }

        // D-pad / left stick left-right flips How to Play pages
        if (!onSettingsTab)
        {
            if (pad.dpad.left.wasPressedThisFrame || pad.leftStick.left.wasPressedThisFrame) ShowPage(page - 1);
            if (pad.dpad.right.wasPressedThisFrame || pad.leftStick.right.wasPressedThisFrame) ShowPage(page + 1);
        }

        // If something knocked the selection off (e.g. a mouse click), put it back
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == null)
            Select(onSettingsTab && sfxSlider != null ? sfxSlider.gameObject : MainButton()?.gameObject);
    }

    void ShowPage(int index)
    {
        if (pages == null || pages.Length == 0) return;
        page = Mathf.Clamp(index, 0, pages.Length - 1);

        for (int i = 0; i < pages.Length; i++)
            if (pages[i] != null) pages[i].SetActive(i == page);

        if (dots != null)
            for (int i = 0; i < dots.Length; i++)
                if (dots[i] != null)
                {
                    dots[i].sprite = i == page ? dotActive : dotInactive;
                    dots[i].SetNativeSize(); // active dot is a wider pill
                }

        // Grey out the arrow at each end
        if (prevArrow != null) prevArrow.sprite = page == 0 ? arrowPrevDisabled : arrowPrev;
        if (nextArrow != null) nextArrow.sprite = page == pages.Length - 1 ? arrowNextDisabled : arrowNext;
    }
}
