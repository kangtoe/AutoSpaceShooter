using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 모든 업그레이드 설정을 하나의 에셋으로 관리하는 데이터베이스 ScriptableObject
/// Inspector에서 직접 편집 가능
/// </summary>
[CreateAssetMenu(fileName = "UpgradeDatabase", menuName = "Game/Upgrade Database", order = 1)]
public class UpgradeDatabase : ScriptableObject
{
    [Header("=== 모든 업그레이드 설정 ===")]
    [Tooltip("모든 업그레이드 정보 리스트 (Inspector에서 직접 편집 가능)")]
    public List<UpgradeConfig> allUpgrades = new List<UpgradeConfig>();

    // 빠른 조회를 위한 Dictionary (런타임에 생성)
    private Dictionary<string, UpgradeConfig> lookup;

    /// <summary>
    /// 데이터베이스 초기화 (게임 시작 시 호출)
    /// </summary>
    public void Initialize()
    {
        lookup = new Dictionary<string, UpgradeConfig>();

        foreach (var upgrade in allUpgrades)
        {
            if (upgrade == null)
            {
                Debug.LogWarning("[UpgradeDatabase] null upgrade config in list!");
                continue;
            }

            if (string.IsNullOrEmpty(upgrade.upgradeId))
            {
                Debug.LogError($"[UpgradeDatabase] Upgrade has empty ID: {upgrade.displayName}");
                continue;
            }

            if (lookup.ContainsKey(upgrade.upgradeId))
            {
                Debug.LogError($"[UpgradeDatabase] Duplicate upgrade ID: {upgrade.upgradeId}");
                continue;
            }

            lookup[upgrade.upgradeId] = upgrade;
        }

        Debug.Log($"[UpgradeDatabase] Initialized with {lookup.Count} upgrade configs");
    }

    /// <summary>
    /// 특정 업그레이드 조회
    /// </summary>
    public UpgradeConfig GetUpgrade(string upgradeId)
    {
        // 초기화되지 않았으면 자동 초기화
        if (lookup == null || lookup.Count == 0)
        {
            Initialize();
        }

        if (lookup.ContainsKey(upgradeId))
        {
            return lookup[upgradeId];
        }

        Debug.LogError($"[UpgradeDatabase] Upgrade not found: {upgradeId}");
        return null;
    }

    /// <summary>
    /// 특정 타입의 모든 업그레이드 조회
    /// </summary>
    public List<UpgradeConfig> GetUpgradesByType(UpgradeType upgradeType)
    {
        if (lookup == null || lookup.Count == 0)
        {
            Initialize();
        }

        return allUpgrades.Where(u => u != null && u.upgradeType == upgradeType).ToList();
    }

    /// <summary>
    /// 모든 유니크 업그레이드 조회
    /// </summary>
    public List<UpgradeConfig> GetUniqueUpgrades()
    {
        if (lookup == null || lookup.Count == 0)
        {
            Initialize();
        }

        return allUpgrades.Where(u => u != null && u.isUnique).ToList();
    }

    /// <summary>
    /// 모든 일반 업그레이드 조회
    /// </summary>
    public List<UpgradeConfig> GetGeneralUpgrades()
    {
        if (lookup == null || lookup.Count == 0)
        {
            Initialize();
        }

        return allUpgrades.Where(u => u != null && !u.isUnique).ToList();
    }

    /// <summary>
    /// 특정 타입의 유니크 업그레이드 조회
    /// </summary>
    public List<UpgradeConfig> GetUniqueUpgradesByType(UpgradeType upgradeType)
    {
        if (lookup == null || lookup.Count == 0)
        {
            Initialize();
        }

        return allUpgrades.Where(u => u != null && u.isUnique && u.upgradeType == upgradeType).ToList();
    }

    /// <summary>
    /// 특정 타입의 일반 업그레이드 조회
    /// </summary>
    public List<UpgradeConfig> GetGeneralUpgradesByType(UpgradeType upgradeType)
    {
        if (lookup == null || lookup.Count == 0)
        {
            Initialize();
        }

        return allUpgrades.Where(u => u != null && !u.isUnique && u.upgradeType == upgradeType).ToList();
    }

#if UNITY_EDITOR
    /// <summary>
    /// 에디터에서 업그레이드 ID로 정렬
    /// </summary>
    [ContextMenu("Sort by Upgrade ID")]
    private void SortById()
    {
        allUpgrades.Sort((a, b) => string.Compare(a.upgradeId, b.upgradeId, System.StringComparison.Ordinal));
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[UpgradeDatabase] Sorted {allUpgrades.Count} upgrades by ID");
    }

    /// <summary>
    /// 에디터에서 업그레이드 타입으로 정렬
    /// </summary>
    [ContextMenu("Sort by Type")]
    private void SortByType()
    {
        allUpgrades.Sort((a, b) =>
        {
            int typeCompare = a.upgradeType.CompareTo(b.upgradeType);
            if (typeCompare != 0) return typeCompare;

            // 같은 타입이면 유니크 먼저
            int uniqueCompare = b.isUnique.CompareTo(a.isUnique);
            if (uniqueCompare != 0) return uniqueCompare;

            // 그 다음은 ID 순
            return string.Compare(a.upgradeId, b.upgradeId, System.StringComparison.Ordinal);
        });
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[UpgradeDatabase] Sorted {allUpgrades.Count} upgrades by type");
    }

    /// <summary>
    /// 에디터에서 검증
    /// </summary>
    private void OnValidate()
    {
        // 중복 ID 체크
        HashSet<string> seenIds = new HashSet<string>();
        foreach (var upgrade in allUpgrades)
        {
            if (upgrade != null && !string.IsNullOrEmpty(upgrade.upgradeId))
            {
                if (seenIds.Contains(upgrade.upgradeId))
                {
                    Debug.LogError($"[UpgradeDatabase] Duplicate upgrade ID detected: {upgrade.upgradeId}", this);
                }
                seenIds.Add(upgrade.upgradeId);
            }
        }

        // 각 업그레이드 유효성 검증
        foreach (var upgrade in allUpgrades)
        {
            if (upgrade != null)
            {
                if (!upgrade.Validate(out string errorMessage))
                {
                    Debug.LogError($"[UpgradeDatabase] Validation failed for {upgrade.upgradeId}: {errorMessage}", this);
                }
            }
        }
    }
#endif
}
