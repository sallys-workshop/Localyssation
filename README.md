# Localyssation
A work-in-progress localisation library mod for the video game [ATLYSS](https://store.steampowered.com/app/2768430/ATLYSS/).

# :warning: Important note :warning:
This mod is not finished yet! As such, no public releases are available for now.

## Current features
* New "Language" option in the "Interface" settings tab
* Loading all custom languages found in the BepInEx plugins folder
  * A language consists of the following files:
    * `localyssationLanguage.json` with information about the language
    * `strings.tsv` with text keys and corresponding translated text
* Config options:
  * `Create Default Language Files` - creates the game's default language as files in the mod directory on game load. These files can be copied in a different folder for your own translation.
  * `Reload Language Keybind` - hot-reloads the currently selected language. Useful for seeing immediate changes while writing your own translation.
* Some in-game text gets translated according to the selected language:
  * Main menu
  * Settings
  * Character select menu
  * Character creation menu
  * Equipment tooltips
  * Top-right quest tracker
