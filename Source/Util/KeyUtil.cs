using Localyssation.LanguageModule;
using System;
using System.Linq;

namespace Localyssation.Util
{
    public class TranslationKey
    {
        public readonly string key;
        public TranslationKey(string _key) { key = _key; }
        public TranslationKey Name { get => new TranslationKey(key + "_NAME"); }
        public TranslationKey NamePlural { get => new TranslationKey(key + "_NAME_PLURAL"); }
        public TranslationKey Description { get => new TranslationKey(key + "_DESCRIPTION"); }

        public static implicit operator string(TranslationKey key) => key.key;
        public override string ToString() => key;

        public string Localize()
        {
            return Localyssation.GetString(key);
        }

        public string DefaultString()
        {
            return Localyssation.GetDefaultString(key);
        }

        public TranslationKey NameByQuantity(int quantity)
        {
            return Math.Abs(quantity) > 1? NamePlural : Name;
        }
    }

    public class QuestTranslationKey : TranslationKey
    {
        public QuestTranslationKey(string _key) : base(_key) { }

        public TranslationKey CompleteReturnMessage { get => new TranslationKey(key + "_COMPLETE_RETURN_MESSAGE"); }
    }

    public static class KeyUtil
    {
        public static string Normalize(string key)
        {
            return new string(
                key.ToUpper().Replace(" ", "_").Replace("/", "_")
                .Where(x => "ABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789".Contains(x))
                .ToArray());
        }

        
        public static TranslationKey GetForAsset(ScriptableItem asset)
        {
            return new TranslationKey($"ITEM_{Normalize(asset._itemName)}");
        }

        public static TranslationKey GetForAsset(ScriptableConditionGroup asset)
        {
            return new TranslationKey($"CONDITION_GROUP_{Normalize(asset._conditionGroupTag)}");
        }

        public static TranslationKey GetForAsset(WeaponAnimationSlot asset)
        {
            return new TranslationKey($"WEAPON_TYPE_{Normalize(asset._weaponNameTag)}");
        }

        public static TranslationKey GetForAsset(ScriptableCreep asset)
        {
            return new TranslationKey($"CREEP_{Normalize(asset._creepName)}");
        }

        public static QuestTranslationKey GetForAsset(ScriptableQuest asset)
        {
            return new QuestTranslationKey($"QUEST_{Normalize(asset._questName)}");
        }

        public static TranslationKey GetForAsset(QuestTriggerRequirement asset)
        {
            return new TranslationKey($"QUEST_TRIGGER_{Normalize(asset._questTriggerTag)}");
        }

        public static TranslationKey GetForAsset(ScriptableCondition asset)
        {
            return new TranslationKey($"CONDITION_{Normalize(asset._conditionName)}");
        }

        public static TranslationKey GetForAsset(ScriptableStatModifier asset)
        {
            return new TranslationKey($"STAT_MODIFIER_{Normalize(asset._modifierTag)}_TAG");
        }

        public static TranslationKey GetForAsset(ScriptablePlayerRace asset)
        {
            return new TranslationKey($"RACE_{Normalize(asset._raceName)}");
        }

        public static TranslationKey GetForAsset(ScriptableCombatElement asset)
        {
            return new TranslationKey($"COMBAT_ELEMENT_{Normalize(asset._elementName)}");
        }

        public static TranslationKey GetForAsset(ScriptablePlayerBaseClass asset)
        {
            return new TranslationKey($"PLAYER_CLASS_{Normalize(asset._className)}");
        }

        public static TranslationKey GetForAsset(ScriptableSkill asset)
        {
            return new TranslationKey($"SKILL_{Normalize(asset._skillName)}");
        }

        public static TranslationKey GetForAsset(ScriptableStatAttribute asset)
        {
            return new TranslationKey($"STAT_ATTRIBUTE_{Normalize(asset._attributeName)}");
        }

        public static TranslationKey GetForAsset(ItemRarity asset)
        {
            return new TranslationKey($"ITEM_RARITY_{Normalize(asset.ToString())}");
        }

        public static TranslationKey GetForAsset(DamageType asset)
        {
            return new TranslationKey($"DAMAGE_TYPE_{Normalize(asset.ToString())}");
        }

        public static TranslationKey GetForAsset(SkillControlType asset)
        {
            return new TranslationKey($"SKILL_CONTROL_TYPE_{Normalize(asset.ToString())}");
        }

        public static TranslationKey GetForAsset(CombatColliderType asset)
        {
            return new TranslationKey($"COMBAT_COLLIDER_TYPE_{Normalize(asset.ToString())}");
        }

        public static TranslationKey GetForAsset(ScriptableDialogData asset)
        {
            return new TranslationKey($"{Normalize(asset.name.ToString())}");
        }

        public static TranslationKey GetForAsset(ItemType asset)
        {
            return new TranslationKey($"ITEM_TOOLTIP_TYPE_{Normalize(asset.ToString())}");
        }

        public static TranslationKey GetForAsset(PlayerClassTier asset)
        {
            return new TranslationKey($"PLAYER_CLASS_TIER_{Normalize(asset._classTierName)}");
        }

        public static TranslationKey GetForAsset(ZoneType asset)
        {
            return new TranslationKey($"ZONE_TYPE_{Normalize(asset.ToString())}");
        }

        public static string GetForMapRegionTag(string regionTag)
        {
            if (!string.IsNullOrEmpty(regionTag))
                return $"MAP_REGION_TAG_{Normalize(regionTag)}";
            return "";
        }

        public static TranslationKey GetForAsset(SkillToolTipRequirement asset)
        {
            return new TranslationKey($"SKILL_TOOLTIP_REQUIREMENT_{Normalize(asset.ToString())}");
        }

        public static string GetForMapName(string name)
        {
            return $"MAP_NAME_{Normalize(name)}";
        }

        public static TranslationKey GetForAsset(ScriptableShopkeep asset)
        {
            return new TranslationKey($"SHOP_KEEP_{Normalize(asset._shopName)}");
        }

        public static TranslationKey GetForAsset(ShopTab shopTab)
        {
            return new TranslationKey($"SHOP_TAB_{Normalize(shopTab.ToString())}");
        }

    }
}
