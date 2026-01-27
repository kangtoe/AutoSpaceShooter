using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerStats.csv 로더
/// 플레이어 스탯의 메타데이터(기본값, 단위, 타입 등)를 로드
/// </summary>
public static class PlayerStatsLoader
{
    private const string LOADER_NAME = "PlayerStatsLoader";

    /// <summary>
    /// CSV 파일에서 플레이어 스탯 메타데이터 로드
    /// </summary>
    /// <param name="csvAsset">PlayerStats.csv TextAsset</param>
    /// <returns>스탯 메타데이터 Dictionary</returns>
    public static Dictionary<UpgradeField, StatMetadata> LoadFromCsv(TextAsset csvAsset)
    {
        var metadata = new Dictionary<UpgradeField, StatMetadata>();

        List<Dictionary<string, string>> rows = CsvReader.LoadCsvRows(csvAsset, LOADER_NAME);
        if (rows.Count == 0)
        {
            return metadata;
        }

        foreach (var row in rows)
        {
            // Field 파싱
            if (!row.ContainsKey("Field") || string.IsNullOrEmpty(row["Field"]))
            {
                Debug.LogWarning($"[{LOADER_NAME}] Row missing 'Field' column");
                continue;
            }

            string fieldName = row["Field"];
            if (!System.Enum.TryParse<UpgradeField>(fieldName, out UpgradeField field))
            {
                Debug.LogWarning($"[{LOADER_NAME}] Unknown field: {fieldName}");
                continue;
            }

            // DefaultValue 파싱
            if (!row.ContainsKey("DefaultValue") || !float.TryParse(row["DefaultValue"], out float defaultValue))
            {
                Debug.LogWarning($"[{LOADER_NAME}] Invalid DefaultValue for {fieldName}");
                continue;
            }

            // Unit 파싱
            string unit = row.ContainsKey("Unit") ? row["Unit"] : "";

            // IsInteger 파싱
            bool isInteger = false;
            if (row.ContainsKey("IsInteger") && !string.IsNullOrEmpty(row["IsInteger"]))
            {
                isInteger = row["IsInteger"].ToLower() == "true";
            }

            // Category 파싱
            string category = row.ContainsKey("Category") ? row["Category"] : "Other";

            // Description 파싱 (선택사항)
            string description = row.ContainsKey("Description") ? row["Description"] : "";

            // 메타데이터 생성
            metadata[field] = new StatMetadata
            {
                field = field,
                displayName = fieldName, // 한글 이름은 Upgrades.csv에서 가져옴
                defaultValue = defaultValue,
                unit = unit,
                isInteger = isInteger,
                category = category
            };

            Debug.Log($"[{LOADER_NAME}] Loaded {fieldName}: DefaultValue={defaultValue}, Unit={unit}, IsInteger={isInteger}");
        }

        CsvReader.LogLoadComplete(metadata.Count, "player stat entries", LOADER_NAME);
        return metadata;
    }
}
