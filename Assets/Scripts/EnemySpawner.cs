using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpawnInfo
{
    [SerializeField] string desc = "Spawn Desc";

    [Space]
    public GameObject spawnPrefab;

    public float spawnTime;
    public Edge spawnSide;

    [Header("multi spawn")]
    public int count;
    public float spawnInterval;

    [HideInInspector] public float SpawnEndTime => spawnTime + count * spawnInterval;
}

public class EnemySpawner : MonoSingleton<EnemySpawner>
{
    [Header("MUST SET 0 ON SHPPING")]
    [SerializeField]
    float devStartTime;

    [Space]
    [SerializeField]
    GameObject defaultEnemyPrefab;

    [SerializeField]
    List<SpawnInfo> timeSpawnInfoList;    

    [SerializeField]
    List<SpawnInfo> endlessSpawnInfoList;
    List<SpawnInfo> endlessSpawnInfoListOrigin;

    float ElapsedTime => TimeRecordManager.Instance.TimeRecord;
    float spawnEndTime;

    private void Start()
    {
        for (int i = timeSpawnInfoList.Count - 1; i >= 0; i--)
        {
            SpawnInfo spawnInfo = timeSpawnInfoList[i];
            if (devStartTime > spawnInfo.spawnTime)
            {
                timeSpawnInfoList.RemoveAt(i);
            }
        }

        endlessSpawnInfoListOrigin = new(endlessSpawnInfoList);

    }

    private void Update()
    {
        for (int i = timeSpawnInfoList.Count - 1; i >= 0; i--)
        {
            SpawnInfo spawnInfo = timeSpawnInfoList[i];
            if (ElapsedTime + devStartTime > spawnInfo.spawnTime)
            {
                ObjectSpawner.Instance.SpawnObjects(spawnInfo.spawnPrefab, spawnInfo.spawnSide, spawnInfo.count, spawnInfo.spawnInterval);
                timeSpawnInfoList.RemoveAt(i);

                if (timeSpawnInfoList.Count == 0)
                {
                    spawnEndTime = ElapsedTime;
                } 
            }
        }

        if (timeSpawnInfoList.Count > 0) return;
        for (int i = endlessSpawnInfoList.Count - 1; i >= 0; i--)
        {
            SpawnInfo spawnInfo = endlessSpawnInfoList[i];
            if (ElapsedTime - spawnEndTime > spawnInfo.spawnTime)
            {
                ObjectSpawner.Instance.SpawnObjects(spawnInfo.spawnPrefab, spawnInfo.spawnSide, spawnInfo.count, spawnInfo.spawnInterval);
                endlessSpawnInfoList.RemoveAt(i);

                if (endlessSpawnInfoList.Count == 0)
                {
                    spawnEndTime = ElapsedTime;
                    endlessSpawnInfoList = new(endlessSpawnInfoListOrigin);
                } 
            }
        }
    }

    public void AddSpawnInfo()
    {
        float SpawnEndTime = GetSpawnEndTime();

        SpawnInfo info = new();
        info.spawnPrefab = defaultEnemyPrefab;
        info.spawnTime = GetSpawnEndTime();
        info.count = 1;
        info.spawnSide = Edge.Random;
        timeSpawnInfoList.Add(info);
    }

    public float GetSpawnEndTime()
    {
        if (timeSpawnInfoList.Count == 0) return 0;
        return timeSpawnInfoList[timeSpawnInfoList.Count - 1].SpawnEndTime;
    }
}
