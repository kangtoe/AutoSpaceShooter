using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FindTarget))]
public class RotateToTarget : MonoBehaviour
{
    public float turnSpeed = 1;

    FindTarget findTarget;
    Transform Target => findTarget?.Target;

    // Start is called before the first frame update
    void Start()
    {
        findTarget = GetComponent<FindTarget>();

        // 개체 별 회전시간에 약간의 차이를 둔다.
        float minMult = 0.9f;
        float maxMult = 1.1f;        
        turnSpeed *= Random.Range(minMult, maxMult);
    }

    void Update()
    {        
        if (Target) RotateTo(Target.position, turnSpeed);
    }


    void RotateTo(Vector3 targetPos, float _rotateSpeed)
    {
        if (!Target) return;

        Vector3 dir = targetPos - transform.position;

        // 회전각 구하기
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        // 회전 값 구하기
        Quaternion quat = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        // 회전 보간
        transform.rotation = Quaternion.Lerp(transform.rotation, quat, Time.deltaTime * _rotateSpeed);
    }
}
