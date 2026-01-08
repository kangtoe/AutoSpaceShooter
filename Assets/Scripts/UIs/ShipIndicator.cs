using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class ShipIndicator : MonoBehaviour
{
    [SerializeField] float edgeCheckRange = 20;
    [SerializeField] float indicateRange = 80;

    Image image;
    RectTransform rectTf;

    private void Start()
    {
        image = GetComponent<Image>();
        rectTf = GetComponent<RectTransform>();
    }

    void Update()
    {
        Check();        
    }

    void Check()
    {
        Transform target = GameManager.Instance.PlayerShip?.transform;
        if (!target)
        {
            image.enabled = false;
            return;
        }

        Vector3 screenPos = Camera.main.WorldToScreenPoint(target.position);
        if (screenPos.x <= edgeCheckRange || screenPos.x >= Screen.width - edgeCheckRange || screenPos.y <= edgeCheckRange || screenPos.y >= Screen.height - edgeCheckRange)
        {
            image.enabled = true;

            // pos
            float x = Mathf.Clamp(screenPos.x, indicateRange, Screen.width - indicateRange);
            float y = Mathf.Clamp(screenPos.y, indicateRange, Screen.height - indicateRange);
            rectTf.position = new Vector2(x, y);
            
            // rot
            Vector2 dir = Camera.main.WorldToScreenPoint(target.position) - rectTf.position;            
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rectTf.rotation = Quaternion.Euler(0f, 0f, angle + 90);
        }
        else
        {
            image.enabled = false;
        }
    }
}
