using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Editor-only tool that builds the whole menu system from the sprites + fonts in work_hamid/UI.
// Menu: Tools > When Nature Calls
//   1. Build Menus                      -> OptionsPanel + PauseMenu prefabs and the MainMenu_Full scene
//   2. Add Menu Scenes To Build List    -> MainMenu_Full (0) and BetaTest_Base (1)
//   3. Add Pause Menu To Open Scene     -> drops the PauseMenu prefab into whatever scene is open
// Safe to run again: it overwrites the two prefabs and MainMenu_Full, and never touches your other scenes.
public static class MenuBuilder
{
    const string UiDir = "Assets/work_hamid/UI";
    const string MenuDir = UiDir + "/Menu";
    const string HudDir = UiDir + "/HUDs";
    const string FontDir = UiDir + "/Fonts";
    const string PrefabDir = UiDir + "/Prefabs";
    const string SceneDir = "Assets/work_hamid/scenes";
    const string OptionsPrefabPath = PrefabDir + "/OptionsPanel.prefab";
    const string PausePrefabPath = PrefabDir + "/PauseMenu.prefab";
    const string MenuScenePath = SceneDir + "/MainMenu_Full.unity";
    const string GameScenePath = SceneDir + "/BetaTest_Base.unity";

    // Colours from the design
    static readonly Color Dark = new Color32(0x2B, 0x21, 0x18, 255);
    static readonly Color Cream = new Color32(0xFF, 0xF6, 0xE5, 255);
    static readonly Color Gold = new Color32(0xF5, 0xB8, 0x2E, 255);
    static readonly Color Muted = new Color32(0x5C, 0x4A, 0x3A, 255);

    // Anchor shortcuts
    static readonly Vector2 TL = new Vector2(0f, 1f), TC = new Vector2(0.5f, 1f), TR = new Vector2(1f, 1f);
    static readonly Vector2 ML = new Vector2(0f, 0.5f), MC = new Vector2(0.5f, 0.5f), MR = new Vector2(1f, 0.5f);
    static readonly Vector2 BL = new Vector2(0f, 0f), BC = new Vector2(0.5f, 0f);

    static TMP_FontAsset lilita, nunito;

    // Slice borders (Left, Bottom, Right, Top) so stretched sprites keep their round corners
    static readonly Dictionary<string, Vector4> Borders = new Dictionary<string, Vector4>
    {
        { "button_cream", new Vector4(48, 52, 48, 48) },
        { "button_cream_focus", new Vector4(48, 52, 48, 48) },
        { "button_green", new Vector4(48, 52, 48, 48) },
        { "button_green_focus", new Vector4(48, 52, 48, 48) },
        { "button_grey", new Vector4(48, 52, 48, 48) },
        { "panel_menu", new Vector4(48, 58, 48, 48) },
        { "card_white", new Vector4(36, 36, 36, 36) },
        { "row_card", new Vector4(40, 40, 40, 40) },
        { "row_card_focus", new Vector4(40, 40, 40, 40) },
        { "tab_active", new Vector4(34, 32, 34, 32) },
        { "tab_inactive", new Vector4(34, 32, 34, 32) },
        { "slider_track", new Vector4(20, 18, 20, 18) },
        { "slider_fill_green", new Vector4(14, 14, 14, 14) },
        { "slider_fill_blue", new Vector4(14, 14, 14, 14) },
        { "keycap", new Vector4(12, 12, 12, 12) },
    };

    // ------------------------------------------------------------------ MENU ITEMS

    [MenuItem("Tools/When Nature Calls/1. Build Menus")]
    public static void BuildAll()
    {
        // Building the menu scene replaces the open scene, so offer to save first
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        if (!LoadFonts()) return;

        FixSprites();
        if (!AssetDatabase.IsValidFolder(PrefabDir)) AssetDatabase.CreateFolder(UiDir, "Prefabs");

        GameObject optionsPrefab = BuildOptionsPrefab();
        BuildPausePrefab(optionsPrefab);
        BuildMainMenuScene(optionsPrefab);

        AssetDatabase.SaveAssets();
        Debug.Log("[MenuBuilder] Done: " + OptionsPrefabPath + ", " + PausePrefabPath + ", " + MenuScenePath);
    }

