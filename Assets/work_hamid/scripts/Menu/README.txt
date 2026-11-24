WHEN NATURE CALLS - MENU KIT
Copy this "Menu" folder into:  Assets/work_hamid/scripts/   (so you get Assets/work_hamid/scripts/Menu)

Then in Unity:  Tools > When Nature Calls
  1. Build Menus                    - makes OptionsPanel + PauseMenu prefabs (work_hamid/UI/Prefabs)
                                      and a new scene work_hamid/scenes/MainMenu_Full
  2. Add Menu Scenes To Build List  - MainMenu_Full (0), BetaTest_Base (1)
  3. Add Pause Menu To Open Scene   - open BetaTest_Base first, run this, then Ctrl+S

Scripts
  GameSettings        saves Sound FX / Music / Render Distance and applies them
  OptionsPanel        tabs, sliders, How to Play pages, Back/Resume/Quit
  MainMenuController  start screen buttons (Game Scene Name field = scene PLAY loads)
  PauseMenu           Menu button pauses; disables InputManager while paused
  RowHighlight        gold row when a slider is selected
  MusicVolume         add to any music AudioSource so the MUSIC slider controls it
  Editor/MenuBuilder  the Tools menu above (editor only)
