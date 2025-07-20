using System;
using System.Collections.Generic;
using System.Linq;

namespace Localyssation
{
    public static class TSVUtil
    {
        public static string makeTsv(List<List<string>> rows, string delimeter = "\t")
        {
            var rowStrs = new List<string>();
            List<string> headerRow = null;
            for (var i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                for (var j = 0; j < row.Count; j++)
                {
                    row[j] = row[j].Replace("\n", "\\n").Replace("\t", "\\t");
                }

                var rowStr = string.Join(delimeter, row);

                if (headerRow == null)
                {
                    headerRow = row;
                }
                else if (headerRow.Count != row.Count)
                {
                    Localyssation.logger.LogError($"Row {i} has {row.Count} columns, which does not match header column count (${headerRow.Count})");
                    Localyssation.logger.LogError($"Row content: {rowStr}");
                    return string.Join(delimeter, headerRow);
                }
                rowStrs.Add(rowStr);
            }
            return string.Join("\n", rowStrs);
        }

        public static List<List<string>> parseTsv(string tsv, string delimeter = "\t")
        {
            var parsedTsv = new List<List<string>>();
            List<string> headerRow = null;
            var splitTsv = tsv.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < splitTsv.Length; i++)
            {
                var rowStr = splitTsv[i];
                if (rowStr.EndsWith("\r")) rowStr = rowStr.Substring(0, rowStr.Length - 2); // convert CRLF to LF

                var row = new List<string>(Split(rowStr, delimeter));
                for (var j = 0; j < row.Count; j++)
                {
                    row[j] = row[j].Replace("\\n", "\n").Replace("\\t", "\t");
                }

                if (headerRow == null)
                {
                    headerRow = row;
                }
                else if (headerRow.Count != row.Count)
                {
                    Localyssation.logger.LogError($"Row {i} has {row.Count} columns, which does not match header column count (${headerRow.Count})");
                    Localyssation.logger.LogError($"Row content: {rowStr}");
                    return new List<List<string>>() { headerRow };
                }
                parsedTsv.Add(row);
            }
            return parsedTsv;
        }

        public static List<Dictionary<string, string>> parseTsvWithHeaders(string tsv, string delimeter = "\t")
        {
            var parsedTsv = parseTsv(tsv, delimeter);
            var withHeaders = new List<Dictionary<string, string>>();
            if (parsedTsv.Count <= 0) return withHeaders;

            var headerRow = parsedTsv[0];
            for (var i = 1; i < parsedTsv.Count; i++)
            {
                var dict = parsedTsv[i]
                    .Select((x, y) => new KeyValuePair<string, string>(headerRow[y], x))
                    .ToDictionary(x => x.Key, x => x.Value);
                withHeaders.Add(dict);
            }
            return withHeaders;
        }

        public static List<string> Split(string str, string delimeter)
        {
            var result = new List<string>();

            var delimeterIsEscape = delimeter.StartsWith("\\");

            var splitStartIndex = 0;
            var searchIndex = 0;
            while (true)
            {
                var delimIndex = str.IndexOf(delimeter, searchIndex);
                if (delimIndex == -1)
                {
                    result.Add(str.Substring(splitStartIndex, str.Length - splitStartIndex));
                    break;
                }

                searchIndex = delimIndex + delimeter.Length;
                if (!delimeterIsEscape || delimIndex > 0 && str[delimIndex - 1] != '\\')
                {
                    result.Add(str.Substring(splitStartIndex, delimIndex - splitStartIndex));
                    splitStartIndex = searchIndex;
                }

                if (searchIndex >= str.Length)
                {
                    result.Add(str.Substring(splitStartIndex, str.Length - splitStartIndex));
                    break;
                }
            }

            return result;
        }
    }

}
