using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StackSystem : MonoBehaviour
{
    [SerializeField]
    float chargeDelay = 1f;
    [SerializeField]
    float rechargeDelay = 1f; // hold charge stack when use
    float lastStackUsed = 0;

    [SerializeField]
    int maxStack = 5;
    float? currStack;

    [SerializeField]
    bool showRatio; // to show fill ratio?

    [HideInInspector]
    public UnityEvent onChangeValue = new UnityEvent();

    public int MaxStack => maxStack;
    public float CurrStack
    {
        get
        {
            if (currStack == null) InitStack();
            return currStack.Value;
        }
    }
    public float CurrStackOnUI => showRatio ? CurrStack : Mathf.Floor(CurrStack);    

    // Start is called before the first frame update
    private void Start()
    {
        onChangeValue.AddListener(delegate
        {
            currStack = Mathf.Clamp(CurrStack, 0, maxStack);
        });
    }

    // Update is called once per frame
    void Update()
    {
        // wait recharge delay
        if (lastStackUsed != 0 && Time.time < lastStackUsed + rechargeDelay) return;

        if (currStack < MaxStack) currStack += Time.deltaTime / chargeDelay;
        onChangeValue.Invoke();
    }

    public void InitStack(bool stackFull = false)
    {
        if (stackFull) currStack = maxStack;
        else currStack = 0;
    }

    public void SetMaxStack(int amount)
    {
        maxStack = amount;
        onChangeValue.Invoke();
    }

    public void SetChargeDelay(float amount)
    {
        chargeDelay = amount;
    }

    public bool UseStack(bool withoutCunsume = false)
    {
        if (withoutCunsume) return true;

        if (CurrStack < 1) return false;

        currStack--;        
        onChangeValue.Invoke();
        lastStackUsed = Time.time;

        return true;
    }
}