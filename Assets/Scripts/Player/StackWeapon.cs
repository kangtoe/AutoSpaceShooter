using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 일정 시간 경과시, 스택을 쌓는다
// 스택 사용 가능 여부를 판단
// 외부에서 스택을 사용할 수 있도록 기능 제공
// 스택을 사용하여 Shooter에서 사격을 가한다.

[RequireComponent(typeof(ShooterBase))]
public class StackWeapon : StackSystem
{
    [SerializeField]
    ShooterBase useShooter;

    new public bool UseStack(bool withoutCunsume = false)
    {
        bool used = base.UseStack(withoutCunsume);
        
        if (used) useShooter.TryFire(true);
        
        return used;
    }
}
