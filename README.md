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
  * Tab menu:
    * Stats
    * Quests (partially)
  * Quests
  * Dialogues
* Translated text can have special text edit tags:
  * `<firstupper>` - the first letter of the string inside this tag will be changed to the uppercase variant
    * Example: `slime <firstupper>diva</firstupper>` will be changed into `slime Diva`
  * `<firstlower>` - the first letter of the string inside this tag will be changed to the lowercase variant
    * Example: `Slime <firstlower>Diva</firstlower>` will be changed into `Slime diva`
  * `<scale=1.0>` - text in this tag will have its scale multiplied by the amount provided in the argument
    * Example: `Slime <scale=0.8>Diva</scale>` will resize the word "Diva" to 80% of its original size
