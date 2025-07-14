
using System.Text.RegularExpressions;

namespace Localyssation
{
    internal static partial class I18nKeys
    {

        internal static class Equipment
        {
            internal static void init() { }
            public static readonly string TOOLTIP_GAMBLE_ITEM_NAME
                = create("EQUIP_TOOLTIP_GAMBLE_ITEM_NAME", "Mystery Gear");

            public static readonly string TOOLTIP_GAMBLE_ITEM_RARITY
                = create("EQUIP_TOOLTIP_GAMBLE_ITEM_RARITY", "[Unknown]");
            public static readonly string TOOLTIP_GAMBLE_ITEM_TYPE
                = create("EQUIP_TOOLTIP_GAMBLE_ITEM_TYPE", "???");
            public static readonly string TOOLTIP_GAMBLE_ITEM_DESCRIPTION
                = create("EQUIP_TOOLTIP_GAMBLE_ITEM_DESCRIPTION", "You can't really see what this is until you buy it.");

            public static readonly string FORMAT_LEVEL_REQUIREMENT
                = create("FORMAT_EQUIP_LEVEL_REQUIREMENT", "Lv-{0}");
            public static readonly string FORMAT_CLASS_REQUIREMENT
                = create("FORMAT_EQUIP_CLASS_REQUIREMENT", "Class: {0}");
            public static readonly string FORMAT_WEAPON_CONDITION
                = create("FORMAT_EQUIP_WEAPON_CONDITION", "\n<color=lime>- <color=yellow>{0}%</color> chance to apply {1}.</color>");
            public static readonly string TOOLTIP_TYPE_HELM
                = create("EQUIP_TOOLTIP_TYPE_HELM", "Helm (Armor)");
            public static readonly string TOOLTIP_TYPE_CHESTPIECE
                = create("EQUIP_TOOLTIP_TYPE_CHESTPIECE", "Chestpiece (Armor)");
            public static readonly string TOOLTIP_TYPE_LEGGINGS
                = create("EQUIP_TOOLTIP_TYPE_LEGGINGS", "Leggings (Armor)");
            public static readonly string TOOLTIP_TYPE_CAPE
                = create("EQUIP_TOOLTIP_TYPE_CAPE", "Cape (Armor)");
            public static readonly string TOOLTIP_TYPE_RING
                = create("EQUIP_TOOLTIP_TYPE_RING", "Ring (Armor)");

            public static readonly string FORMAT_TOOLTIP_TYPE_WEAPON
                = create("FORMAT_EQUIP_TOOLTIP_TYPE_WEAPON", "{0} (Weapon)");
            public static readonly string TOOLTIP_TYPE_SHIELD
                = create("EQUIP_TOOLTIP_TYPE_SHIELD", "Shield");
            public static readonly string STATS_DAMAGE
                = create("EQUIP_TOOLTIP_STATS_DAMAGE", "Damage");
            public static readonly string STATS_BASE_DAMAGE
                = create("EQUIP_TOOLTIP_STATS_BASE_DAMAGE", "Base Damage");
            public static readonly string FORMAT_STATS_DAMAGE_UNSCALED
                = create("FORMAT_EQUIP_STATS_DAMAGE_UNSCALED", "({0} - {1}) Damage");
            public static readonly string FORMAT_STATS_BLOCK_THRESHOLD
                = create("FORMAT_EQUIP_STATS_BLOCK_THRESHOLD", "Block threshold: {0} damage");


            public static readonly string FORMAT_WEAPON_DAMAGE_TYPE
                = create("FORMAT_EQUIP_STATS_WEAPON_DAMAGE_TYPE", "{0} Weapon");
            public static readonly string FORMAT_WEAPON_TRANSMUTE_TYPE
                = create("FORMAT_EQUIP_STATS_WEAPON_TRASMUTE_TYPE", "Damage Transmute: {0}");

            public static readonly string COMPARE
                = create("EQUIP_TOOLTIP_COMPARE", "Compare");






            // Stat
            public static string statDisplayKey(string stat)
            {
                return "ITEM_STAT_DISPLAY_" + KeyUtil.Normalize(Regex.Replace(stat, "[A-Z]", x => $"_{x}"));
            }


            public static readonly string STAT_DISPLAY_DEFENSE
                    = create(statDisplayKey("defense"), "Defense");

            public static readonly string STAT_DISPLAY_MAGIC_DEFENSE
                    = create(statDisplayKey("magicDefense"), "Mgk. Defense");

            public static readonly string STAT_DISPLAY_MAX_HEALTH
                    = create(statDisplayKey("maxHealth"), "Max Health");

            public static readonly string STAT_DISPLAY_MAX_MANA
                    = create(statDisplayKey("maxMana"), "Max Mana");

            public static readonly string STAT_DISPLAY_MAX_STAMINA
                    = create(statDisplayKey("maxStamina"), "Max Stamina");

            public static readonly string STAT_DISPLAY_ATTACK_POWER
                    = create(statDisplayKey("attackPower"), "Attack Power");

            public static readonly string STAT_DISPLAY_MAGIC_POWER
                    = create(statDisplayKey("magicPower"), "Mgk. Power");

            public static readonly string STAT_DISPLAY_DEX_POWER
                    = create(statDisplayKey("dexPower"), "Dex Power");

            public static readonly string STAT_DISPLAY_CRITICAL
                    = create(statDisplayKey("critical"), "Phys. Critical");

            public static readonly string STAT_DISPLAY_MAGIC_CRITICAL
                    = create(statDisplayKey("magicCritical"), "Mgk. Critical");

            public static readonly string STAT_DISPLAY_EVASION
                    = create(statDisplayKey("evasion"), "Evasion");

            public static readonly string STAT_DISPLAY_RESIST_FIRE
                    = create(statDisplayKey("resistFire"), "Fire Resist");

            public static readonly string STAT_DISPLAY_RESIST_WATER
                    = create(statDisplayKey("resistWater"), "Water Resist");

            public static readonly string STAT_DISPLAY_RESIST_NATURE
                    = create(statDisplayKey("resistNature"), "Nature Resist");

            public static readonly string STAT_DISPLAY_RESIST_EARTH
                    = create(statDisplayKey("resistEarth"), "Earth Resist");

            public static readonly string STAT_DISPLAY_RESIST_HOLY
                    = create(statDisplayKey("resistHoly"), "Holy Resist");

            public static readonly string STAT_DISPLAY_RESIST_SHADOW
                    = create(statDisplayKey("resistShadow"), "Shadow Resist");

        }

    }
}