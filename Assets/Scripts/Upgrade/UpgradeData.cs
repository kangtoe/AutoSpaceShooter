using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeOption
{
    public UpgradeField field;
    public string displayName;
    public string description;
    public float incrementValue;
    public int currentLevel;
    public int maxLevel;

    public UpgradeOption(UpgradeField field, string displayName, string description, float incrementValue, int currentLevel, int maxLevel)
    {
        this.field = field;
        this.displayName = displayName;
        this.description = description;
        this.incrementValue = incrementValue;
        this.currentLevel = currentLevel;
        this.maxLevel = maxLevel;
    }
}

public static class UpgradeData
{
    // 각 스탯의 1회 증가량 (CSV에서 로드됨)
    static Dictionary<UpgradeField, float> IncrementValues = new();

    // 각 스탯의 최대 레벨 (CSV에서 로드됨)
    static Dictionary<UpgradeField, int> MaxLevels = new();

    // 표시 이름 (CSV에서 로드됨)
    static Dictionary<UpgradeField, string> DisplayNames = new();

    /// <summary>
    /// CSV 파일에서 업그레이드 데이터 초기화 (게임 시작 시 호출)
    /// </summary>
    public static void Initialize(TextAsset upgradesCsv)
    {
        if (upgradesCsv == null)
        {
            Debug.LogError("[UpgradeData] Upgrades CSV가 할당되지 않았습니다!");
            return;
        }

        // CSV 로드
        UpgradesLoader.LoadFromCsv(upgradesCsv, out IncrementValues, out MaxLevels, out DisplayNames);

        if (IncrementValues.Count == 0 || MaxLevels.Count == 0 || DisplayNames.Count == 0)
        {
            Debug.LogError("[UpgradeData] CSV 로드 실패!");
            return;
        }

        Debug.Log($"[UpgradeData] Initialized with {IncrementValues.Count} upgrades from CSV");
    }

    public static float GetIncrement(UpgradeField field)
    {
        if (IncrementValues.ContainsKey(field))
            return IncrementValues[field];
        return 0;
    }

    public static int GetMaxLevel(UpgradeField field)
    {
        if (MaxLevels.ContainsKey(field))
            return MaxLevels[field];
        return 1;
    }

    public static string GetDisplayName(UpgradeField field)
    {
        if (DisplayNames.ContainsKey(field))
            return DisplayNames[field];
        return field.ToString();
    }

    public static List<UpgradeOption> GetRandomUpgradeOptions(int count, Dictionary<UpgradeField, int> currentLevels)
    {
        // 모든 가능한 업그레이드 필드 가져오기
        List<UpgradeField> availableFields = IncrementValues.Keys
            .Where(field => {
                int currentLevel = currentLevels.ContainsKey(field) ? currentLevels[field] : 0;
                return currentLevel < GetMaxLevel(field);
            })
            .ToList();

        // 랜덤 셔플
        System.Random rng = new System.Random();
        availableFields = availableFields.OrderBy(x => rng.Next()).ToList();

        // count만큼 선택
        int actualCount = Mathf.Min(count, availableFields.Count);
        List<UpgradeOption> options = new List<UpgradeOption>();

        for (int i = 0; i < actualCount; i++)
        {
            UpgradeField field = availableFields[i];
            int currentLevel = currentLevels.ContainsKey(field) ? currentLevels[field] : 0;

            string description = GetUpgradeDescription(field, currentLevel);

            options.Add(new UpgradeOption(
                field,
                GetDisplayName(field),
                description,
                GetIncrement(field),
                currentLevel,
                GetMaxLevel(field)
            ));
        }

        return options;
    }

    static string GetUpgradeDescription(UpgradeField field, int currentLevel)
    {
        float increment = GetIncrement(field);
        int maxLevel = GetMaxLevel(field);

        // 현재 값과 다음 값 가져오기
        float currentValue = GetCurrentValue(field);
        float nextValue = currentValue + increment;

        // 증가량 표시
        string sign = increment > 0 ? "+" : "";
        string incrementStr = FormatValue(increment, field);
        string unit = GetUnit(field);

        // 현재 값 → 다음 값 표시
        string currentStr = FormatValue(currentValue, field);
        string nextStr = FormatValue(nextValue, field);
        string changeStr = $"({currentStr}{unit} → {nextStr}{unit})";

        return $"{sign}{incrementStr}{unit}\n{changeStr}\nLv.{currentLevel}/{maxLevel}";
    }

    static float GetCurrentValue(UpgradeField field)
    {
        var stats = PlayerStats.Instance;
        if (stats == null) return 0;

        switch (field)
        {
            case UpgradeField.MaxDurability: return stats.maxDurability;
            case UpgradeField.MaxShield: return stats.maxShield;
            case UpgradeField.ShieldRegenRate: return stats.shieldRegenRate;
            case UpgradeField.ShieldRegenDelay: return stats.shieldRegenDelay;
            case UpgradeField.MultiShot: return stats.multiShot;
            case UpgradeField.FireRate: return stats.fireRate;
            case UpgradeField.ProjectileDamage: return stats.projectileDamage;
            case UpgradeField.ProjectileSpeed: return stats.projectileSpeed;
            case UpgradeField.OnImpact: return stats.onImpact;
            case UpgradeField.ImpactResist: return stats.impactResist;
            case UpgradeField.MoveSpeed: return stats.moveSpeed;
            case UpgradeField.RotateSpeed: return stats.rotateSpeed;
            default: return 0;
        }
    }

    static string GetUnit(UpgradeField field)
    {
        switch (field)
        {
            case UpgradeField.ShieldRegenRate: return "/초";
            case UpgradeField.ShieldRegenDelay: return "초";
            case UpgradeField.FireRate: return "초";
            case UpgradeField.MultiShot: return "발";
            case UpgradeField.ImpactResist: return "%";
            case UpgradeField.RotateSpeed: return "°/s";
            default: return "";
        }
    }

    static string FormatValue(float value, UpgradeField field)
    {
        // 정수형 필드
        if (field == UpgradeField.MultiShot ||
            field == UpgradeField.ProjectileDamage ||
            field == UpgradeField.MaxDurability ||
            field == UpgradeField.MaxShield)
        {
            return ((int)value).ToString();
        }

        // 소수점이 있는 경우
        if (value % 1 != 0)
            return value.ToString("F1");

        return ((int)value).ToString();
    }
}
