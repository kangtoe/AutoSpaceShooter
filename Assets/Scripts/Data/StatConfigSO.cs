using UnityEngine;

/// <summary>
/// 단일 스탯의 모든 정보를 담는 직렬화 가능한 클래스
/// PlayerStats.csv와 Upgrades.csv의 정보를 통합
/// </summary>
[System.Serializable]
public class StatConfig
{
    [Header("=== 기본 정보 ===")]
    [Tooltip("스탯 필드 (enum)")]
    public UpgradeField field;

    [Tooltip("표시 이름 (한글)")]
    public string displayName;

    [Tooltip("카테고리 (Survival, Shooting, Impact, Mobility)")]
    public string category;

    [Header("=== 기본값 및 타입 ===")]
    [Tooltip("기본값")]
    public float defaultValue;

    [Tooltip("단위 (/s, s, ° 등)")]
    public string unit;

    [Tooltip("정수 타입 여부")]
    public bool isInteger;

    [Header("=== 추가 정보 (선택) ===")]
    [Tooltip("스탯 설명")]
    [TextArea(2, 4)]
    public string description;

    [Tooltip("스탯 아이콘 (UI용, 추후 확장)")]
    public Sprite icon;

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

    /// <summary>
    /// StatMetadata로 변환 (기존 코드 호환성)
    /// </summary>
    public StatMetadata ToMetadata()
    {
        return new StatMetadata
        {
            field = field,
            displayName = displayName,
            defaultValue = defaultValue,
            unit = unit,
            isInteger = isInteger,
            category = category
        };
    }
}
