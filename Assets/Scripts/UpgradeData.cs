using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    // 각 스탯의 1회 증가량
    static Dictionary<UpgradeField, float> IncrementValues = new()
    {
        // 생존
        { UpgradeField.MaxDurability, 50 },      // +50
        { UpgradeField.MaxShield, 50 },          // +50
        { UpgradeField.ShieldRegenRate, 5 },     // +5/초
        { UpgradeField.ShieldRegenDelay, -0.2f }, // -0.2초

        // 사격
        { UpgradeField.MultiShot, 1 },           // +1발
        // TODO: 추가 구현 필요
        // { UpgradeField.FireRate, -0.02f },       // -0.02초 (연사 속도 증가)
        // { UpgradeField.ProjectileDamage, 2 },   // +2
        // { UpgradeField.ProjectileSpeed, 1 },    // +1

        // 충돌
        { UpgradeField.OnImpact, 5 },            // +5
        // { UpgradeField.ImpactResist, 0.05f },   // +5%

        // 이동
        // { UpgradeField.MoveSpeed, 1 },          // +1
        // { UpgradeField.RotateSpeed, 20 },       // +20 deg/s
    };

    // 각 스탯의 최대 레벨 (선택 가능 횟수)
    static Dictionary<UpgradeField, int> MaxLevels = new()
    {
        // 생존
        { UpgradeField.MaxDurability, 10 },      // 최대 10회 → 100 + 500 = 600
        { UpgradeField.MaxShield, 10 },          // 최대 10회 → 100 + 500 = 600
        { UpgradeField.ShieldRegenRate, 8 },     // 최대 8회 → 20 + 40 = 60/초
        { UpgradeField.ShieldRegenDelay, 5 },    // 최대 5회 → 2 - 1 = 1초

        // 사격
        { UpgradeField.MultiShot, 4 },           // 최대 4회 → 1 + 4 = 5발

        // 충돌
        { UpgradeField.OnImpact, 10 },           // 최대 10회 → 10 + 50 = 60
    };

    // 한글 표시 이름
    static Dictionary<UpgradeField, string> DisplayNames = new()
    {
        // 생존
        { UpgradeField.MaxDurability, "최대 내구도" },
        { UpgradeField.MaxShield, "최대 실드" },
        { UpgradeField.ShieldRegenRate, "실드 재생 속도" },
        { UpgradeField.ShieldRegenDelay, "실드 재생 지연" },

        // 사격
        { UpgradeField.MultiShot, "멀티샷" },

        // 충돌
        { UpgradeField.OnImpact, "충돌 피해" },
    };

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
        int actualCount = UnityEngine.Mathf.Min(count, availableFields.Count);
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
