using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateByVelocity : MonoBehaviour
{
    public float rotateMult = 1;
    Rigidbody2D rbody;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponentInParent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        float vel = rbody.linearVelocity.magnitude;
        transform.Rotate(0, 0, vel * rotateMult);
    }

}
