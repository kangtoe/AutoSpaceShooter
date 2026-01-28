using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 추적 발사체
[RequireComponent(typeof(RotateToTarget))]
public class BulletChase : BulletBase
{
    FindTarget findTarget;
    FindTarget FindTarget
    {
        get
        {
            if(!findTarget) findTarget = GetComponent<FindTarget>();
            findTarget.targetLayer = targetLayer;
            return findTarget;
        }
    }

    Transform Target => FindTarget.Target;

    // Note: 조기 폭발 로직 제거됨. 충돌 판정은 BulletBase의 일반 로직 사용
    // hitEffectRadius가 설정되어 있으면 충돌 시 범위 데미지 발생

    // Update is called once per frame
    void FixedUpdate()
    {
        float powerMult = 1;
        if (Target) powerMult -= (GetAngleToTarget(Target) / 180);
        RBody.AddForce(transform.up * movePower * powerMult);
    }

    float GetAngleToTarget(Transform target)
    {
        Vector2 directionToTarget = target.position - transform.position;
        Vector2 currentDirection = transform.up;

        return Vector2.Angle(currentDirection, directionToTarget);
    }
}
