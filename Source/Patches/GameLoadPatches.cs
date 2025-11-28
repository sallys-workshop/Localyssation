using HarmonyLib;
using Localyssation.Exporter;
using Localyssation.LanguageModule;
using Localyssation.Patches.ReplaceText;
using Localyssation.Util;
using System;
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
        [HarmonyPatch(typeof(GameManager), nameof(GameManager.Cache_ScriptableAssets))]
        [HarmonyPostfix]
        public static void GameManager_Cache_ScriptableAssets(GameManager __instance)
        {
            Localyssation.logger.LogMessage("Dynamically registering I18n keys for scriptables and dialogs.");
            if (LocalyssationConfig.ExportExtra)
            {
                ExportUtil.InitExports();
            }
            // cached scriptables
            // items
            RegisterCachedScriptableObjects(__instance._cachedScriptableItems, item =>
            {
                var key = KeyUtil.GetForAsset(item);
                LanguageManager.RegisterKey(key.Name, item._itemName);
                LanguageManager.RegisterKey(key.NamePlural, item._itemName);
                LanguageManager.RegisterKey(key.Description, item._itemDescription);
            });
            // creeps
            __instance._cachedScriptableCreeps.Values.Do(creep =>
            {
                var key = KeyUtil.GetForAsset(creep);
                LanguageManager.RegisterKey(key.Name, creep._creepName);
                LanguageManager.RegisterKey(key.NamePlural, creep._creepName + "s");
            });
            // quests
            foreach (var quest in __instance._cachedScriptableQuests.Values)
            {
                var key = KeyUtil.GetForAsset(quest);
                LanguageManager.RegisterKey(key.Name, quest._questName);
                LanguageManager.RegisterKey(key.Description, quest._questDescription);
                LanguageManager.RegisterKey(key.CompleteReturnMessage, quest._questCompleteReturnMessage);
                
                foreach (var questTriggerRequirement in quest._questObjective._questTriggerRequirements)
                {
                    LanguageManager.RegisterKey($"{KeyUtil.GetForAsset(questTriggerRequirement)}_PREFIX", questTriggerRequirement._prefix);
                    LanguageManager.RegisterKey($"{KeyUtil.GetForAsset(questTriggerRequirement)}_SUFFIX", questTriggerRequirement._suffix);
                }
            }
            // conditions
            foreach (var condition in __instance._cachedScriptableConditions.Values)
            {
                var key = KeyUtil.GetForAsset(condition);
                LanguageManager.RegisterKey(key.Name, condition._conditionName);
                LanguageManager.RegisterKey(key.Description, condition._conditionDescription);
            }
            // stat modifiers
            foreach (var statModifier in __instance._cachedScriptableStatModifiers.Values)
            {
                var key = KeyUtil.GetForAsset(statModifier);
                LanguageManager.RegisterKey($"{key}", statModifier._modifierTag);
            }
            // races
            foreach (var race in __instance._cachedScriptableRaces.Values)
            {
                var key = KeyUtil.GetForAsset(race);
                LanguageManager.RegisterKey(key.Name, race._raceName);
                LanguageManager.RegisterKey(key.Description, race._raceDescription);
                LanguageManager.RegisterKey($"{key}_MISC", race._miscName);
            }
            // combat elements
            foreach (var combatElement in __instance._cachedScriptableCombatElements.Values)
            {
                var key = KeyUtil.GetForAsset(combatElement);
                LanguageManager.RegisterKey(key.Name, combatElement._elementName);
            }
            LanguageManager.RegisterKey($"PLAYER_CLASS_EMPTY_NAME", GameManager._current._statLogics._emptyClassName);

            // player classes
            foreach (var playerClass in __instance._cachedScriptablePlayerClasses.Values)
            {
                var key = KeyUtil.GetForAsset(playerClass);
                LanguageManager.RegisterKey(key.Name, playerClass._className);
                // PlayerClassTier
                foreach (PlayerClassTier playerClassTier in playerClass._playerClassTiers)
                {
                    LanguageManager.RegisterKey(KeyUtil.GetForAsset(playerClassTier).Name, playerClassTier._classTierName);
                }
            }

            // skills
            foreach (var skill in __instance._cachedScriptableSkills.Values)
            {
                var key = KeyUtil.GetForAsset(skill);
                LanguageManager.RegisterKey(key.Name, skill._skillName);
                LanguageManager.RegisterKey(key.Description, skill._skillDescription);
                //for (var rankIndex = 0; rankIndex < skill._skillRanks.Length; rankIndex++)
                //{
                //    var rank = skill._skillRanks[rankIndex];
                //    LanguageManager.RegisterKey($"{key}_RANK_{rankIndex + 1}_DESCRIPTOR", rank._rankDescriptor);
                //}
            }
            // stats
            foreach (var statAttribute in GameManager._current._statLogics._statAttributes)
            {
                var key = KeyUtil.GetForAsset(statAttribute);
                LanguageManager.RegisterKey(key.Name, statAttribute._attributeName);
                LanguageManager.RegisterKey($"{key}_DESCRIPTOR", statAttribute._attributeDescriptor);
            }

            // condition group
            foreach (var conditionGroup in GameManager._current._cachedScriptableConditionGroups.Values)
            {
                var key = KeyUtil.GetForAsset(conditionGroup);
                LanguageManager.RegisterKey($"{key}_NAME", conditionGroup._conditionGroupTag);
            }

            // uncached scriptables
            // weapon type
            foreach (var weaponType in Resources.LoadAll<ScriptableWeaponType>(""))
			{
				string key = KeyUtil.GetForAsset(weaponType);
				LanguageManager.RegisterKey(key, weaponType._weaponTypeClassTag);
			}
            // dialog data
            foreach (var dialogData in Resources.LoadAll<ScriptableDialogData>(""))
            {
                var key = KeyUtil.GetForAsset(dialogData);
                LanguageManager.RegisterKey($"{key}_NAME_TAG", dialogData._nameTag);

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
                        LanguageManager.RegisterKey(quickSentenceKey, quickSentence);
                    }
                }

                // export quest info
                if (dialogData._scriptableQuests.Length > 0 && LocalyssationConfig.ExportExtra)
                {
                    new ScriptableQuestExporter(dialogData._nameTag).Export(dialogData._scriptableQuests);
                }
            }

            // shopkeep
            Resources.LoadAll<ScriptableShopkeep>("").ToList().ForEach(scriptableShopkeep =>
            {
                LanguageManager.RegisterKey(KeyUtil.GetForAsset(scriptableShopkeep) + "_SHOP_NAME", scriptableShopkeep._shopName);
            });

            /// enums
            /// move to <see cref="I18nKeys.Enums"/>


            // scene-specific
            // this temporarily loads EVERY scene in the game to gather scene-specific keys, so we'll do it only when necessary
            if (LocalyssationConfig.TranslatorMode && LocalyssationConfig.CreateDefaultLanguageFiles)
            {
                Localyssation.instance.StartCoroutine(RegisterSceneSpecificStrings());
            }

            // misc
            LanguageManager.RegisterKey("FORMAT_QUEST_MENU_CELL_REWARD_CURRENCY", $"{{0}} {GameManager._current._statLogics._currencyName}");

            // Export extra
            if (LocalyssationConfig.ExportExtra)
            {
                new ScriptableItemExporter().Export(GameManager._current._cachedScriptableItems.Values);
            }


            if (LocalyssationConfig.TranslatorMode && LocalyssationConfig.CreateDefaultLanguageFiles)
                LanguageManager.UpdateDefaultLanguageFile();
            Resources.UnloadUnusedAssets();
        }

        private static void RegisterCachedScriptableObjects<KeyType, ValueType>(IDictionary<KeyType, ValueType> scriptables, Action<ValueType> action)
        {
            scriptables.Values.ToList().ForEach(action);
        }

        static IEnumerator RegisterSceneSpecificStrings()
        {
            for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
                if (!excludedSceneNames.Any(x => scenePath.Contains(x)))
                {
                    yield return SceneManager.LoadSceneAsync(scenePath, LoadSceneMode.Additive);

                    var scene = SceneManager.GetSceneByPath(scenePath);
                    if (scene.IsValid())
                    {
                        string sceneName = scene.name;

                        RegisterDialogTriggers(sceneName);
                        RegisterMapVisualOverrideTrigger(sceneName);
                        RegisterMapInstance(sceneName);
                        RegisterNetTriggers(sceneName);
                        yield return SceneManager.UnloadSceneAsync(scene);
                    }
                }
            }

            yield return Resources.UnloadUnusedAssets();

            LanguageManager.UpdateDefaultLanguageFile();

            yield break;
        }

        private static void RegisterKeysForDialogBranch(string dialogDataKey, string keySuffixBranch, DialogBranch branch)
        {
            for (var dialogIndex = 0; dialogIndex < branch.dialogs.Length; dialogIndex++)
            {
                var dialog = branch.dialogs[dialogIndex];
                LanguageManager.RegisterKey($"{dialogDataKey}_{keySuffixBranch}_DIALOG_{dialogIndex}_INPUT", dialog._dialogInput);

                if (dialog._altInputs != null && dialog._altInputs.Length != 0)
                {
                    for (var altInputIndex = 0; altInputIndex < dialog._altInputs.Length; altInputIndex++)
                    {
                        LanguageManager.RegisterKey($"{dialogDataKey}_{keySuffixBranch}_DIALOG_{dialogIndex}_INPUT_ALT_{altInputIndex}", dialog._altInputs[altInputIndex]);
                    }
                }

                for (var selectionIndex = 0; selectionIndex < dialog._dialogSelections.Length; selectionIndex++)
                {
                    var selection = dialog._dialogSelections[selectionIndex];
                    LanguageManager.RegisterKey($"{dialogDataKey}_{keySuffixBranch}_DIALOG_{dialogIndex}_SELECTION_{selectionIndex}", selection._selectionCaption);
                }
            }
        }

        static readonly List<string> excludedSceneNames = new List<string>()
        {
            "00_bootStrapper", "01_rootScene"
        };
        

        private static Func<MonoBehaviour, bool> IsInSceneGenerator(string sceneName)
        {
            return o => o.gameObject.scene.name == sceneName;
        }

        private static void RegisterDialogTriggers(string sceneName)
        {
            GameObject.FindObjectsOfType<DialogTrigger>(true)
                .Where(IsInSceneGenerator(sceneName)).Cast<DialogTrigger>()
                .Where(o => o._useLocalDialogBranch)
                .Do(dialogTrigger =>
                {
                    var key = KeyUtil.GetForAsset(dialogTrigger._scriptDialogData);
                    RegisterKeysForDialogBranch(
                        key,
                        KeyUtil.Normalize($"LOCAL_BRANCH_{sceneName}_{PathUtil.GetChildTransformPath(dialogTrigger.transform, 2)}"),
                        dialogTrigger._localDialogBranch
                        );
                });
        }


        private static void RegisterMapVisualOverrideTrigger(string sceneName)
        {
            GameObject.FindObjectsOfType<MapVisualOverrideTrigger>(true)
                .Where(IsInSceneGenerator(sceneName)).Cast<MapVisualOverrideTrigger>()
                .Do(mapVisualOverrideTrigger =>
                {
                    string regionTag = mapVisualOverrideTrigger._reigonName;
                    LanguageManager.RegisterKey(
                        KeyUtil.GetForMapRegionTag(regionTag), regionTag
                    );
                });

        }

        private static void RegisterMapInstance(string sceneName)
        {
            GameObject.FindObjectsOfType<MapInstance>(true)
                .Where(IsInSceneGenerator(sceneName)).Cast<MapInstance>()
                .Do(mapInstance =>
                {
                    string mapName = mapInstance._mapName;
                    LanguageManager.RegisterKey(
                        KeyUtil.GetForMapName(mapName),
                        mapName
                    );
                });
        }

        private static void RegisterNetTriggers(string sceneName)
        {
            GameObject.FindObjectsOfType<NetTrigger>(true)
                .Where(IsInSceneGenerator(sceneName)).Cast<NetTrigger>()
                .Where(netTrigger => netTrigger._triggerMessage != null)
                .Do(netTrigger =>
                {
                    string triggerName = netTrigger.name;
                    TriggerMessage triggerMessage = netTrigger._triggerMessage;
                    if (!string.IsNullOrEmpty(triggerMessage._singleMessage))
                    {
                        LanguageManager.RegisterKey(
                            KeyUtil.GetForAsset(netTrigger).SingleMessage,
                            triggerMessage._singleMessage
                        );
                    }
                    if (triggerMessage._triggerMessageArray.Length > 0)
                    {

                        triggerMessage._triggerMessageArray.Select((msg, idx) =>
                        {
                            if (!string.IsNullOrEmpty(triggerMessage._triggerMessageArray[idx]))
                            {
                                LanguageManager.RegisterKey(
                                    KeyUtil.GetForAsset(netTrigger).MessageArray(idx),
                                    triggerMessage._triggerMessageArray[idx]
                                );
                            }
                            return true;
                        }).All(o => o); // finalizer to make sure code above get done 

                    }

                });

        }
    }
}
