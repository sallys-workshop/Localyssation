using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localyssation
{

    public static class KeyUtil
    {
        public static string Normalize(string key)
        {
            return new string(
                key.ToUpper().Replace(" ", "_").Replace("/", "_")
                .Where(x => "ABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789".Contains(x))
                .ToArray());
        }

        public static string GetForAsset(ScriptableItem asset)
        {
            return $"ITEM_{Normalize(asset._itemName)}";
        }

        public static string GetForAsset(ScriptableWeaponType asset)
        {
            return $"WEAPON_TYPE_{Normalize(asset._weaponTypeClassTag)}";
        }

        public static string GetForAsset(ScriptableCreep asset)
        {
            return $"CREEP_{Normalize(asset._creepName)}";
        }

        public static string GetForAsset(ScriptableQuest asset)
        {
            return $"QUEST_{Normalize(asset._questName)}";
        }

        public static string GetForAsset(QuestTriggerRequirement asset)
        {
            return $"QUEST_TRIGGER_{Normalize(asset._questTriggerTag)}";
        }

        public static string GetForAsset(ScriptableCondition asset)
        {
            return $"CONDITION_{Normalize(asset._conditionName)}";
        }

        public static string GetForAsset(ScriptableStatModifier asset)
        {
            return $"STAT_MODIFIER_{Normalize(asset._modifierTag)}";
        }

        public static string GetForAsset(ScriptablePlayerRace asset)
        {
            return $"RACE_{Normalize(asset._raceName)}";
        }

        public static string GetForAsset(ScriptableCombatElement asset)
        {
            return $"COMBAT_ELEMENT_{Normalize(asset._elementName)}";
        }

        public static string GetForAsset(ScriptablePlayerBaseClass asset)
        {
            return $"PLAYER_CLASS_{Normalize(asset._className)}";
        }

        public static string GetForAsset(ScriptableSkill asset)
        {
            return $"SKILL_{Normalize(asset._skillName)}";
        }

        public static string GetForAsset(ScriptableStatAttribute asset)
        {
            return $"STAT_ATTRIBUTE_{Normalize(asset._attributeName)}";
        }

        public static string GetForAsset(ItemRarity asset)
        {
            return $"ITEM_RARITY_{Normalize(asset.ToString())}";
        }

        public static string GetForAsset(DamageType asset)
        {
            return $"DAMAGE_TYPE_{Normalize(asset.ToString())}";
        }

        public static string GetForAsset(SkillControlType asset)
        {
            return $"SKILL_CONTROL_TYPE_{Normalize(asset.ToString())}";
        }

        public static string GetForAsset(CombatColliderType asset)
        {
            return $"COMBAT_COLLIDER_TYPE_{Normalize(asset.ToString())}";
        }

        public static string GetForAsset(ScriptableDialogData asset)
        {
            return $"{Normalize(asset.name.ToString())}";
        }

        public static string GetForAsset(ItemType asset)
        {
            return $"ITEM_TOOLTIP_TYPE_{Normalize(asset.ToString())}";
        }

        public static string GetForAsset(PlayerClassTier asset)
        {
            return $"PLAYER_CLASS_TIER_{Normalize(asset._classTierName)}";
        }

        public static string GetForAsset(ZoneType asset)
        {
            return $"ZONE_TYPE_{Normalize(asset.ToString())}";
        }

        public static string GetForMapRegionTag(string regionTag)
        {
            return $"MAP_REGION_TAG_{KeyUtil.Normalize(regionTag)}";
        }
    }
}
