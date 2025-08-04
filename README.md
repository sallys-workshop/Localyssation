# Localyssation

[![Version](https://thunderstore-badges.foreverjlong.workers.dev/Sallys_Workshop/Localyssation/version)](https://thunderstore.io/c/atlyss/p/Sallys_Workshop/Localyssation/)
[![Downloads](https://thunderstore-badges.foreverjlong.workers.dev/Sallys_Workshop/Localyssation/downloads)](https://thunderstore.io/c/atlyss/p/Sallys_Workshop/Localyssation/)



A work-in-progress localization library mod for [ATLYSS](https://store.steampowered.com/app/2768430/ATLYSS/). Check [Github page](https://github.com/sallys-workshop/Localyssation/) for newest update.

## Requirements

- BepInEx 5.4
- EasySettings 1.1.8+
- **UNINSTALL** BepInEx Fixer
  - Outdated. May install via mod dependencies when using mod manager (r2modman) but is unnecessary for latest ATLYSS mods and can cause issues.

## :warning: Important note :warning:

This mod **REQUIRES** a translation patch of your language to show in-game translation, it is **NOT** intended to include translations on its own, while translators can make translation patches with this mod (see below).

Currently listed translation patch(es):

| Language |                            Author                            |                          Patch Link                          |
| :------: | :----------------------------------------------------------: | :----------------------------------------------------------: |
| 简体中文 |                莎莉工作室(Sallys's Workshop)                 | [Github](https://github.com/sallys-workshop/zh-CN-patch-for-Localyssation) [Thunder Store] |
| 繁體中文 |  [jasonpepe](https://thunderstore.io/c/atlyss/p/jasonpepe/)  | [LocalyssationZHTW](https://thunderstore.io/c/atlyss/p/jasonpepe/LocalyssationZHTW) |
| Español  |    [Alhenix](https://thunderstore.io/c/atlyss/p/Alhenix/)    | [LocalyssationESLA](https://thunderstore.io/c/atlyss/p/Alhenix/localyssationESLA/) |
|  한국어  | [KR_Kemonoz](https://thunderstore.io/c/atlyss/p/KR_Kemonoz/) | [AtlyssKr](https://thunderstore.io/c/atlyss/p/KR_Kemonoz/AtlyssKr/) |
|Brasileiro|[Guachenim](https://thunderstore.io/c/atlyss/p/Guachenim/)|[Thunder Store](https://thunderstore.io/c/atlyss/p/Guachenim/Ordem_e_Atlyss_a_Braziliant_PTBR_Translation/)|

## How to use

1. Install BepInEx.
2. Install EasySettings and Localyssation.
3. Put a translation patch inside the `plugins` folder. 

## Current features

* Add Localyssation options in the mods tab of settings (Install EasySettings if missing).
  * Where you can change languages or fonts (unifont by default).
* Show correct texts in chat box and chat bubbles, if the words can be found in font bundle you use.
* Load translation patches found in the BepInEx plugins folder.
  * A patch consists of the following things:
    * `localyssationLanguage.json` with information about the language.
    * `strings.yml` with text keys and corresponding translated text.
    * A folder to keep them together.
  * Including translation for mods (if the modder provides).
* Useful features for translators' convenience.
  * Exporting original `strings.yml` to begin with.
  * Generate `.md` files for items and quests by detail inside the mod folder.
    * `ScriptableItem.md`: icons, translation keys and names
    * `ScriptableQuest.md`: givers, translation keys, types, subtypes, and names
  * Option to show KEY of empty translations in game.
  * Reload strings with hot key in game.


## Report a bug / missing translation

Use issue feature on GitHub please. [Create an Issue](https://github.com/sallys-workshop/Localyssation/issues/new/choose)


## For translators or mod creaters

Feel free to create your own translation patch for your language. Send issues to let me put your patch link in this README. 

Go to this [wiki](https://github.com/sallys-workshop/Localyssation/wiki/To-create-a-game-translation) page for guidance of installing the mod and creating your own language translation patch, as well as making your own [Font Bundles](https://github.com/sallys-workshop/Localyssation/wiki/Font-Bundle-aka.-Why-characters-don't-get-displayed-(2.0)) (make sure using **profitable & embeddable** fonts to avoid potential legal issues).

Localyssation supports mod translation after `2.0.0`, check this [wiki page](https://github.com/sallys-workshop/Localyssation/wiki/To-create-a-mod-translation) for more information.

Also, this mod can't help with the texts inside pictures. Try texture swapping if you'd like to translate them.

## Credit

Originally created by [TheMysticSword](https://github.com/TheMysticSword/Localyssation), then forked and maintained by [TowardtheStars](https://github.com/TowardtheStars) as part of the CHS translation project held by team **Sally's Workshop**(莎莉小店).
