using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UpgradeButtonUI : MonoBehaviour
{
    Button button;
    public Button Button
    {
        get
        {
            if (!button) button = GetComponent<Button>();
            return button;
        }
    }    

    [SerializeField] Text title;
    [SerializeField] Text desc;

    public void SetDesc(string str)
    {
        desc.text = str;
    }

    public void SetTitle(string str)
    {
        title.text = str;
    }
}