    [MenuItem("Tools/When Nature Calls/2. Add Menu Scenes To Build List")]
    public static void AddScenesToBuild()
    {
        // Start menu first (0), game second (1); keeps any other scenes already in the list after them
        var list = new List<EditorBuildSettingsScene>
        {
            new EditorBuildSettingsScene(MenuScenePath, true),
            new EditorBuildSettingsScene(GameScenePath, true)
        };
        foreach (EditorBuildSettingsScene s in EditorBuildSettings.scenes)
            if (s.path != MenuScenePath && s.path != GameScenePath) list.Add(s);
        EditorBuildSettings.scenes = list.ToArray();
        Debug.Log("[MenuBuilder] Build list: MainMenu_Full (0), BetaTest_Base (1)");
    }

    [MenuItem("Tools/When Nature Calls/3. Add Pause Menu To Open Scene")]
    public static void AddPauseToOpenScene()
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PausePrefabPath);
        if (prefab == null) { Debug.LogError("[MenuBuilder] Run '1. Build Menus' first."); return; }
        if (Object.FindAnyObjectByType<PauseMenu>(FindObjectsInactive.Include) != null)
        {
            Debug.Log("[MenuBuilder] This scene already has a PauseMenu.");
            return;
        }
        Scene scene = SceneManager.GetActiveScene();
        PrefabUtility.InstantiatePrefab(prefab, scene);
        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("[MenuBuilder] PauseMenu added to " + scene.name + ". Save the scene (Ctrl+S).");
    }

    // ------------------------------------------------------------------ SETUP

    static bool LoadFonts()
    {
        lilita = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontDir + "/LilitaOne-Regular SDF.asset");
        nunito = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontDir + "/Nunito-ExtraBold SDF.asset");
        if (lilita == null || nunito == null)
        {
            Debug.LogError("[MenuBuilder] Missing font assets in " + FontDir + " (need 'LilitaOne-Regular SDF' and 'Nunito-ExtraBold SDF').");
            return false;
        }
        return true;
    }

    // Makes sure every menu/HUD image is a Sprite, and sets slice borders where needed
    static void FixSprites()
    {
        foreach (string dir in new[] { MenuDir, HudDir })
        {
            if (!AssetDatabase.IsValidFolder(dir)) continue;
            foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { dir }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                if (ti == null) continue;

                bool changed = false;
                if (ti.textureType != TextureImporterType.Sprite) { ti.textureType = TextureImporterType.Sprite; changed = true; }
                if (ti.spriteImportMode != SpriteImportMode.Single) { ti.spriteImportMode = SpriteImportMode.Single; changed = true; }

                string name = Path.GetFileNameWithoutExtension(path);
                if (Borders.TryGetValue(name, out Vector4 border) && ti.spriteBorder != border)
                {
                    ti.spriteBorder = border;
                    var settings = new TextureImporterSettings();
                    ti.ReadTextureSettings(settings);
                    settings.spriteMeshType = SpriteMeshType.FullRect; // needed for clean slicing
                    ti.SetTextureSettings(settings);
                    changed = true;
                }
                if (changed) ti.SaveAndReimport();
            }
        }
    }

    // ------------------------------------------------------------------ OPTIONS PANEL PREFAB

    static GameObject BuildOptionsPrefab()
    {
        RectTransform root = Node("OptionsPanel", null);
        Stretch(root);
        OptionsPanel op = root.gameObject.AddComponent<OptionsPanel>();

        // Dark see-through backdrop over whatever is behind (also blocks clicks)
        Image dim = Img(root, "Dim", null, false, true);
        dim.color = new Color(Dark.r, Dark.g, Dark.b, 170f / 255f);
        Stretch(dim.rectTransform);

        // Big cream panel
        Image panel = Img(root, "Panel", S("panel_menu"), true, true);
        Place(panel.rectTransform, MC, MC, new Vector2(0, 20), new Vector2(1200, 850));
        Transform p = panel.transform;

        // Title (OPTIONS / PAUSED)
        TextMeshProUGUI title = Txt(p, "Title", "OPTIONS", lilita, 64, Dark, TextAlignmentOptions.Left);
        Place(title.rectTransform, TL, TL, new Vector2(48, -36), new Vector2(420, 70));

        // Tabs row: LB [SETTINGS] [HOW TO PLAY] RB
        RectTransform tabs = Node("Tabs", p);
        Place(tabs, TR, TR, new Vector2(-48, -40), new Vector2(660, 64));
        HorizontalLayoutGroup tabLayout = tabs.gameObject.AddComponent<HorizontalLayoutGroup>();
        tabLayout.spacing = 12;
        tabLayout.childAlignment = TextAnchor.MiddleRight;
        tabLayout.childControlWidth = tabLayout.childControlHeight = false;
        tabLayout.childForceExpandWidth = tabLayout.childForceExpandHeight = false;

        NativeImg(tabs, "GlyphLB", S("glyph_LB"));
        Image settingsTab = Tab(tabs, "SettingsTab", "SETTINGS", S("tab_active"), 210);
        Image howToTab = Tab(tabs, "HowToPlayTab", "HOW TO PLAY", S("tab_inactive"), 250);
        NativeImg(tabs, "GlyphRB", S("glyph_RB"));

        // Line under the header
        Image divider = Img(p, "Divider", null);
        divider.color = Dark;
        Place(divider.rectTransform, TC, TC, new Vector2(0, -130), new Vector2(1104, 4));

        // ---------- SETTINGS page
        RectTransform settingsPage = Node("SettingsPage", p);
        Place(settingsPage, TC, TC, new Vector2(0, -160), new Vector2(1116, 384));
        VerticalLayoutGroup rows = settingsPage.gameObject.AddComponent<VerticalLayoutGroup>();
        rows.spacing = 12;
        rows.childControlWidth = rows.childControlHeight = true;
        rows.childForceExpandWidth = true;
        rows.childForceExpandHeight = false;

        Slider sfx = Row(settingsPage, "SfxRow", S("icon_sound"), "SOUND FX", null, S("slider_fill_green"), out TextMeshProUGUI sfxValue);
        Slider music = Row(settingsPage, "MusicRow", S("icon_music"), "MUSIC", null, S("slider_fill_green"), out TextMeshProUGUI musicValue);
        Slider render = Row(settingsPage, "RenderRow", S("icon_render"), "RENDER DISTANCE", "Lower = smoother on slow PCs", S("slider_fill_blue"), out TextMeshProUGUI renderValue);

        // ---------- HOW TO PLAY page
        RectTransform howTo = Node("HowToPlayPage", p);
        Place(howTo, TC, TC, new Vector2(0, -150), new Vector2(1116, 530));

        Image card = Img(howTo, "Card", S("card_white"), true);
        Place(card.rectTransform, TC, TC, new Vector2(0, 0), new Vector2(920, 460));

        Image prev = NativeImg(howTo, "PrevArrow", S("arrow_prev_disabled"));
        Place(prev.rectTransform, ML, ML, new Vector2(8, 35), prev.rectTransform.sizeDelta);
        Image next = NativeImg(howTo, "NextArrow", S("arrow_next"));
        Place(next.rectTransform, MR, MR, new Vector2(-8, 35), next.rectTransform.sizeDelta);

        var pages = new List<GameObject>
        {
            ControlsPage(card.transform),
            TipPage(card.transform, 2, "FLY", S("menu_background"), new Color32(0xBD, 0xE8, 0xF5, 255),
                "Flap to climb and glide to save energy. Boost for speed, and nose dive to drop fast onto a target.",
                new[] { (S("glyph_LS"), "Steer"), (S("glyph_RT"), "Tap to flap"), (S("glyph_RT"), "Hold to boost"), (S("glyph_LT"), "Nose dive") }),
            TipPage(card.transform, 3, "FILL YOUR STOMACH", S("tip_stomach"), new Color32(0xBD, 0xE8, 0xF5, 255),
                "Fly into food around the city to eat it. The green in your stomach is your poop ammo, and a full stomach can't eat any more.",
                new[] { (S("glyph_LS"), "Fly into food: eating is automatic") }),
            TipPage(card.transform, 4, "POOP MODE", S("poopmode_reticle"), new Color32(0x3A, 0x3F, 0x4A, 255),
                "While flying, press RB for poop mode: the camera looks straight down. Line up the target and press X to drop. RB again to go back.",
                new[] { (S("glyph_RB"), "Poop mode on / off"), (S("glyph_X"), "Drop poop"), (S("glyph_LS"), "Steer the target") }),
            TipPage(card.transform, 5, "EARN CASH", S("icon_piggybank"), new Color32(0xFF, 0xE9, 0xA8, 255),
                "Every poop that lands on a pedestrian puts $100 in your piggybank. Moving targets are harder to hit.",
                new[] { (S("glyph_X"), "Drop poop in poop mode") }),
            TipPage(card.transform, 6, "WATCH YOUR ALTITUDE", S("tip_altitude"), new Color32(0xFF, 0xF6, 0xE5, 255),
                "Keep the arrow in the blue. The red ends of the altitude bar are the height limits: fly past them and it's game over.",
                new[] { (S("glyph_RT"), "Tap to climb"), (S("glyph_LT"), "Nose dive to drop") }),
        };

        // Page dots
        RectTransform dotsRow = Node("Dots", howTo);
        Place(dotsRow, TC, TC, new Vector2(0, -490), new Vector2(320, 24));
        HorizontalLayoutGroup dotLayout = dotsRow.gameObject.AddComponent<HorizontalLayoutGroup>();
        dotLayout.spacing = 12;
        dotLayout.childAlignment = TextAnchor.MiddleCenter;
        dotLayout.childControlWidth = dotLayout.childControlHeight = false;
        dotLayout.childForceExpandWidth = dotLayout.childForceExpandHeight = false;
        var dots = new List<Image>();
        for (int i = 0; i < pages.Count; i++)
            dots.Add(NativeImg(dotsRow, "Dot" + (i + 1), i == 0 ? S("dot_active") : S("dot_inactive")));

        // ---------- Bottom buttons
        RectTransform startButtons = Node("StartButtons", p);
        Place(startButtons, BC, BC, new Vector2(0, 40), new Vector2(1104, 100));
        Button back = Btn(startButtons, "BackButton", "BACK TO START", 420, 100, S("glyph_B"));
        Place((RectTransform)back.transform, MC, MC, Vector2.zero, new Vector2(420, 100));

        RectTransform pauseButtons = Node("PauseButtons", p);
        Place(pauseButtons, BC, BC, new Vector2(0, 40), new Vector2(1104, 100));
        Button resume = Btn(pauseButtons, "ResumeButton", "RESUME", 380, 100, S("glyph_A"));
        Place((RectTransform)resume.transform, MC, MC, new Vector2(-200, 0), new Vector2(380, 100));
        Button quit = Btn(pauseButtons, "QuitButton", "QUIT TO MENU", 380, 100, null);
        Place((RectTransform)quit.transform, MC, MC, new Vector2(200, 0), new Vector2(380, 100));
        pauseButtons.gameObject.SetActive(false);

        // Controller hint under the panel
        TextMeshProUGUI hint = Txt(root, "Hint", "D-pad  Choose / Adjust          LB / RB  Switch tab          B  Back", nunito, 22, Cream, TextAlignmentOptions.Center);
        Place(hint.rectTransform, BC, BC, new Vector2(0, 28), new Vector2(1400, 40));

        // ---------- Wire the script
        op.title = title;
        op.settingsTab = settingsTab;
        op.howToTab = howToTab;
        op.tabActive = S("tab_active");
        op.tabInactive = S("tab_inactive");
        op.settingsPage = settingsPage.gameObject;
        op.howToPage = howTo.gameObject;
        op.sfxSlider = sfx; op.musicSlider = music; op.renderSlider = render;
        op.sfxValue = sfxValue; op.musicValue = musicValue; op.renderValue = renderValue;
        op.pages = pages.ToArray();
        op.dots = dots.ToArray();
        op.dotActive = S("dot_active"); op.dotInactive = S("dot_inactive");
        op.prevArrow = prev; op.nextArrow = next;
        op.arrowPrev = S("arrow_prev"); op.arrowPrevDisabled = S("arrow_prev_disabled");
        op.arrowNext = S("arrow_next"); op.arrowNextDisabled = S("arrow_next_disabled");
        op.startButtons = startButtons.gameObject;
        op.pauseButtons = pauseButtons.gameObject;
        op.backButton = back; op.resumeButton = resume; op.quitButton = quit;

        // Show only page 1 and the SETTINGS tab in the saved prefab
        for (int i = 1; i < pages.Count; i++) pages[i].SetActive(false);
        howTo.gameObject.SetActive(false);

        GameObject saved = PrefabUtility.SaveAsPrefabAsset(root.gameObject, OptionsPrefabPath);
        Object.DestroyImmediate(root.gameObject);
        return saved;
    }

    // Page 1: every control in two columns
    static GameObject ControlsPage(Transform card)
    {
        RectTransform page = Node("Page1_Controls", card);
        Stretch(page);
        PageHeader(page, 1, "CONTROLS", 36, 640);

        var entries = new[]
        {
            (S("glyph_LS"), "Steer / walk", "Left stick"),
            (S("glyph_RS"), "Look around", "Right stick"),
            (S("glyph_RT"), "Flap wings", "Tap RT"),
            (S("glyph_RT"), "Boost", "Hold RT"),
            (S("glyph_LT"), "Nose dive", "Press LT"),
            (S("glyph_RB"), "Poop mode", "RB while flying"),
            (S("glyph_X"), "Drop poop", "X in poop mode"),
            (S("glyph_Menu"), "Pause", "Menu button"),
        };

        for (int i = 0; i < entries.Length; i++)
        {
            float x = i < 4 ? 40 : 480;
            float y = -120 - (i % 4) * 82;
            Image g = NativeImg(page, "Glyph" + (i + 1), entries[i].Item1);
            Place(g.rectTransform, TL, ML, new Vector2(x, y), g.rectTransform.sizeDelta);

            TextMeshProUGUI action = Txt(page, "Action" + (i + 1), entries[i].Item2, lilita, 30, Dark, TextAlignmentOptions.Left);
            Place(action.rectTransform, TL, ML, new Vector2(x + 84, y + 14), new Vector2(330, 36));
            TextMeshProUGUI how = Txt(page, "How" + (i + 1), entries[i].Item3, nunito, 20, Muted, TextAlignmentOptions.Left);
            Place(how.rectTransform, TL, ML, new Vector2(x + 84, y - 18), new Vector2(330, 28));
        }
        return page.gameObject;
    }

    // Pages 2-6: picture, title, description, and the controls for that tip
    static GameObject TipPage(Transform card, int number, string titleText, Sprite picture, Color pictureBg, string body, (Sprite, string)[] controls)
    {
        RectTransform page = Node("Page" + number, card);
        Stretch(page);

        Image picBg = Img(page, "PictureBackground", null);
        picBg.color = pictureBg;
        Place(picBg.rectTransform, TL, TL, new Vector2(32, -32), new Vector2(360, 250));
        Image pic = Img(picBg.transform, "Picture", picture);
        pic.preserveAspect = true;
        Stretch(pic.rectTransform, 14, 14, 14, 14);

        PageHeader(page, number, titleText, 420, 470);

        TextMeshProUGUI desc = Txt(page, "Body", body, nunito, 22, Dark, TextAlignmentOptions.TopLeft);
        Place(desc.rectTransform, TL, TL, new Vector2(420, -150), new Vector2(470, 140));

        Image line = Img(page, "ControlsLine", null);
        line.color = Muted;
        Place(line.rectTransform, TL, TL, new Vector2(32, -306), new Vector2(856, 3));
        TextMeshProUGUI label = Txt(page, "ControlsLabel", "CONTROLS", nunito, 16, Muted, TextAlignmentOptions.Left);
        Place(label.rectTransform, TL, TL, new Vector2(32, -314), new Vector2(200, 24));

        for (int i = 0; i < controls.Length; i++)
        {
            float x = i % 2 == 0 ? 32 : 470;
            float y = -372 - (i / 2) * 56;
            Image g = NativeImg(page, "Glyph" + (i + 1), controls[i].Item1);
            Place(g.rectTransform, TL, ML, new Vector2(x, y), g.rectTransform.sizeDelta);
            TextMeshProUGUI t = Txt(page, "Control" + (i + 1), controls[i].Item2, lilita, 26, Dark, TextAlignmentOptions.Left);
            Place(t.rectTransform, TL, ML, new Vector2(x + 80, y), new Vector2(340, 40));
        }
        return page.gameObject;
    }

    // "PAGE n OF 6" pill + big title
    static void PageHeader(Transform page, int number, string titleText, float x, float titleWidth)
    {
        Image tag = Img(page, "Tag", null);
        tag.color = Dark;
        Place(tag.rectTransform, TL, TL, number == 1 ? new Vector2(260, -40) : new Vector2(x, -40), new Vector2(170, 34));
        TextMeshProUGUI tagText = Txt(tag.transform, "Text", "PAGE " + number + " OF 6", nunito, 16, Cream, TextAlignmentOptions.Center);
        Stretch(tagText.rectTransform);

        TextMeshProUGUI title = Txt(page, "Title", titleText, lilita, 48, Dark, TextAlignmentOptions.Left);
        title.enableAutoSizing = true;
        title.fontSizeMin = 28;
        title.fontSizeMax = 48;
        if (number == 1) Place(title.rectTransform, TL, TL, new Vector2(36, -28), new Vector2(210, 60));
        else Place(title.rectTransform, TL, TL, new Vector2(x, -82), new Vector2(titleWidth, 60));
    }

    // One settings row: icon, label, slider, value
    static Slider Row(Transform parent, string name, Sprite icon, string label, string subLabel, Sprite fill, out TextMeshProUGUI value)
    {
        Image row = Img(parent, name, S("row_card"), true);
        row.gameObject.AddComponent<LayoutElement>().preferredHeight = 120;
        Transform r = row.transform;

        Image ic = Img(r, "Icon", icon);
        Place(ic.rectTransform, ML, ML, new Vector2(40, 0), new Vector2(56, 56));

        TextMeshProUGUI lbl = Txt(r, "Label", label, lilita, 34, Dark, TextAlignmentOptions.Left);
        lbl.textWrappingMode = TextWrappingModes.NoWrap;   // one line; shrink long labels to fit
        lbl.enableAutoSizing = true;
        lbl.fontSizeMin = 24;
        lbl.fontSizeMax = 34;
        Place(lbl.rectTransform, ML, ML, new Vector2(116, subLabel != null ? 12 : 0), new Vector2(310, 50));
        if (subLabel != null)
        {
            TextMeshProUGUI sub = Txt(r, "SubLabel", subLabel, nunito, 18, Muted, TextAlignmentOptions.Left);
            Place(sub.rectTransform, ML, ML, new Vector2(118, -24), new Vector2(310, 28));
        }

        Slider slider = BuildSlider(r, "Slider", fill);
        Place((RectTransform)slider.transform, ML, ML, new Vector2(440, 0), new Vector2(440, 36));

        value = Txt(r, "Value", "80%", lilita, 40, Dark, TextAlignmentOptions.Right);
        Place(value.rectTransform, MR, MR, new Vector2(-40, 0), new Vector2(140, 60));

        RowHighlight highlight = slider.gameObject.AddComponent<RowHighlight>();
        highlight.row = row;
        highlight.normal = S("row_card");
        highlight.focused = S("row_card_focus");
        return slider;
    }

    static Slider BuildSlider(Transform parent, string name, Sprite fill)
    {
        RectTransform rt = Node(name, parent);
        Slider slider = rt.gameObject.AddComponent<Slider>();

        Image bg = Img(rt, "Background", S("slider_track"), true);
        Stretch(bg.rectTransform);

        RectTransform fillArea = Node("Fill Area", rt);
        Stretch(fillArea, 4, 4, 4, 4);
        Image fillImg = Img(fillArea, "Fill", fill, true);
        RectTransform fillRt = fillImg.rectTransform;
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = new Vector2(0f, 1f);
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;

        RectTransform handleArea = Node("Handle Slide Area", rt);
        Stretch(handleArea);
        Image handle = Img(handleArea, "Handle", S("slider_knob"), false, true);
        RectTransform handleRt = handle.rectTransform;
        handleRt.anchorMin = Vector2.zero;
        handleRt.anchorMax = new Vector2(0f, 1f);
        handleRt.sizeDelta = new Vector2(52, 16); // 36 tall track + 16 = 52 px knob

        slider.fillRect = fillRt;
        slider.handleRect = handleRt;
        slider.targetGraphic = handle;
        slider.direction = Slider.Direction.LeftToRight;
        slider.transition = Selectable.Transition.None;
        slider.minValue = 0;
        slider.maxValue = 10;
        slider.wholeNumbers = true;
        slider.value = 8;
        return slider;
    }

    static Image Tab(Transform parent, string name, string label, Sprite sprite, float width)
    {
        Image tab = Img(parent, name, sprite, true);
        tab.rectTransform.sizeDelta = new Vector2(width, 64);
        TextMeshProUGUI t = Txt(tab.transform, "Text", label, lilita, 28, Dark, TextAlignmentOptions.Center);
        Stretch(t.rectTransform);
        return tab;
    }

    // ------------------------------------------------------------------ PAUSE MENU PREFAB

    static void BuildPausePrefab(GameObject optionsPrefab)
    {
        GameObject root = NewCanvas("PauseMenu", 100);
        PauseMenu pause = root.AddComponent<PauseMenu>();

        GameObject options = (GameObject)PrefabUtility.InstantiatePrefab(optionsPrefab, root.transform);
        Stretch((RectTransform)options.transform);
        options.SetActive(false);
        pause.options = options.GetComponent<OptionsPanel>();

        PrefabUtility.SaveAsPrefabAsset(root, PausePrefabPath);
        Object.DestroyImmediate(root);
    }

    // ------------------------------------------------------------------ START MENU SCENE

    static void BuildMainMenuScene(GameObject optionsPrefab)
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Camera (just a dark background behind the UI)
        GameObject camGo = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        camGo.tag = "MainCamera";
        Camera cam = camGo.GetComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Dark;

        // EventSystem that listens to the controller
        GameObject es = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
        es.GetComponent<InputSystemUIInputModule>().deselectOnBackgroundClick = false;

        GameObject canvas = NewCanvas("Canvas", 0);
        Transform c = canvas.transform;

        Image bg = Img(c, "Background", S("menu_background"));
        Stretch(bg.rectTransform);
        Image fade = Img(c, "Fade", S("menu_fade"));
        Stretch(fade.rectTransform);

        Image logo = NativeImg(c, "Logo", S("logo_title"));
        Place(logo.rectTransform, TL, TL, new Vector2(140, -100), logo.rectTransform.sizeDelta);

        TextMeshProUGUI tagline = Txt(c, "Tagline", "Fly. Eat. Aim. Poop.", nunito, 30, Gold, TextAlignmentOptions.Left);
        Place(tagline.rectTransform, TL, TL, new Vector2(146, -390), new Vector2(600, 44));

        // PLAY / HOW TO PLAY / SETTINGS
        RectTransform buttons = Node("MenuButtons", c);
        Place(buttons, ML, ML, new Vector2(140, -110), new Vector2(534, 332));
        VerticalLayoutGroup v = buttons.gameObject.AddComponent<VerticalLayoutGroup>();
        v.spacing = 16;
        v.childControlWidth = v.childControlHeight = true;
        v.childForceExpandWidth = true;
        v.childForceExpandHeight = false;
        Button play = Btn(buttons, "Play", "PLAY", 534, 100, null);
        Button howTo = Btn(buttons, "HowToPlay", "HOW TO PLAY", 534, 100, null);
        Button settings = Btn(buttons, "Settings", "SETTINGS", 534, 100, null);
        foreach (Button b in new[] { play, howTo, settings })
            b.gameObject.AddComponent<LayoutElement>().preferredHeight = 100;

        // Controller hint: D-pad Choose  (A) Select
        RectTransform hint = Node("Hint", c);
        Place(hint, BL, BL, new Vector2(140, 40), new Vector2(600, 56));
        HorizontalLayoutGroup h = hint.gameObject.AddComponent<HorizontalLayoutGroup>();
        h.spacing = 16;
        h.childAlignment = TextAnchor.MiddleLeft;
        h.childControlWidth = h.childControlHeight = false;
        h.childForceExpandWidth = h.childForceExpandHeight = false;
        TextMeshProUGUI dpad = Txt(hint, "DpadText", "D-pad  Choose", nunito, 24, Cream, TextAlignmentOptions.Left);
        dpad.rectTransform.sizeDelta = new Vector2(190, 56);
        Image a = Img(hint, "GlyphA", S("glyph_A"));
        a.rectTransform.sizeDelta = new Vector2(40, 40);
        TextMeshProUGUI select = Txt(hint, "SelectText", "Select", nunito, 24, Cream, TextAlignmentOptions.Left);
        select.rectTransform.sizeDelta = new Vector2(120, 56);

        // Shared Options panel (hidden until HOW TO PLAY / SETTINGS is pressed)
        GameObject options = (GameObject)PrefabUtility.InstantiatePrefab(optionsPrefab, c);
        Stretch((RectTransform)options.transform);
        options.SetActive(false);

        // Script that runs the start screen
        MainMenuController menu = canvas.AddComponent<MainMenuController>();
        menu.gameSceneName = "BetaTest_Base";
        menu.playButton = play;
        menu.howToPlayButton = howTo;
        menu.settingsButton = settings;
        menu.options = options.GetComponent<OptionsPanel>();

        es.GetComponent<EventSystem>().firstSelectedGameObject = play.gameObject;

        EditorSceneManager.SaveScene(scene, MenuScenePath);
    }

    // ------------------------------------------------------------------ HELPERS

    static Sprite S(string name)
    {
        foreach (string path in new[] { MenuDir + "/" + name + ".png", MenuDir + "/" + name + ".jpg", HudDir + "/" + name + ".png" })
        {
            Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (s != null) return s;
        }
        Debug.LogWarning("[MenuBuilder] Sprite not found: " + name);
        return null;
    }

    static GameObject NewCanvas(string name, int sortOrder)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        go.layer = LayerMask.NameToLayer("UI");
        Canvas canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortOrder;
        CanvasScaler scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        return go;
    }

    static RectTransform Node(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = LayerMask.NameToLayer("UI");
        RectTransform rt = (RectTransform)go.transform;
        if (parent != null) rt.SetParent(parent, false);
        return rt;
    }

    static void Place(RectTransform rt, Vector2 anchor, Vector2 pivot, Vector2 position, Vector2 size)
    {
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = pivot;
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
    }

    static void Stretch(RectTransform rt, float left = 0, float top = 0, float right = 0, float bottom = 0)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = new Vector2(left, bottom);
        rt.offsetMax = new Vector2(-right, -top);
    }

    static Image Img(Transform parent, string name, Sprite sprite, bool sliced = false, bool raycast = false)
    {
        RectTransform rt = Node(name, parent);
        Image img = rt.gameObject.AddComponent<Image>();
        img.sprite = sprite;
        img.type = sliced ? Image.Type.Sliced : Image.Type.Simple;
        img.raycastTarget = raycast;
        return img;
    }

    // Image at the sprite's own pixel size (glyphs, arrows, dots, logo)
    static Image NativeImg(Transform parent, string name, Sprite sprite)
    {
        Image img = Img(parent, name, sprite);
        if (sprite != null) img.SetNativeSize();
        return img;
    }

    static TextMeshProUGUI Txt(Transform parent, string name, string text, TMP_FontAsset font, float size, Color color, TextAlignmentOptions align)
    {
        RectTransform rt = Node(name, parent);
        TextMeshProUGUI t = rt.gameObject.AddComponent<TextMeshProUGUI>();
        t.font = font;
        t.text = text;
        t.fontSize = size;
        t.color = color;
        t.alignment = align;
        t.fontStyle = FontStyles.Normal;
        t.textWrappingMode = TextWrappingModes.Normal;
        t.raycastTarget = false;
        return t;
    }

    // Cream pill button that turns green when selected by the controller
    static Button Btn(Transform parent, string name, string label, float width, float height, Sprite glyph)
    {
        Image img = Img(parent, name, S("button_cream"), true, true);
        img.rectTransform.sizeDelta = new Vector2(width, height);

        Button b = img.gameObject.AddComponent<Button>();
        b.targetGraphic = img;
        b.transition = Selectable.Transition.SpriteSwap;
        b.spriteState = new SpriteState
        {
            highlightedSprite = S("button_green"),
            selectedSprite = S("button_green"),
            pressedSprite = S("button_green_focus"),
            disabledSprite = S("button_grey")
        };

        TextMeshProUGUI t = Txt(img.transform, "Text", label, lilita, 40, Dark, TextAlignmentOptions.Center);
        Stretch(t.rectTransform);
        // The pill art sits 3px high (its shadow is underneath), so nudge text up a little
        t.margin = new Vector4(glyph != null ? 56 : 0, 0, 0, 6);

        if (glyph != null)
        {
            Image g = Img(img.transform, "Glyph", glyph);
            Place(g.rectTransform, ML, ML, new Vector2(26, 3), new Vector2(44, 44));
        }
        return b;
    }
}
