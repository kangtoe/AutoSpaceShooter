using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRecordManager : MonoSingleton<TimeRecordManager>
{
    float timeRecord;
    public float TimeRecord => timeRecord;

    bool canCount = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canCount)
        {
            timeRecord += Time.deltaTime;
            UiManager.Instance.SetTimeRecordText((int)TimeRecord);
        }
    }

    public void SetActiveCount(bool _active)
    {
        canCount = _active;
    }
}
