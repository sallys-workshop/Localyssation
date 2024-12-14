# Localyssation
A work-in-progress localisation library mod for the video game [ATLYSS](https://store.steampowered.com/app/2768430/ATLYSS/).

## :warning: Important note :warning:
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
    * Skills (partially)
    * Quests (partially)
  * Quests
  * Dialogues

## For translators
You do not have to ask me if you can make a translation. This mod is a library that lets others easily make their own translations. It is not intended to include translations on its own.  
Go to the [For Translators](https://github.com/TheMysticSword/Localyssation/wiki/For-Translators) wiki page for a guide on installing the mod and creating your own language.
