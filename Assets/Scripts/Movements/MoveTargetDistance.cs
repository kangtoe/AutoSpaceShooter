using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// target과 일정거리 유지
public class MoveTargetDistance : MoveStandard
{
    [Header("MoveTargetDistance")]
    public float distance = 3;
    public float relaxZone = 1;

    FindTarget findTarget;
    Transform Target => findTarget?.Target;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        findTarget = GetComponent<FindTarget>();

    }

    // Update is called once per frame
    new void FixedUpdate()
    {
        if (!Target)
        {
            rbody.AddForce(transform.up * movePower * rbody.mass);
            return;
        }

        float currentDist = (Target.transform.position - transform.position).magnitude;

        if (currentDist > distance + relaxZone)
        {
            // 너무 먼 거리 -> 접근
            rbody.AddForce(transform.up * movePower);
        }
        else if (currentDist < distance - relaxZone)
        {
            // 너무 가까운 거리 -> 후퇴
            rbody.AddForce(transform.up * movePower * -1);
        }
        else
        {            
            // 적정 거리 -> 우측 이동
            rbody.AddForce(transform.right * currentDist * 0.5f);
        }
    }
}
