using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    //public Transform ui;
    //public Transform rotator;

    public float speadIn = 1f;    
    
    public float spreadMin = 0;
    public float spreadMax = 20;    

    [SerializeField]
    float currSpead = 0f;

    [Header("crosshair UI")]
    public RectTransform arr_ur;
    public RectTransform arr_ul;
    public RectTransform arr_dr;
    public RectTransform arr_dl;

    [Header("fill")]
    [SerializeField] Image[] fills;

    void Start()
    {
        //Cursor.visible = false;
        //RotateCross(3);
    }

    // ui 초기화. 무기 교체 시 호출
    public void Init()
    {
        //Debug.Log("Init");        
        //rotator.rotation = Quaternion.Euler(0, 0, 0);
        //currentSpead = 0;
        StopAllCoroutines();
    }

    // weapon에서 호출
    public void SetArrSpreadRatio(float ratio)
    {
        currSpead = Mathf.Lerp(spreadMin, spreadMax, ratio);
        SetArrSpread(currSpead);

        foreach(Image fill in fills)
        {
            fill.fillAmount = ratio;
        }
    }

    void SetArrSpread(float f)
    {
        arr_ur.anchoredPosition = new Vector2(f, f);
        arr_ul.anchoredPosition = new Vector2(-f, f);
        arr_dr.anchoredPosition = new Vector2(f, -f);
        arr_dl.anchoredPosition = new Vector2(-f, -f);
    }

    // time 동안 360도 회전한다
    //public void RotateCross(float time)
    //{
    //    StartCoroutine(RotateCrossCr(time));
    //}

    //IEnumerator RotateCrossCr(float time)
    //{
    //    // 프레임 당 회전할 값 구하기
    //    float frameDeg = 360 / time;

    //    // 매 프레임마다 회전
    //    for (; time > 0; time -= Time.deltaTime)
    //    {
    //        rotator.Rotate(0, 0, frameDeg * Time.deltaTime);
    //        yield return new WaitForEndOfFrame();
    //    }

    //    rotator.rotation = Quaternion.Euler(0, 0, 360);
    //}
}
