using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindTarget : MonoBehaviour
{
    public LayerMask targetLayer;
    [SerializeField] float searchRadius = 20;
    [SerializeField][Range(0, 1)] float anglePriority; // 대상 탐색 시, 거리보다 각도를 우선적으로 고려하는 정도
    [SerializeField] bool findAlways = false;

    [Header("for debug")]
    [SerializeField] Transform target;
    public Transform Target => target;

    // GC 최적화를 위한 재사용 배열
    private Collider2D[] colliderBuffer = new Collider2D[50];

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateTargetCr());
    }

    IEnumerator UpdateTargetCr()
    {
        while (true)
        {
            if(!target || findAlways)
            {
                // 타겟 갱신
                Collider2D coll = GetTargetCollider(transform.position, searchRadius, targetLayer);
                if (coll) target = coll.transform;
            }

            // 대기
            yield return new WaitForSeconds(0.2f);
        }
    }

    Collider2D GetTargetCollider(Vector2 pos, float radius, LayerMask layer)
    {
        int count = Physics2D.OverlapCircleNonAlloc(pos, radius, colliderBuffer, layer);
        Collider2D targetColl = null;
        float minWeight = Mathf.Infinity;

        for (int i = 0; i < count; i++)
        {
            Collider2D coll = colliderBuffer[i];

            // 거리 계산
            float dist = Vector3.Distance(transform.position, coll.transform.position);

            // 각도 계산
            Vector2 directionToTarget = coll.transform.position - transform.position;
            Vector2 currentDirection = transform.up;
            float angle = Vector2.Angle(currentDirection, directionToTarget);

            float weight = (dist / searchRadius) * (1 - anglePriority) + (angle / 180) * anglePriority;
            if (weight < minWeight)
            {
                minWeight = weight;
                targetColl = coll;
            }
        }
        return targetColl;
    }

}
