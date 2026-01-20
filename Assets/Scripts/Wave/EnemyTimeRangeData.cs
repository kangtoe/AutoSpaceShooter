using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 각 적의 스폰 가능한 시간 범위를 정의하고 관리하는 정적 클래스
/// 시간 기반 스폰 시스템에서 사용
/// </summary>
public static class EnemyTimeRangeData
{
    /// <summary>
    /// 적 이름 -> (spawnTimeMin, spawnTimeMax) 매핑
    /// spawnTimeMin: 스폰 가능한 최소 시간 (작은 값, 게임 후반)
    /// spawnTimeMax: 스폰 가능한 최대 시간 (큰 값, 게임 초반)
    /// 카운트다운 방식: 840초(14:00) → 0초(0:00)
    /// </summary>
    private static Dictionary<string, (float min, float max)> timeRanges = new Dictionary<string, (float, float)>()
    {
        // Light 등급 (비용 20-25)
        { "Enemy_light_child", (660f, 840f) },      // 14:00 ~ 11:00 (3분)
        { "Enemy_light_kido", (630f, 810f) },       // 13:30 ~ 10:30 (3분)
        { "Enemy_light_thunder", (600f, 780f) },    // 13:00 ~ 10:00 (3분)

        // Light+ 등급 (비용 40)
        { "Enemy_light_shield", (540f, 750f) },     // 12:30 ~ 9:00 (3.5분)

        // Mid 등급 (비용 80-100)
        { "Enemy_mid_Ghost", (420f, 720f) },        // 12:00 ~ 7:00 (5분)
        { "Enemy_mid_Hornet", (390f, 690f) },       // 11:30 ~ 6:30 (5분)
        { "Enemy_mid_master", (360f, 660f) },       // 11:00 ~ 6:00 (5분)

        // Mid+ 등급 (비용 120-150)
        { "Enemy_mid_Knight", (300f, 630f) },       // 10:30 ~ 5:00 (5.5분)
        { "Enemy_mid_sniper", (270f, 600f) },       // 10:00 ~ 4:30 (5.5분)
        { "Enemy_mid_tank", (240f, 570f) },         // 9:30 ~ 4:00 (5.5분)
        { "Enemy_mid_Spiral", (210f, 540f) },       // 9:00 ~ 3:30 (5.5분)

        // Heavy 등급 (비용 350-400)
        { "Enemy_heavy_mother", (60f, 480f) },      // 8:00 ~ 1:00 (7분)
        { "Enemy_heavy_Gunship", (0f, 420f) },      // 7:00 ~ 0:00 (7분)
    };

    /// <summary>
    /// 특정 시간에 해당 적을 스폰할 수 있는지 확인
    /// </summary>
    /// <param name="enemyName">적 프리팹 이름 (예: "Enemy_light_child")</param>
    /// <param name="timeRemaining">남은 시간 (초 단위, 840 → 0)</param>
    /// <returns>스폰 가능 여부</returns>
    public static bool CanSpawnAtTime(string enemyName, float timeRemaining)
    {
        if (!timeRanges.ContainsKey(enemyName))
        {
            Debug.LogWarning($"[EnemyTimeRangeData] Unknown enemy: {enemyName}");
            return false;
        }

        var (min, max) = timeRanges[enemyName];
        return timeRemaining >= min && timeRemaining <= max;
    }

    /// <summary>
    /// 특정 시간에 스폰 가능한 모든 적 목록 반환
    /// </summary>
    /// <param name="timeRemaining">남은 시간 (초 단위, 840 → 0)</param>
    /// <returns>스폰 가능한 적 이름 목록</returns>
    public static List<string> GetSpawnableEnemiesAtTime(float timeRemaining)
    {
        List<string> result = new List<string>();

        foreach (var kvp in timeRanges)
        {
            string enemyName = kvp.Key;
            var (min, max) = kvp.Value;

            if (timeRemaining >= min && timeRemaining <= max)
            {
                result.Add(enemyName);
            }
        }

        return result;
    }

    /// <summary>
    /// 특정 적의 시간 범위 정보 조회
    /// </summary>
    /// <param name="enemyName">적 프리팹 이름</param>
    /// <param name="min">spawnTimeMin (out)</param>
    /// <param name="max">spawnTimeMax (out)</param>
    /// <returns>시간 범위 정보가 존재하는지 여부</returns>
    public static bool GetTimeRange(string enemyName, out float min, out float max)
    {
        if (timeRanges.ContainsKey(enemyName))
        {
            (min, max) = timeRanges[enemyName];
            return true;
        }

        min = 0f;
        max = 0f;
        return false;
    }

    /// <summary>
    /// 등록된 모든 적 이름 목록 반환
    /// </summary>
    public static List<string> GetAllEnemyNames()
    {
        return new List<string>(timeRanges.Keys);
    }

    /// <summary>
    /// 디버그용: 특정 시간대의 스폰 가능 적 수 반환
    /// </summary>
    public static int GetSpawnableCount(float timeRemaining)
    {
        int count = 0;
        foreach (var kvp in timeRanges)
        {
            var (min, max) = kvp.Value;
            if (timeRemaining >= min && timeRemaining <= max)
            {
                count++;
            }
        }
        return count;
    }
}
