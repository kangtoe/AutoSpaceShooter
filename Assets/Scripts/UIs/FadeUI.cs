using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeUI : MonoBehaviour
{
    public float fadeSpeed = 1;
    Graphic ui;

    // Start is called before the first frame update
    void Start()
    {
        ui = GetComponent<Graphic>();
    }

    // Update is called once per frame
    void Update()
    {        
        Color color = ui.color;
        color.a -= Time.deltaTime * fadeSpeed;
        ui.color = color;

        if (ui.color.a < 0.01f) Destroy(gameObject);
    }
}
