using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BrakeSystem : MonoBehaviour
{
    Rigidbody2D rbody;

    [SerializeField] float nomalDrag = 1;
    [SerializeField] float breakDrag = 3;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    public void SetBreak(bool active)
    {
        //Debug.Log("SetBreak " + active);

        if (active) rbody.linearDamping = breakDrag;
        else rbody.linearDamping = nomalDrag;
    }
}
