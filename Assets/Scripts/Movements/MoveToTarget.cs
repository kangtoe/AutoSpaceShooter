using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 현재 방향과 관련없이 target으로 이동하도록 힘을 가한다 
[RequireComponent(typeof(FindTarget))]
public class MoveToTarget : MonoBehaviour
{
    public float movePower = 1f;
    Rigidbody2D rbody;

    FindTarget findTarget;
    Transform Target => findTarget?.Target;

    void Start()
    {
        findTarget = GetComponent<FindTarget>();
        rbody = GetComponent<Rigidbody2D>();        
    }

    private void FixedUpdate()
    {
        Vector2 dir = transform.up;
        if (Target) dir = (Target.position - transform.position).normalized;

        //Debug.Log(dir);
        rbody.AddForce(dir * movePower * rbody.mass);
    }
}
