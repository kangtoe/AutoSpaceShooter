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
    // StatConfigDatabase 참조
    private static StatConfigDatabase database;

    // 각 스탯의 1회 증가량 (StatConfigDatabase에서 로드됨)
    static Dictionary<UpgradeField, float> IncrementValues = new();

    // 각 스탯의 최대 레벨 (StatConfigDatabase에서 로드됨)
    static Dictionary<UpgradeField, int> MaxLevels = new();

    // 표시 이름 (StatConfigDatabase에서 로드됨)
    static Dictionary<UpgradeField, string> DisplayNames = new();

    /// <summary>
    /// StatConfigDatabase에서 업그레이드 데이터 초기화 (게임 시작 시 호출)
    /// UpgradeManager가 호출
    /// </summary>
    /// <param name="statDatabase">StatConfigDatabase SO</param>
    public static void Initialize(StatConfigDatabase statDatabase)
    {
        if (statDatabase == null)
        {
            Debug.LogError("[UpgradeData] StatConfigDatabase가 할당되지 않았습니다!");
            return;
        }

        database = statDatabase;
        database.Initialize();

        // StatConfigDatabase에서 데이터 추출
        IncrementValues.Clear();
        MaxLevels.Clear();
        DisplayNames.Clear();

        foreach (var config in database.allStats)
        {
            IncrementValues[config.field] = config.incrementPerLevel;
            MaxLevels[config.field] = config.maxLevel;
            DisplayNames[config.field] = config.displayName;
        }

        if (IncrementValues.Count == 0 || MaxLevels.Count == 0 || DisplayNames.Count == 0)
        {
            Debug.LogError("[UpgradeData] StatConfigDatabase 로드 실패!");
            return;
        }

        // StatMetadataRegistry에 DisplayName 병합
        StatMetadataRegistry.MergeDisplayNames(DisplayNames);

        Debug.Log($"[UpgradeData] Initialized with {IncrementValues.Count} upgrades from StatConfigDatabase");
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
        string changeStr = $"{currentStr}{unit} → {nextStr}{unit}";

        return $"{sign}{incrementStr}{unit}\n{changeStr}\nLv.{currentLevel}/{maxLevel}";
    }

    static float GetCurrentValue(UpgradeField field)
    {
        return PlayerStatsManager.Instance?.GetStat(field) ?? 0f;
    }

    static string GetUnit(UpgradeField field)
    {
        return StatMetadataRegistry.Get(field)?.unit ?? "";
    }

    static string FormatValue(float value, UpgradeField field)
    {
        return StatMetadataRegistry.Get(field)?.FormatValue(value) ?? value.ToString();
    }
}
