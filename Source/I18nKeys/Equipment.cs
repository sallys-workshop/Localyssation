
using Localyssation.Util;
using System.Text.RegularExpressions;

namespace Localyssation
{
    internal static partial class I18nKeys
    {

        internal static class Equipment
        {
            internal static void init() { }
            public static readonly string TOOLTIP_GAMBLE_ITEM_NAME
                = Create("EQUIP_TOOLTIP_GAMBLE_ITEM_NAME", "Mystery Gear");

            public static readonly string TOOLTIP_GAMBLE_ITEM_RARITY
                = Create("EQUIP_TOOLTIP_GAMBLE_ITEM_RARITY", "[Unknown]");
            public static readonly string TOOLTIP_GAMBLE_ITEM_TYPE
                = Create("EQUIP_TOOLTIP_GAMBLE_ITEM_TYPE", "???");
            public static readonly string TOOLTIP_GAMBLE_ITEM_DESCRIPTION
                = Create("EQUIP_TOOLTIP_GAMBLE_ITEM_DESCRIPTION", "You can't really see what this is until you buy it.");

            public static readonly string FORMAT_LEVEL_REQUIREMENT
                = Create("FORMAT_EQUIP_LEVEL_REQUIREMENT", "Lv-{0}");
            public static readonly string FORMAT_CLASS_REQUIREMENT
                = Create("FORMAT_EQUIP_CLASS_REQUIREMENT", "Class: {0}");
            public static readonly string FORMAT_WEAPON_CONDITION
                = Create("FORMAT_EQUIP_WEAPON_CONDITION", "\n<color=lime>- <color=yellow>{0}%</color> chance to apply {1}.</color>");
            public static readonly string TOOLTIP_TYPE_HELM
                = Create("EQUIP_TOOLTIP_TYPE_HELM", "Helm (Armor)");
            public static readonly string TOOLTIP_TYPE_CHESTPIECE
                = Create("EQUIP_TOOLTIP_TYPE_CHESTPIECE", "Chestpiece (Armor)");
            public static readonly string TOOLTIP_TYPE_LEGGINGS
                = Create("EQUIP_TOOLTIP_TYPE_LEGGINGS", "Leggings (Armor)");
            public static readonly string TOOLTIP_TYPE_CAPE
                = Create("EQUIP_TOOLTIP_TYPE_CAPE", "Cape (Armor)");
            public static readonly string TOOLTIP_TYPE_RING
                = Create("EQUIP_TOOLTIP_TYPE_RING", "Ring (Armor)");

            public static readonly string FORMAT_TOOLTIP_TYPE_WEAPON
                = Create("FORMAT_EQUIP_TOOLTIP_TYPE_WEAPON", "{0} (Weapon)");
            public static readonly string TOOLTIP_TYPE_SHIELD
                = Create("EQUIP_TOOLTIP_TYPE_SHIELD", "Shield (Off Hand)");
            public static readonly string STATS_DAMAGE
                = Create("EQUIP_TOOLTIP_STATS_DAMAGE", "Damage");
            public static readonly string STATS_BASE_DAMAGE
                = Create("EQUIP_TOOLTIP_STATS_BASE_DAMAGE", "Base Damage");
            public static readonly string FORMAT_STATS_DAMAGE_UNSCALED
                = Create("FORMAT_EQUIP_STATS_DAMAGE_UNSCALED", "({0} - {1}) Damage");
            public static readonly string FORMAT_STATS_BLOCK_THRESHOLD
                = Create("FORMAT_EQUIP_STATS_BLOCK_THRESHOLD", "Block threshold: {0} damage");


            public static readonly string FORMAT_WEAPON_DAMAGE_TYPE
                = Create("FORMAT_EQUIP_STATS_WEAPON_DAMAGE_TYPE", "{0} Weapon");
            public static readonly string FORMAT_WEAPON_TRANSMUTE_TYPE
                = Create("FORMAT_EQUIP_STATS_WEAPON_TRASMUTE_TYPE", "Damage Transmute: {0}");

            public static readonly string COMPARE
                = Create("EQUIP_TOOLTIP_COMPARE", "Compare");






            // Stat
            public static string statDisplayKey(string stat)
            {
                return "ITEM_STAT_DISPLAY_" + KeyUtil.Normalize(Regex.Replace(stat, "[A-Z]", x => $"_{x}"));
            }


            public static readonly string STAT_DISPLAY_DEFENSE
                    = Create(statDisplayKey("defense"), "Defense");

            public static readonly string STAT_DISPLAY_MAGIC_DEFENSE
                    = Create(statDisplayKey("magicDefense"), "Mgk. Defense");

            public static readonly string STAT_DISPLAY_MAX_HEALTH
                    = Create(statDisplayKey("maxHealth"), "Max Health");

            public static readonly string STAT_DISPLAY_MAX_MANA
                    = Create(statDisplayKey("maxMana"), "Max Mana");

            public static readonly string STAT_DISPLAY_MAX_STAMINA
                    = Create(statDisplayKey("maxStamina"), "Max Stamina");

            public static readonly string STAT_DISPLAY_ATTACK_POWER
                    = Create(statDisplayKey("attackPower"), "Attack Power");

            public static readonly string STAT_DISPLAY_MAGIC_POWER
                    = Create(statDisplayKey("magicPower"), "Mgk. Power");

            public static readonly string STAT_DISPLAY_DEX_POWER
                    = Create(statDisplayKey("dexPower"), "Dex Power");

            public static readonly string STAT_DISPLAY_CRITICAL
                    = Create(statDisplayKey("critical"), "Phys. Critical");

            public static readonly string STAT_DISPLAY_MAGIC_CRITICAL
                    = Create(statDisplayKey("magicCritical"), "Mgk. Critical");

            public static readonly string STAT_DISPLAY_EVASION
                    = Create(statDisplayKey("evasion"), "Evasion");

            public static readonly string STAT_DISPLAY_RESIST_FIRE
                    = Create(statDisplayKey("resistFire"), "Fire Resist");

            public static readonly string STAT_DISPLAY_RESIST_WATER
                    = Create(statDisplayKey("resistWater"), "Water Resist");

            public static readonly string STAT_DISPLAY_RESIST_NATURE
                    = Create(statDisplayKey("resistNature"), "Nature Resist");

            public static readonly string STAT_DISPLAY_RESIST_EARTH
                    = Create(statDisplayKey("resistEarth"), "Earth Resist");

            public static readonly string STAT_DISPLAY_RESIST_HOLY
                    = Create(statDisplayKey("resistHoly"), "Holy Resist");

            public static readonly string STAT_DISPLAY_RESIST_SHADOW
                    = Create(statDisplayKey("resistShadow"), "Shadow Resist");

        }

    }
}