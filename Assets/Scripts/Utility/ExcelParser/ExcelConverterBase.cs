using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;
using UnityEditor;
using UnityEngine;

namespace YEONJI.Kindergarten
{
    public abstract class ExcelConverterBase
    {
        protected DataTable OpenSheet(string excelPath, string sheetName, out Dictionary<string, int> colMap)
        {
            colMap = new Dictionary<string, int>();

            string abs = Path.GetFullPath(excelPath);
            if (!File.Exists(abs))
            {
                Debug.LogError($"엑셀 없음: {abs}");
                return null;
            }

            using (var stream = File.Open(abs, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                DataTable sheet = reader.AsDataSet().Tables[sheetName];
                if (sheet == null)
                {
                    Debug.LogError($"시트 없음: {sheetName}");
                    return null;
                }

                // 0행 헤더 → 이름:인덱스
                DataRow header = sheet.Rows[0];
                for (int c = 0; c < sheet.Columns.Count; c++)
                {
                    string name = header[c]?.ToString()?.Trim();
                    if (!string.IsNullOrEmpty(name)) colMap[name] = c;
                }

                return sheet;
            }
        }

        protected void WriteJson(object data, string outputPath)
        {
            string json = JsonUtility.ToJson(data, true);
            string abs = Path.GetFullPath(outputPath);
            Directory.CreateDirectory(Path.GetDirectoryName(abs));
            File.WriteAllText(abs, json, System.Text.Encoding.UTF8);
            AssetDatabase.Refresh();
        }

        protected string Cell(DataRow row, Dictionary<string, int> colMap, string colName)
        {
            if (!colMap.TryGetValue(colName, out int c)) return string.Empty;
            if (c >= row.ItemArray.Length) return string.Empty;
            return row[c]?.ToString()?.Trim() ?? string.Empty;
        }
        
        protected int ParseInt(string s, int fallback = 0)
        {
            if (string.IsNullOrWhiteSpace(s)) return fallback;
            return (int)ParseFloat(s, fallback);
        }

        protected float ParseFloat(string s, float fallback = 0f)
        {
            if (string.IsNullOrWhiteSpace(s)) return fallback;
            if (float.TryParse(s, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var v)) return v;
            throw new System.FormatException($"숫자가 아님: '{s}'");
        }

        protected Vector2Int ParseVector2Int(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return Vector2Int.zero;
            string inner = s.Trim().TrimStart('(').TrimEnd(')');
            string[] parts = inner.Split(',');
            if (parts.Length != 2) throw new System.FormatException($"좌표 형식 아님: '{s}'");
            return new Vector2Int((int)ParseFloat(parts[0].Trim()), (int)ParseFloat(parts[1].Trim()));
        }

        protected NeedType ParseNeedType(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return NeedType.None;
            string name = StripEnumPrefix(s, "NeedType");
            if (System.Enum.TryParse(name, out NeedType need)) return need;
            throw new System.FormatException($"NeedType 파싱 실패: '{s}'");
        }

        protected ZoneType ParseZoneType(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return ZoneType.None;
            string name = StripEnumPrefix(s, "ZoneType");
            if (System.Enum.TryParse(name, out ZoneType zone)) return zone;
            throw new System.FormatException($"ZoneType 파싱 실패: '{s}'");
        }

        protected string StripEnumPrefix(string s, string prefix)
        {
            s = s.Trim();
            string dotted = prefix + ".";
            return s.StartsWith(dotted) ? s.Substring(dotted.Length) : s;
        }
    }
}
