# Localyssation
A work-in-progress localisation library mod for the video game [ATLYSS](https://store.steampowered.com/app/2768430/ATLYSS/).
Originally created by [TheMysticSword](https://github.com/TheMysticSword/Localyssation).
This fork is mainly maintained by TowardtheStars, as a part of CHS translation project, but should be applicable to other languages.

## Requirements
- BepInEx 5
- EasySettings

## :warning: Important note :warning:
~~This mod is not finished yet!~~ This mod is mostly finished. The pre-release versions you see are unfinished versions of the mod intended for translators to make their own languages early.

## Current features
* New "Language" option in the "Interface" settings tab
* Loading all custom languages found in the BepInEx plugins folder
  * A language consists of the following files:
    * `localyssationLanguage.json` with information about the language
    * `strings.tsv` with text keys and corresponding translated text
* Exporting game data for translators' convenience
  * Item icons, translation keys and names
  * Quest givers, translation keys, types, subtypes, and names
* Some in-game text gets translated according to the selected language:
  * Main menu
  * Settings
  * Character select menu
  * Character creation menu
  * Equipment tooltips
  * Tab menu:
    * ESC menu
    * Stats
    * Skills
    * Items
    * Quests
    * Who
  * Quests
  * Dialogues
  * Map splash titles

## For translators
You do not have to ask me if you can make a translation. This mod is a library that lets others easily make their own translations. It is not intended to include translations on its own.  
Go to the [For Translators](https://github.com/TheMysticSword/Localyssation/wiki/For-Translators) wiki page for a guide on installing the mod and creating your own language.
