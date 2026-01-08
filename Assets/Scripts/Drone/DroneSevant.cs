using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneSevant : MonoBehaviour
{
    public void DelayDestory(float f)
    {
        Vector2 force = Random.insideUnitCircle; // 임의의 힘
        float torque = Random.Range(-1, 1);
        StartCoroutine(DelayDestoryCr(f, force, torque));
    }

    IEnumerator DelayDestoryCr(float delay, Vector2 force, float torque)
    {
        // 드론 마스터와 별도의 물리 계산을 위해 Rigidbody2D 추가
        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.AddForce(force, ForceMode2D.Impulse);
        rb.AddTorque(torque);

        yield return new WaitForSeconds(delay);

        // 즉사에 해당하는 피해
        GetComponent<Damageable>().GetDamaged(float.MaxValue);
    }
}
