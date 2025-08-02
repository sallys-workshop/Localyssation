## 2.2.2
- Bug fix
	- Issue #26 & #27
- New translation keys
	- Added death prompt for localisation

## 2.2.1
- Bug fix
	- Fixed a bug that items in enchanter subtitles are not translated
- New translation keys
	- Added translation to map name on topleft corner
	- Added translation for multiplayer room creating and searching


## 2.2.0
- New translation keys
	- Add new translation keys for chats
- Bug fix
	- Fixed enchanter translation keys registration
	- Fixed an issue that buttons for translation mode will not disappear when translation mode is off
	- Stat discriptor tag in equipment tooltip now update with language changes
- New feature
	- New configuration `Log Vanilla Fonts` to control whether to log vanilla fonts, making loading faster.
	- `TestMeshProUGUI` type of text components now are available to replace font in `componentSpecifiedFontReplacement`

## 2.1.3
- Bug fixes
	- Fixed a bug that enchantment on items are not properly displayed when dropping on ground
	- Fixed a bug that quest accepting and completing messages do not display translated strings
	- Fixed translation of "Points" in stat tab menu
- New translation keys
	- Added new translation keys for game logic and chat strings when accepting tasks
	- Added some task-related error messages
	- Added translation to "Points Avaliable" hint when points avaliable


## 2.1.0
- Fixed YML file searching logic
- Fixed tsv file loading logic
- Fixed Localyssation config panel translation
- Added translations of `NetTrigger`s. Thanks to [Marioalexsan](https://github.com/Marioalexsan)
	- Diva dialogs
	- Dungeon and boss dialogs

## 2.0.1
Added `unifont` to vanilla `TMP_FontAsset` fallback.

## 2.0.0
Nothing much, basically 1.99.0 with small fixes

## 1.99.0
- Remake font bundle
	- Remove json descriptor
	- Using `.fontbundle` suffix to locate asset bundles and identify them as font bundles
	- Associated modification for `localyssationLanguage.json`
		- `chatFont` field for replacing all `TMP_FontAsset` type font
			- Includes chat, chat bubble, items on the ground, and nametag of players
		- `fontReplacement` dictionary for replacing arbitrary `Font` type font in game, compatible to mods.
		- `pathSpecifiedFontReplacement` for replacing `Text` component in specified `GameObject` path, for some of UI elements in game have weird settings and you want to stick a different font for them.
		- `replacementForXXXFont` field are removed (Will not cause error if remains in your json)
		- Font description for replacment don't need to specify which font bundle to use
		- Font scale merges into font descriptor in language json
- Using `YAML` as translation strings file
	- Will load `tsv` format for backward compatibility
	- Default language generation will use yml format
	- Allow modders adding their own localisation file for their own mod
	- Add missing translation keys button now creates a `missing.{CurrentLanguage.code}.yml` at the same folder of your `localyssationLanguage.json`
		- Note: anything inside that yml, if exists, **will be erased**
- Localisation keys updates
	- Localyssation settings tab
	- World Portal
	- Dungeon Portal
	- Re-roll button in gamble shop
- Fixed localisations
	- Action bar when casting spells
	- Cancel button in World portal dialogs

## 202507.07.1.2
* Fixed an issue that "enchanted" creeps cannot display their "enchantment" correctly.
* Added multiplayer "Join" "Host" and "Return" to translation


## 202507.07.1.1
* Update mod to ATLYSS 72025.a7
	* UI translation for shops, enchanter and more
	* Fixed non-element weapon damage text
	* Fixed shield type descriptor in tooltip
	* Fixed gamble item tooltips
	* Fixed and map splash text
	* Added some tool classes and methods for better transpilers
	* Added portal caption and interaction tooltip translations


## 202507.06.1.0
* Update mod to ATLYSS 72025.a6
	* Total rework for items and skills
	* Added hundreds of translation keys
	* Made more UI element translatable
	* Fixed erros caused by quest condition descriptors
* Replace extra font bundle with `unifont`. It contains most of unicode characters.
	* `localyssationLanguage.json` now includes 1 more new field `fontReplacementLibrationSans`:
		* Same as `fontReplacementCentaur` and `fontReplacementTerminalGrotesque`
		* Mostly used in chat box
* Restructurized code, separate codes to more files, easier for updating.
	* `ReplaceText.cs => ReplaceText/*.cs`
		* Moved most of utility methods to `Localyssation.Patches.ReplaceText.RTUtil`
			* Make generalized port of utility methods
		* Most patchers are under class `Localyssation.Patches.ReplaceText.RTReplacer`
		* Use symbols instead of constant strings as translation keys for static cases. No more typos. Symbols stored in `Localyssation.I18nKeys`
	* `PatcherUtil.cs` for parametric locating nested methods
	* Translation key registers for `Enum`s in `GameLoadPatches.cs` moved to `Localyssation.I18nKeys.Enums`, making them reachable during static loading.

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
