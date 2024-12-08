# Localyssation
A work-in-progress localisation library mod for the video game [ATLYSS](https://store.steampowered.com/app/2768430/ATLYSS/).

# :warning: Important note :warning:
This mod is not finished yet! The pre-release versions you see are unfinished versions of the mod intended for translators to make their own languages early.

## Current features
* New "Language" option in the "Interface" settings tab
* Loading all custom languages found in the BepInEx plugins folder
  * A language consists of the following files:
    * `localyssationLanguage.json` with information about the language
    * `strings.tsv` with text keys and corresponding translated text
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

## For translators
### Important
You do not have to ask me if you can make a translation. This mod is a library that lets others easily make their own translations. It is not intended to include translations on its own.
### How to install the mod
1. Download BepInEx and install it (either manually or through a mod manager): https://thunderstore.io/c/atlyss/p/BepInEx/BepInExPack/
2. Launch the game at least once
3. Open the folder where BepInEx is installed, then open the `plugins` folder inside the BepInEx folder
    * If you installed BepInEx manually, the `BepInEx` folder should be in the game's root folder
    * If you installed BepInEx through a mod manager, find an option in the mod manager to "browse local mod files", which should open your File Explorer in the BepInEx folder
4. Download the mod zip from the Releases tab on the right side of this page
5. Unpack everything from the downloaded zip into the `BepInEx/plugins` folder
### How to create a new language
1. Launch the game at least once with the mod installed
2. Open your mod manager's config editor, and select the config called `com.themysticsword.localyssation.cfg`
    * If you installed BepInEx manually without a mod manager, go to `<game root folder>/BepInEx/config` to find the aforementioned config file, and open it in any text editor
3. Change `Create Default Language Files` from `false` to `true`, then save the config
4. Launch the game once, wait for it to load, then close it
5. Change `Create Default Language Files` back to `false`, then save the config
6. Go to where the Localyssation mod files are stored. You should see a new folder called `defaultLanguage`. Move this folder to `BepInEx/plugins` and rename it to any folder name of your choice
7. Open `localyssationLanguage.json` with any text editor and change the following values in such ways:
    * `code` - every language must have a unique code. A good format to follow is the IETF language tag standard (`en-US`, `en-GB`, `es-419`, `ru-RU`, etc.)
    * `name` - the name of the language that will be displayed inside the game
    * `autoShrinkOverflowingText` - set to `true` only if you want the mod to attempt to automatically shrink text if it overflows. Experimental feature, not guaranteed to help all the time.
8. Open `strings.tsv` in a table editor, or in any text editor:
    * The values under the `key` column should NOT be changed
    * The values under the `value` are the translated strings of text that will show up in the game when your language is selected. You can change those.
9. Open the game, go to Settings > Interface, then open the Language dropdown and select your new language
### Translation tips
* You can wrap your translated text in special tags to achieve certain things:
    * `<firstupper>` - the first letter of the string inside this tag will be changed to the uppercase variant
        * Example: `slime <firstupper>diva</firstupper>` will be changed into `slime Diva`
    * `<firstlower>` - the first letter of the string inside this tag will be changed to the lowercase variant
        * Example: `Slime <firstlower>Diva</firstlower>` will be changed into `Slime diva`
    * `<scale=1.0>` - text in this tag will have its scale multiplied by the amount provided in the argument
        * Example: `Slime <scale=0.8>Diva</scale>` will resize the word "Diva" to 80% of its original size
        * This is particularly useful for shrinking text that otherwise wouldn't fit in the UI
* You can change the `Reload Language Keybind` config option to let yourself reload your translation files right inside the game, without having to relaunch it every time you make a change
