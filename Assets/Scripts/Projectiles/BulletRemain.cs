using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 탄환이 멈추면 삭제
public class BulletRemain : BulletBase
{
    public float remainVelocity = 0.2f;

    private void LateUpdate()
    {                
        if (RBody.linearVelocity.magnitude <= remainVelocity)
        {
            OnHitDestory();
        }
    }
}
