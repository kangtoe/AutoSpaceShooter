using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EnemySpawnTimes.csv 파일을 로드하여 EnemyTimeRange 배열로 변환
/// </summary>
public static class EnemySpawnTimesLoader
{
    private const string LOADER_NAME = "EnemySpawnTimesLoader";
    private static readonly string[] REQUIRED_COLUMNS = { "Name", "SpawnTime", "DespawnTime" };

    /// <summary>
    /// CSV 파일에서 시간 정보를 로드하여 프리팹 리스트와 매칭
    /// </summary>
    /// <param name="csvAsset">EnemySpawnTimes.csv TextAsset</param>
    /// <param name="prefabList">에디터에서 설정한 적 프리팹 배열</param>
    /// <returns>시간 정보가 설정된 EnemyTimeRange 배열</returns>
    public static EnemyTimeRange[] LoadFromCsv(TextAsset csvAsset, EnemyShip[] prefabList)
    {
        if (prefabList == null || prefabList.Length == 0)
        {
            Debug.LogError($"[{LOADER_NAME}] Prefab list is empty! Please set enemy prefabs in Inspector.");
            return new EnemyTimeRange[0];
        }

        // 프리팹 이름 -> EnemyShip 매핑 생성
        Dictionary<string, EnemyShip> prefabLookup = new();
        foreach (var prefab in prefabList)
        {
            if (prefab == null)
            {
                Debug.LogWarning($"[{LOADER_NAME}] Null prefab found in prefab list, skipping");
                continue;
            }

            // 프리팹 이름에서 "Enemy_" 접두사 제거
            string prefabName = prefab.name.Replace("Enemy_", "");
            prefabLookup[prefabName] = prefab;
        }

        List<Dictionary<string, string>> rows = CsvReader.LoadCsvRows(csvAsset, LOADER_NAME);
        if (rows.Count == 0)
        {
            Debug.LogWarning($"[{LOADER_NAME}] No rows loaded from CSV");
            return new EnemyTimeRange[0];
        }

        List<EnemyTimeRange> ranges = new List<EnemyTimeRange>();

        foreach (var row in rows)
        {
            // 필수 컬럼 확인
            if (!CsvReader.ValidateRequiredColumns(row, REQUIRED_COLUMNS, LOADER_NAME))
            {
                continue;
            }

            string enemyName = row["Name"];

            // SpawnTime 파싱
            if (!CsvReader.TryParseFloat(row["SpawnTime"], "SpawnTime", LOADER_NAME, out float spawnTime))
            {
                continue;
            }

            // DespawnTime 파싱 (-1이면 무한대)
            float despawnTime;
            if (row["DespawnTime"] == "-1")
            {
                despawnTime = float.MaxValue;
            }
            else if (!CsvReader.TryParseFloat(row["DespawnTime"], "DespawnTime", LOADER_NAME, out despawnTime))
            {
                continue;
            }

            // 프리팹 리스트에서 찾기
            if (!prefabLookup.TryGetValue(enemyName, out EnemyShip prefab))
            {
                Debug.LogWarning($"[{LOADER_NAME}] Prefab not found in list for '{enemyName}'. Please add it to TimeBasedSpawnManager.");
                continue;
            }

            // EnemyTimeRange 생성
            EnemyTimeRange range = new(prefab, spawnTime, despawnTime);
            ranges.Add(range);

            Debug.Log($"[{LOADER_NAME}] Loaded {enemyName}: {spawnTime}s ~ {despawnTime}s");
        }

        CsvReader.LogLoadComplete(ranges.Count, "enemy time ranges", LOADER_NAME);
        return ranges.ToArray();
    }
}
