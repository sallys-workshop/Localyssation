using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Localyssation.Exporter
{
    internal class ScriptableItemExporter : Exporter<ScriptableItem>
    {
        protected override string CreateEnding()
        {
            return "";
        }

        public override string Name()
        {
            return nameof(ScriptableItem);
        }

        protected override string CreateHeader()
        {
            return "|Icon|Key|Name|\n|---|---|---|";
        }

        public override string Serialize(ScriptableItem data)
        {
            return $"|{CreateAndInsertImageAsset(data._itemName, data._itemIcon)}|{KeyUtil.GetForAsset(data)}|{data._itemName}|";
        }
    }
}
