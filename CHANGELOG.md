## 0.0.4
* Added custom font support:
	* In addition to creating custom languages, you can now create "font bundles"
		* Font bundle creation guide here: https://github.com/TheMysticSword/Localyssation/wiki/Font-Bundles
	* A default extra font bundle is included in the mod, named `localyssation_extra_font_bundle`. It contains the following fonts:
		* Alegreya (includes Latin, Greek and Cyrillic glyphs)
		* Open Sans (includes Latin, Greek, Cyrillic and Hebrew glyphs)
		* Source Han Sans (includes glyphs used in Simplified Chinese, Traditional Chinese, Japanese and Korean)
	* `localyssationLanguage.json` now includes 2 new fields, `fontReplacementCentaur` and `fontReplacementTerminalGrotesque`:
		* Centaur is the fancy fantasy serif font
		* Terminal Grotesque is the flat font used in most UI and tooltips
		* To set a font replacement, change these fields like this:
		```
		"fontReplacementCentaur": {
			"bundleName": "localyssation_extra_font_bundle",
			"fontName": "Alegreya"
		}
		```
		where `bundleName` is the name of the font bundle you want to use a font from, and `fontName` is the name of the font you want to use
* Renamed the following keys:
	* `FORMAT_EQUIP_ITEM_RARITY` to `FORMAT_ITEM_RARITY`
* Dialogue "quick sentences" can now be translated (e.g. quest accept/complete responses, enchanting responses, etc.)
* Fixed Skrit's gamble item descriptions being revealed, and made gamble item text translatable (under `EQUIP_TOOLTIP_GAMBLE_ITEM_` keys)
* Made new settings translatable:
	* Display Global Nametags
	* Hide Stat Point Notice Panel
	* Hide Skill Point Notice Panel
* Fixed "Log Untranslated Strings" button not calculating the percentage progress properly
## 0.0.3
* The mod now requires [EasySettings](https://thunderstore.io/c/atlyss/p/Nessie/EasySettings/)
* Moved the Language selection button and "Developer" configs to **Settings > Mods > Localyssation**
* "Developer" configs were renamed to "Translator" configs, and are now disabled by default. They can be re-enabled by checking "Translator Mode" in the mod settings
* Added new buttons for translators:
	* Add Missing Keys to Current Language - finds strings that exist in the default language, but not in the currently selected language, and saves them to the current language's file
	* Log Untranslated Strings - finds every string that is the same in both the default and the selected languages, and prints them to the BepInEx log output
* Skills are now translatable
* Added `SKILL_<name>_RANK_<rank number>_DESCRIPTOR` strings for skills' descriptions on certain ranks
* Added optional `OF` variant for player class strings
	* This variant gets used in the Skills tab for the "<class name> Skillbook" and "<class name> Skills" strings
* Fixed the Reset button on some settings not being translated
## 0.0.2
* Fixed Nickname, Race, Class, Level and Experience strings not getting translated in the Stats tab menu
* Fixed FORMAT_QUEST_PROGRESS_CREEPS_KILLED being forced to use the _VARIANT_MANY variant even if the current language doesn't have one
* Added info about `VARIANT_MANY` creep strings in the readme
## 0.0.1
* First pre-release.
