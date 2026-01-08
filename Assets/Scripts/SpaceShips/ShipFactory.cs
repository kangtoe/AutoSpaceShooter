using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 간격을 두고 오브젝트 스폰
// 스폰 오브젝트 동시에 존재 가능한 갯수 제한
// 격파 시, 한번에 많은 오브젝트 스폰
public class ShipFactory : MonoBehaviour
{
    //public GameObject createEffect;
    public GameObject spawnablePrefab;
    public Transform spawnPoint;

    [Header("Delay")]
    public float spawnStartDelay = 3f; // 생성 시작 대기
    public float spawnInterval = 2f; // 생성 간 딜레이
    float lastSapwnTime = 0;

    [Header("Phsics")]    
    //public float spawnAngle = 90;
    public float spawnPower = 1;

    [Header("Create Count")]
    public int spawnOnDie = 3;
    public int maxSpawnExist = 10;

    [Header("for debug")]
    [SerializeField] List<GameObject> spawnList;

    int CurrObjExist
    {
        get
        {
            // null 요소 제거 // 뒤에서 순회하여 인덱스 변경에 영향 받지 않도록 함
            for (int i = spawnList.Count - 1; i >= 0; i--)
            {
                if (spawnList[i] == null) spawnList.RemoveAt(i);
            }                

            return spawnList.Count;
        }
    }

    private void Start()
    {
        // 사망 시 오브젝트 생성
        Damageable damageable = GetComponent<Damageable>();
        damageable.onDead.AddListener(delegate
        {            
            for (int i = 0; i < spawnOnDie; i++)
            {
                Create();
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnStartDelay > 0)
        {
            spawnStartDelay -= Time.deltaTime;
            return;
        }         

        CreateCheck();
    }

    protected virtual void CreateCheck()
    {
        // 시간 간격 확인
        if (Time.time < lastSapwnTime + spawnInterval) return;

        // 최대 생성 개수 미만
        if (maxSpawnExist <= CurrObjExist) return;

        //Debug.Log("Create");
        lastSapwnTime = Time.time;
        Create();
    }

    public void Create()
    {
        // 생성
        GameObject go = Instantiate(spawnablePrefab, spawnPoint.position, spawnPoint.rotation);

        // 약간의 랜덤한 힘 가하기        
        //float randomAngle = Random.Range(-spawnAngle, spawnAngle); // 무작위 각도
        //Vector2 dir = Quaternion.Euler(0, 0, randomAngle) * transform.up; // 현재 오브젝트 각도 + 무작위 각도만큼 회전        
        Vector2 dir = spawnPoint.up;
        go.GetComponent<Rigidbody2D>().AddForce(dir.normalized * spawnPower, ForceMode2D.Impulse);

        // 시각 효과
        //if (createEffect) Instantiate(createEffect, transform.position, transform.rotation);

        spawnList.Add(go);
    }    
}
