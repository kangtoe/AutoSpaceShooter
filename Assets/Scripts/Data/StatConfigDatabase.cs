using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모든 스탯 설정을 하나의 에셋으로 관리하는 데이터베이스 ScriptableObject
/// CSV 대신 Inspector에서 직접 편집 가능
/// </summary>
[CreateAssetMenu(fileName = "StatConfigDatabase", menuName = "Game/Stat Config Database", order = 0)]
public class StatConfigDatabase : ScriptableObject
{
    [Header("=== 모든 스탯 설정 ===")]
    [Tooltip("모든 스탯 정보 리스트 (Inspector에서 직접 편집 가능)")]
    public List<StatConfig> allStats = new List<StatConfig>();

    // 빠른 조회를 위한 Dictionary (런타임에 생성)
    private Dictionary<UpgradeField, StatConfig> lookup;

    /// <summary>
    /// 데이터베이스 초기화 (게임 시작 시 호출)
    /// </summary>
    public void Initialize()
    {
        lookup = new Dictionary<UpgradeField, StatConfig>();

        foreach (var stat in allStats)
        {
            if (stat == null)
            {
                Debug.LogWarning("[StatConfigDatabase] null stat config in list!");
                continue;
            }

            if (lookup.ContainsKey(stat.field))
            {
                Debug.LogError($"[StatConfigDatabase] Duplicate field: {stat.field}");
                continue;
            }

            lookup[stat.field] = stat;
        }

        Debug.Log($"[StatConfigDatabase] Initialized with {lookup.Count} stat configs");
    }

    /// <summary>
    /// 특정 스탯 설정 조회
    /// </summary>
    public StatConfig GetConfig(UpgradeField field)
    {
        // 초기화되지 않았으면 자동 초기화
        if (lookup == null || lookup.Count == 0)
        {
            Initialize();
        }

        if (lookup.ContainsKey(field))
        {
            return lookup[field];
        }

        Debug.LogError($"[StatConfigDatabase] Config not found: {field}");
        return null;
    }

    /// <summary>
    /// 특정 카테고리의 모든 스탯 조회
    /// </summary>
    public List<StatConfig> GetStatsByCategory(string category)
    {
        if (lookup == null || lookup.Count == 0)
        {
            Initialize();
        }

        List<StatConfig> result = new List<StatConfig>();
        foreach (var stat in allStats)
        {
            if (stat != null && stat.category == category)
            {
                result.Add(stat);
            }
        }

        return result;
    }

#if UNITY_EDITOR
    /// <summary>
    /// 에디터에서 UpgradeField enum 순서로 정렬
    /// </summary>
    [ContextMenu("Sort by UpgradeField")]
    private void SortByField()
    {
        allStats.Sort((a, b) => a.field.CompareTo(b.field));
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[StatConfigDatabase] Sorted {allStats.Count} stats by field");
    }

    /// <summary>
    /// 에디터에서 검증
    /// </summary>
    private void OnValidate()
    {
        // 중복 체크
        HashSet<UpgradeField> seen = new HashSet<UpgradeField>();
        foreach (var stat in allStats)
        {
            if (stat != null)
            {
                if (seen.Contains(stat.field))
                {
                    Debug.LogError($"[StatConfigDatabase] Duplicate field detected: {stat.field}", this);
                }
                seen.Add(stat.field);
            }
        }
    }
#endif
}
