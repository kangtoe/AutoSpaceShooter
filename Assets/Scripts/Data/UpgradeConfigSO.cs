using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 단일 업그레이드의 모든 정보를 담는 직렬화 가능한 클래스
/// </summary>
[System.Serializable]
public class UpgradeConfig
{
    [Header("=== 기본 정보 ===")]
    [Tooltip("고유 ID (예: rapid_fire, spread_shot)")]
    public string upgradeId;

    [Tooltip("표시 이름 (한글)")]
    public string displayName;

    [Tooltip("업그레이드 설명")]
    [TextArea(2, 4)]
    public string description;

    [Tooltip("업그레이드 아이콘")]
    public Sprite icon;

    [Header("=== 분류 ===")]
    [Tooltip("업그레이드 타입 (Ship, Shooter, Missile, Pulse, EmergencyProtocol)")]
    public UpgradeType upgradeType;

    [Tooltip("유니크 업그레이드 여부 (true: 1회 획득, false: 최대 5레벨)")]
    public bool isUnique;

    [Header("=== 레벨 설정 ===")]
    [Tooltip("최대 레벨 (유니크: 1, 일반: 5)")]
    public int maxLevel = 5;

    [Header("=== 스탯 효과 ===")]
    [Tooltip("이 업그레이드가 영향을 주는 스탯 목록")]
    public List<StatModifier> statModifiers = new List<StatModifier>();

    /// <summary>
    /// 특정 레벨에서의 스탯 효과를 계산
    /// </summary>
    public Dictionary<UpgradeField, float> GetStatEffects(int level)
    {
        var effects = new Dictionary<UpgradeField, float>();

        foreach (var modifier in statModifiers)
        {
            effects[modifier.field] = modifier.valuePerLevel * level;
        }

        return effects;
    }

    /// <summary>
    /// 업그레이드 유효성 검증
    /// </summary>
    public bool Validate(out string errorMessage)
    {
        if (string.IsNullOrEmpty(upgradeId))
        {
            errorMessage = "업그레이드 ID가 비어있습니다.";
            return false;
        }

        if (string.IsNullOrEmpty(displayName))
        {
            errorMessage = "표시 이름이 비어있습니다.";
            return false;
        }

        if (maxLevel < 1)
        {
            errorMessage = "최대 레벨은 1 이상이어야 합니다.";
            return false;
        }

        if (isUnique && maxLevel != 1)
        {
            errorMessage = "유니크 업그레이드의 최대 레벨은 1이어야 합니다.";
            return false;
        }

        if (statModifiers == null || statModifiers.Count == 0)
        {
            errorMessage = "최소 1개 이상의 스탯 효과가 필요합니다.";
            return false;
        }

        errorMessage = "";
        return true;
    }
}
