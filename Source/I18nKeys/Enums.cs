
using Localyssation.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Localyssation
{
    internal static partial class I18nKeys
    {
        internal static class Enums
        {
            internal static void Init() { }
            private static IDictionary<TranslationKey, string> CreateEnumKeys<TEnum>(
                Func<TEnum, TranslationKey> keyOverride = null,
                Func<TEnum, string> valueOverride = null,
                IDictionary<TEnum, string> valueOverrideDict = null
                )
                where TEnum : Enum
            {
                TranslationKey defaultGetString(TEnum item)
                {
                    var GetForAsset = typeof(KeyUtil).GetMethods()
                        .Where(m => m.Name == "GetForAsset")
                        .FirstOrDefault(m =>
                        {
                            var parameter = m.GetParameters();
                            return parameter.Length == 1 && parameter[0].ParameterType == typeof(TEnum);
                        });
                    return (TranslationKey)GetForAsset.Invoke(null, new object[] { item });
                }
                if (keyOverride == null)
                {
                    keyOverride = defaultGetString;
                }
                if (valueOverride == null)
                {
                    valueOverride = item => item.ToString();
                }
                var _valueOverride = valueOverride;
                if (valueOverrideDict != null)
                {
                    _valueOverride = item => valueOverrideDict.TryGetValue(item, out var result) ? result : valueOverride(item);
                }
                return Enum.GetValues(typeof(TEnum)).OfType<TEnum>().ToDictionary(
                    item => Create(keyOverride(item).ToString(), _valueOverride(item)),
                    _valueOverride
                );
            }


            public static readonly IDictionary<TranslationKey, string> ITEM_RARITY
                = CreateEnumKeys<ItemRarity>();
            public static readonly IDictionary<TranslationKey, string> DAMAGE_TYPE
                = CreateEnumKeys<DamageType>();

            public static readonly IDictionary<TranslationKey, string> SKILL_CONTROL_TYPE
                = CreateEnumKeys<SkillControlType>();
            public static readonly IDictionary<TranslationKey, string> COMBAT_COLLIDER_TYPE
                = CreateEnumKeys<CombatColliderType>();
            public static readonly IDictionary<TranslationKey, string> ITEM_TYPE
                = CreateEnumKeys<ItemType>();
            public static readonly IDictionary<TranslationKey, string> ZONE_TYPE
                = CreateEnumKeys<ZoneType>();
            public static readonly IDictionary<TranslationKey, string> SKILL_TOOLTIP_REQUIREMENT
                = CreateEnumKeys<SkillToolTipRequirement>(valueOverride: skillToolTipRequirement => skillToolTipRequirement.ToString().ToLower());
            public static readonly IDictionary<TranslationKey, string> QUEST_SUB_TYPE
                = CreateEnumKeys<QuestSubType>(
                    valueOverride: questSubType => "",
                    valueOverrideDict: new Dictionary<QuestSubType, string>() {
                        { QuestSubType.CLASS, "Class Tome" },
                        { QuestSubType.SKILL, "Skill Scroll" }
                    });

            private static readonly IDictionary<ShopTab, string> __SHOP_TABS = new Dictionary<ShopTab, string>()
            {
                { ShopTab.GEAR, "Equipment" },
                { ShopTab.CONSUMABLE, "Consumables" },
                { ShopTab.TRADE, "Trade Items" },
                { ShopTab.BUYBACK, "Sold Items" }
            };
            public static readonly IDictionary<TranslationKey, string> SHOP_TAB
                = __SHOP_TABS.ToDictionary(
                    kv => Create(KeyUtil.GetForAsset(kv.Key).ToString(), kv.Value),
                    kv => kv.Value
                );

        }

    }
}