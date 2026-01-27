using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 각 스탯의 메타데이터 정의
/// </summary>
[System.Serializable]
public class StatMetadata
{
    public UpgradeField field;
    public string displayName;
    public float defaultValue;
    public string unit;
    public bool isInteger;
    public string category;

    /// <summary>
    /// 값을 포맷팅하여 문자열로 반환
    /// </summary>
    public string FormatValue(float value)
    {
        if (isInteger)
            return ((int)value).ToString();

        if (value % 1 != 0)
            return value.ToString("F1");

        return ((int)value).ToString();
    }
}

/// <summary>
/// 모든 스탯의 메타데이터를 중앙 관리하는 레지스트리
/// </summary>
public static class StatMetadataRegistry
{
    private static Dictionary<UpgradeField, StatMetadata> registry;

    /// <summary>
    /// PlayerStats.csv에서 메타데이터 초기화
    /// PlayerStatsManager가 호출
    /// </summary>
    public static void InitializeFromPlayerStats(Dictionary<UpgradeField, StatMetadata> metadata)
    {
        registry = metadata;
        Debug.Log($"[StatMetadataRegistry] Initialized {registry.Count} stat metadata from PlayerStats.csv");
    }

    /// <summary>
    /// Upgrades.csv에서 DisplayName 병합
    /// UpgradeManager가 호출
    /// </summary>
    public static void MergeDisplayNames(Dictionary<UpgradeField, string> displayNames)
    {
        if (registry == null)
        {
            Debug.LogError("[StatMetadataRegistry] Registry not initialized! Call InitializeFromPlayerStats first.");
            return;
        }

        foreach (var kvp in displayNames)
        {
            if (registry.ContainsKey(kvp.Key))
            {
                registry[kvp.Key].displayName = kvp.Value;
            }
        }

        Debug.Log($"[StatMetadataRegistry] Merged display names from Upgrades.csv");
    }

    /// <summary>
    /// 특정 스탯의 메타데이터 조회
    /// </summary>
    public static StatMetadata Get(UpgradeField field)
    {
        if (registry != null && registry.ContainsKey(field))
            return registry[field];

        Debug.LogError($"[StatMetadataRegistry] Metadata not found: {field}");
        return null;
    }

    /// <summary>
    /// 레지스트리가 초기화되었는지 확인
    /// </summary>
    public static bool IsInitialized()
    {
        return registry != null && registry.Count > 0;
    }
}
