using HarmonyLib;
using System;
using UnityEngine;

namespace Localyssation.Patches
{
    internal static class GameLoadPatches
    {
        [HarmonyPatch(typeof(GameManager), "Cache_ScriptableAssets")]
        [HarmonyPostfix]
        public static void GameManager_Cache_ScriptableAssets(GameManager __instance)
        {
            // cached scriptables
            foreach (var item in __instance._cachedScriptableItems.Values)
            {
                var key = KeyUtil.GetForAsset(item);
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME", item._itemName);
                Localyssation.defaultLanguage.RegisterKey($"{key}_DESCRIPTION", item._itemDescription);
            }
            foreach (var creep in __instance._cachedScriptableCreeps.Values)
            {
                var key = KeyUtil.GetForAsset(creep);
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME", creep._creepName);
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME_IN_QUEST_TRACK_SLAIN", creep._creepName + " slain");
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME_IN_QUEST_TRACK_SLAIN_MULTIPLE", creep._creepName + "s slain");
            }
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
            foreach (var condition in __instance._cachedScriptableConditions.Values)
            {
                var key = $"{KeyUtil.GetForAsset(condition)}_{condition._conditionRank}";
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME", condition._conditionName);
                Localyssation.defaultLanguage.RegisterKey($"{key}_DESCRIPTION", condition._conditionDescription);
            }
            foreach (var statModifier in __instance._cachedScriptableStatModifiers.Values)
            {
                var key = KeyUtil.GetForAsset(statModifier);
                Localyssation.defaultLanguage.RegisterKey($"{key}_TAG", statModifier._modifierTag);
            }
            foreach (var race in __instance._cachedScriptableRaces.Values)
            {
                var key = KeyUtil.GetForAsset(race);
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME", race._raceName);
                Localyssation.defaultLanguage.RegisterKey($"{key}_DESCRIPTION", race._raceDescription);
                Localyssation.defaultLanguage.RegisterKey($"{key}_MISC", race._miscName);
            }
            foreach (var combatElement in __instance._cachedScriptableCombatElements.Values)
            {
                var key = KeyUtil.GetForAsset(combatElement);
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME", combatElement._elementName);
            }
            Localyssation.defaultLanguage.RegisterKey($"PLAYER_CLASS_EMPTY_NAME", GameManager._current._statLogics._emptyClassName);
            foreach (var playerClass in __instance._cachedScriptablePlayerClasses.Values)
            {
                var key = KeyUtil.GetForAsset(playerClass);
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME", playerClass._className);
            }
            foreach (var skill in __instance._cachedScriptableSkills.Values)
            {
                var key = KeyUtil.GetForAsset(skill);
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME", skill._skillName);
                Localyssation.defaultLanguage.RegisterKey($"{key}_DESCRIPTION", skill._skillDescription);
            }
            foreach (var statAttribute in GameManager._current._statLogics._statAttributes)
            {
                var key = KeyUtil.GetForAsset(statAttribute);
                Localyssation.defaultLanguage.RegisterKey($"{key}_NAME", statAttribute._attributeName);
                Localyssation.defaultLanguage.RegisterKey($"{key}_DESCRIPTOR", statAttribute._attributeDescriptor);
            }

            // uncached scriptables
            foreach (var weaponType in Resources.LoadAll<ScriptableWeaponType>(""))
            {
                var key2 = KeyUtil.GetForAsset(weaponType);
                Localyssation.defaultLanguage.RegisterKey($"{key2}_NAME", weaponType._weaponTypeName);
            }

            // enums
            foreach (ItemRarity itemRarity in Enum.GetValues(typeof(ItemRarity)))
                Localyssation.defaultLanguage.RegisterKey(KeyUtil.GetForAsset(itemRarity), itemRarity.ToString());

            foreach (DamageType damageType in Enum.GetValues(typeof(DamageType)))
                Localyssation.defaultLanguage.RegisterKey(KeyUtil.GetForAsset(damageType), damageType.ToString());

            // misc
            Localyssation.defaultLanguage.strings["FORMAT_QUEST_MENU_CELL_REWARD_CURRENCY"] = $"{{0}} {GameManager._current._statLogics._currencyName}";

            if (Localyssation.configCreateDefaultLanguageFiles.Value)
                Localyssation.defaultLanguage.WriteToFileSystem();
        }
    }
}
