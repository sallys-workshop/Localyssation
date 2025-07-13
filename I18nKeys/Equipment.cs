
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
            public static readonly string FORMAT_STATS_DAMAGE_SCALED
                = create("FORMAT_EQUIP_STATS_DAMAGE_SCALED", "<color=#c5e384>({0} - {1})</color> Damage");
            public static readonly string FORMAT_STATS_DAMAGE_SCALED_POWERFUL
                = create("FORMAT_EQUIP_STATS_DAMAGE_SCALED_POWERFUL", "<color=#efcc00>({0} - {1})</color> Damage");
            public static readonly string FORMAT_STATS_DAMAGE_COMPARE_BASE
                = create("FORMAT_EQUIP_STATS_DAMAGE_COMPARE_BASE", "\n<color=grey>(Base Damage: {0} - {1})</color>");
            public static readonly string FORMAT_STATS_DAMAGE_UNSCALED
                = create("FORMAT_EQUIP_STATS_DAMAGE_UNSCALED", "({0} - {1}) Damage");
            public static readonly string FORMAT_STATS_BLOCK_THRESHOLD
                = create("FORMAT_EQUIP_STATS_BLOCK_THRESHOLD", "Block threshold: {0} damage");
        }

    }
}