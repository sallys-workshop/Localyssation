using HarmonyLib;
using Localyssation.Exporter;
using Localyssation.Patches.ReplaceText;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Localyssation.Patches
{
    internal static class GameLoadPatches
    {
        /// <summary>
        /// Runtime register i18n keys
        /// Constants such as Enum should be registered at static loading
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPatch(typeof(GameManager), "Cache_ScriptableAssets")]
        [HarmonyPostfix]
        public static void GameManager_Cache_ScriptableAssets(GameManager __instance)
        {
            if (Localyssation.configExportExtra.Value)
            {
                ExportUtil.InitExports();
            }
            // cached scriptables
            // items
            foreach (var item in __instance._cachedScriptableItems.Values)
            {
                var key = KeyUtil.GetForAsset(item);
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME", item._itemName);
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME_PLURAL", item._itemName);
                Localyssation.defaultLanguage.RegisterKey($"{key}_DESCRIPTION", item._itemDescription);
            }
            // creeps
            foreach (var creep in __instance._cachedScriptableCreeps.Values)
            {
                var key = KeyUtil.GetForAsset(creep);
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME", creep._creepName);
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME_PLURAL", creep._creepName + "s");
            }
            // quests
            foreach (var quest in __instance._cachedScriptableQuests.Values)
            {
                var key = KeyUtil.GetForAsset(quest);
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME", quest._questName);
                Localyssation.defaultLanguage.RegisterKey($"{key}_DESCRIPTION", quest._questDescription);
                Localyssation.defaultLanguage.RegisterKey($"{key}_COMPLETE_RETURN_MESSAGE", quest._questCompleteReturnMessage);
                foreach (var questTriggerRequirement in quest._questObjective._questTriggerRequirements)
                {
                    Localyssation.defaultLanguage.RegisterKey($"{KeyUtil.GetForAsset(questTriggerRequirement)}_PREFIX", questTriggerRequirement._prefix);
                    Localyssation.defaultLanguage.RegisterKey($"{KeyUtil.GetForAsset(questTriggerRequirement)}_SUFFIX", questTriggerRequirement._suffix);
                }
            }
            // conditions
            foreach (var condition in __instance._cachedScriptableConditions.Values)
            {
                var key = $"{KeyUtil.GetForAsset(condition)}";
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME", condition._conditionName);
                Localyssation.defaultLanguage.RegisterKey($"{key}_DESCRIPTION", condition._conditionDescription);
            }
            // stat modifiers
            foreach (var statModifier in __instance._cachedScriptableStatModifiers.Values)
            {
                var key = KeyUtil.GetForAsset(statModifier);
                Localyssation.defaultLanguage.RegisterKey($"{key}_TAG", statModifier._modifierTag);
            }
            // races
            foreach (var race in __instance._cachedScriptableRaces.Values)
            {
                var key = KeyUtil.GetForAsset(race);
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME", race._raceName);
                Localyssation.defaultLanguage.RegisterKey($"{key}_DESCRIPTION", race._raceDescription);
                Localyssation.defaultLanguage.RegisterKey($"{key}_MISC", race._miscName);
            }
            // combat elements
            foreach (var combatElement in __instance._cachedScriptableCombatElements.Values)
            {
                var key = KeyUtil.GetForAsset(combatElement);
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME", combatElement._elementName);
            }
            Localyssation.defaultLanguage.RegisterKey($"PLAYER_CLASS_EMPTY_NAME", GameManager._current._statLogics._emptyClassName);

            // player classes
            foreach (var playerClass in __instance._cachedScriptablePlayerClasses.Values)
            {
                var key = KeyUtil.GetForAsset(playerClass);
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME", playerClass._className);
                // PlayerClassTier
                foreach (PlayerClassTier playerClassTier in playerClass._playerClassTiers)
                {
                    Localyssation.defaultLanguage.RegisterKey($"{KeyUtil.GetForAsset(playerClassTier)}_NAME", playerClassTier._classTierName);
                }
            }

            // skills
            foreach (var skill in __instance._cachedScriptableSkills.Values)
            {
                var key = KeyUtil.GetForAsset(skill);
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME", skill._skillName);
                Localyssation.defaultLanguage.RegisterKey($"{key}_DESCRIPTION", skill._skillDescription);
                //for (var rankIndex = 0; rankIndex < skill._skillRanks.Length; rankIndex++)
                //{
                //    var rank = skill._skillRanks[rankIndex];
                //    Localyssation.defaultLanguage.RegisterKey($"{key}_RANK_{rankIndex + 1}_DESCRIPTOR", rank._rankDescriptor);
                //}
            }
            // stats
            foreach (var statAttribute in GameManager._current._statLogics._statAttributes)
            {
                var key = KeyUtil.GetForAsset(statAttribute);
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME", statAttribute._attributeName);
                Localyssation.defaultLanguage.RegisterKey($"{key}_DESCRIPTOR", statAttribute._attributeDescriptor);
            }

            // condition group
            foreach (var conditionGroup in GameManager._current._cachedScriptableConditionGroups.Values)
            {
                var key = KeyUtil.GetForAsset(conditionGroup);
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME", conditionGroup._conditionGroupTag);
            }

            // uncached scriptables
            // weapon type
            foreach (var weaponType in Resources.LoadAll<ScriptableWeaponType>(""))
            {
                foreach (var animationSlot in weaponType._weaponAnimSlots)
                {
                    string key = KeyUtil.GetForAsset(animationSlot);
                    Localyssation.defaultLanguage.RegisterKey(key, animationSlot._weaponNameTag);
                }
            }
            // dialog data
            foreach (var dialogData in Resources.LoadAll<ScriptableDialogData>(""))
            {
                var key = KeyUtil.GetForAsset(dialogData);
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME_TAG", dialogData._nameTag);

                var branchTypes = new Dictionary<DialogBranch[], string>()
                {
                    { dialogData._dialogBranches, "BRANCH" },
                    { dialogData._introductionBranches, "INTRODUCTION_BRANCH" }
                };

                foreach (var branchType in branchTypes)
                {
                    var branchArray = branchType.Key;
                    var branchTypeName = branchType.Value;
                    for (var branchIndex = 0; branchIndex < branchArray.Length; branchIndex++)
                    {
                        var branch = branchArray[branchIndex];
                        RegisterKeysForDialogBranch(key, $"{branchTypeName}_{branchIndex}", branch);
                    }
                }

                var quickSentenceArrays = new Dictionary<string[], string>()
                {
                    { dialogData._shopkeepResponses, "SHOPKEEP_RESPONSE" },
                    { dialogData._shopkeepRejections, "SHOPKEEP_REJECTION" },
                    { dialogData._questAcceptResponses, "QUEST_ACCEPT_RESPONSE" },
                    { dialogData._questCompleteResponses, "QUEST_COMPLETE_RESPONSE" }
                };
                foreach (var quickSentenceArrayData in quickSentenceArrays)
                {
                    var quickSentenceArray = quickSentenceArrayData.Key;
                    var quickSentenceArrayName = quickSentenceArrayData.Value;
                    for (var quickSentenceIndex = 0; quickSentenceIndex < quickSentenceArray.Length; quickSentenceIndex++)
                    {
                        var quickSentence = quickSentenceArray[quickSentenceIndex];
                        var quickSentenceKey = $"{key}_{quickSentenceArrayName}_{quickSentenceIndex}";
                        RTReplacer.dialogManagerQuickSentencesHack[quickSentence] = quickSentenceKey;
                        Localyssation.defaultLanguage.RegisterKey(quickSentenceKey, quickSentence);
                    }
                }

                // export quest info
                if (dialogData._scriptableQuests.Length > 0 && Localyssation.configExportExtra.Value)
                {
                    new ScriptableQuestExporter(dialogData._nameTag).Export(dialogData._scriptableQuests);
                }
            }

            // shopkeep
            Resources.LoadAll<ScriptableShopkeep>("").ToList().ForEach(scriptableShopkeep =>
            {
                Localyssation.defaultLanguage.RegisterKey(KeyUtil.GetForAsset(scriptableShopkeep) + "_SHOP_NAME", scriptableShopkeep._shopName);
            });

            /// enums
            /// move to <see cref="I18nKeys.Enums"/>


            // scene-specific
            // this temporarily loads EVERY scene in the game to gather scene-specific keys, so we'll do it only when necessary
            if (Localyssation.configTranslatorMode.Value && Localyssation.configCreateDefaultLanguageFiles.Value)
            {
                Localyssation.instance.StartCoroutine(RegisterSceneSpecificStrings());
            }

            // misc
            Localyssation.defaultLanguage.strings["FORMAT_QUEST_MENU_CELL_REWARD_CURRENCY"] = $"{{0}} {GameManager._current._statLogics._currencyName}";

            // Export extra
            if (Localyssation.configExportExtra.Value)
            {
                new ScriptableItemExporter().Export(GameManager._current._cachedScriptableItems.Values);
            }


            if (Localyssation.configTranslatorMode.Value && Localyssation.configCreateDefaultLanguageFiles.Value)
                Localyssation.defaultLanguage.WriteToFileSystem();
        }

        private static void RegisterKeysForDialogBranch(string dialogDataKey, string keySuffixBranch, DialogBranch branch)
        {
            for (var dialogIndex = 0; dialogIndex < branch.dialogs.Length; dialogIndex++)
            {
                var dialog = branch.dialogs[dialogIndex];
                Localyssation.defaultLanguage.RegisterKey($"{dialogDataKey}_{keySuffixBranch}_DIALOG_{dialogIndex}_INPUT", dialog._dialogInput);

                if (dialog._altInputs != null && dialog._altInputs.Length != 0)
                {
                    for (var altInputIndex = 0; altInputIndex < dialog._altInputs.Length; altInputIndex++)
                    {
                        Localyssation.defaultLanguage.RegisterKey($"{dialogDataKey}_{keySuffixBranch}_DIALOG_{dialogIndex}_INPUT_ALT_{altInputIndex}", dialog._altInputs[altInputIndex]);
                    }
                }

                for (var selectionIndex = 0; selectionIndex < dialog._dialogSelections.Length; selectionIndex++)
                {
                    var selection = dialog._dialogSelections[selectionIndex];
                    Localyssation.defaultLanguage.RegisterKey($"{dialogDataKey}_{keySuffixBranch}_DIALOG_{dialogIndex}_SELECTION_{selectionIndex}", selection._selectionCaption);
                }
            }
        }

        static IEnumerator RegisterSceneSpecificStrings()
        {
            var excludedSceneNames = new List<string>()
            {
                "00_bootStrapper", "01_rootScene"
            };
            for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                if (!excludedSceneNames.Any(x => scenePath.Contains(x)))
                {
                    yield return SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);

                    var scene = SceneManager.GetSceneByPath(scenePath);
                    if (scene.IsValid())
                    {
                        var sceneName = scene.name;
                        foreach (var dialogTrigger in GameObject.FindObjectsOfType<DialogTrigger>(true))
                        {
                            if (dialogTrigger._useLocalDialogBranch && dialogTrigger.gameObject.scene.name == sceneName)
                            {
                                var key = KeyUtil.GetForAsset(dialogTrigger._scriptDialogData);
                                RegisterKeysForDialogBranch(key, KeyUtil.Normalize($"LOCAL_BRANCH_{sceneName}_{Util.GetChildTransformPath(dialogTrigger.transform, 2)}"), dialogTrigger._localDialogBranch);
                            }
                        }

                        foreach (var mapVisualOverrideTrigger in GameObject.FindObjectsOfType<MapVisualOverrideTrigger>(true))
                        {
                            if (mapVisualOverrideTrigger.gameObject.scene.name == sceneName)
                            {
                                string regionTag = mapVisualOverrideTrigger._reigonName;
                                Localyssation.defaultLanguage.RegisterKey(
                                    KeyUtil.GetForMapRegionTag(regionTag), regionTag
                                );
                            }
                        }

                        foreach (var mapInstance in GameObject.FindObjectsOfType<MapInstance>(true).Where(o => o.gameObject.scene.name == sceneName))
                        {
                            string mapName = mapInstance._mapName;
                            Localyssation.defaultLanguage.RegisterKey(
                                KeyUtil.GetForMapName(mapName),
                                mapName
                            );
                        }

                        yield return SceneManager.UnloadSceneAsync(scene);
                    }
                }
            }

            yield return Resources.UnloadUnusedAssets();

            Localyssation.defaultLanguage.WriteToFileSystem();

            yield break;
        }
    }
}
