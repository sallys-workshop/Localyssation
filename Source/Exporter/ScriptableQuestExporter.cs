using System.Text;

namespace Localyssation.Exporter
{
    internal class ScriptableQuestExporter : Exporter<ScriptableQuest>
    {
        public readonly string _questGiverName;
        public ScriptableQuestExporter(string questGiverName) : base()
        {
            _questGiverName = questGiverName;
        }

        protected override string CreateEnding()
        {
            return "";
        }

        public override string Name()
        {
            return nameof(ScriptableQuest);
        }

        protected override string CreateHeader()
        {
            return new StringBuilder()
                .AppendLine($"# {_questGiverName}")
                .AppendLine("|Quest Name|Quest Type|Quest Subtype|Quest Level|")
                .Append("|----------|----------|-------------|-----------|")
                .ToString();
        }

        public override string Serialize(ScriptableQuest data)
        {
            return $"|{data._questName}|{data._questType.ToString()}|{data._questSubType.ToString()}|{data._questLevel}|";
        }
    }
}
