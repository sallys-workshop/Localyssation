
using Localyssation.Util;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Localyssation
{
    internal static partial class I18nKeys
    {
        internal static class Enums
        {
            internal static void init() { }
            private static ImmutableDictionary<string, string> CreateEnumKeys<TEnum>(Func<TEnum, string> keyOverride = null, Func<TEnum, string> valueOverride = null)
                where TEnum : Enum
            {
                string defaultGetString(TEnum item)
                {
                    var GetForAsset = typeof(KeyUtil).GetMethods()
                        .Where(m => m.Name == "GetForAsset")
                        .FirstOrDefault(m =>
                        {
                            var parameter = m.GetParameters();
                            return parameter.Length == 1 && parameter[0].ParameterType == typeof(TEnum);
                        });
                    return (string)GetForAsset.Invoke(null, new object[] { item });
                }
                if (keyOverride == null)
                {
                    keyOverride = defaultGetString;
                }
                if (valueOverride == null)
                {
                    valueOverride = item => item.ToString();
                }
                return Enum.GetValues(typeof(TEnum)).OfType<TEnum>().ToImmutableDictionary(
                    item => Create(keyOverride(item), valueOverride(item)),
                    valueOverride
                );
            }


            public static readonly ImmutableDictionary<string, string> ITEM_RARITY
                = CreateEnumKeys<ItemRarity>();
            public static readonly ImmutableDictionary<string, string> DAMAGE_TYPE
                = CreateEnumKeys<DamageType>();

            public static readonly ImmutableDictionary<string, string> SKILL_CONTROL_TYPE
                = CreateEnumKeys<SkillControlType>();
            public static readonly ImmutableDictionary<string, string> COMBAT_COLLIDER_TYPE
                = CreateEnumKeys<CombatColliderType>();
            public static readonly ImmutableDictionary<string, string> ITEM_TYPE
                = CreateEnumKeys<ItemType>();
            public static readonly ImmutableDictionary<string, string> ZONE_TYPE
                = CreateEnumKeys<ZoneType>();
            public static readonly ImmutableDictionary<string, string> SKILL_TOOLTIP_REQUIREMENT
                = CreateEnumKeys<SkillToolTipRequirement>(valueOverride: skillToolTipRequirement => skillToolTipRequirement.ToString().ToLower());

            private static IDictionary<ShopTab, string> __SHOP_TABS = new Dictionary<ShopTab, string>()
            {
                { ShopTab.GEAR, "Equipment" },
                { ShopTab.CONSUMABLE, "Consumables" },
                { ShopTab.TRADE, "Trade Items" },
                { ShopTab.BUYBACK, "Sold Items" }
            };
            public static readonly ImmutableDictionary<string, string> SHOP_TAB
                = __SHOP_TABS.ToImmutableDictionary(
                    kv => Create(KeyUtil.GetForAsset(kv.Key)),
                    kv => kv.Value
                );

        }

    }
}