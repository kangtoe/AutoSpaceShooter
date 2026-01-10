using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

// 최대값에 따라 크기가 동적으로 변하는 게이지 컴포넌트
[RequireComponent(typeof(RectTransform))]
public class DynamicGauge : MonoBehaviour
{
    const float BASE_MAX_VALUE = 100f;  // baseSize가 나타내는 기준 최대값

    [Header("References")]
    [SerializeField] Image fillImage;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] FillSparkUI fillSparkUI;  // 게이지 끝 파티클 효과

    [Header("Settings")]
    [SerializeField] bool scaleHorizontally = true;
    [SerializeField] Vector2 baseSize;  // 기준 크기 (최대값 100에 해당하는 크기)

    float lastValue = -1f;  // 이전 값 (-1은 초기화되지 않았음을 의미)

    // 초기화
    public void Initialize()
    {
        if (baseSize == Vector2.zero)
        {
            Debug.LogError($"DynamicGauge ({gameObject.name}): baseSize is Vector2.zero!", this);
            baseSize = rectTransform.sizeDelta;
        }

        // FillSparkUI 초기 비활성화
        if (fillSparkUI != null)
        {
            fillSparkUI.gameObject.SetActive(false);
        }
    }


    // 게이지 업데이트 (현재 값, 최대 값)
    public void UpdateGauge(float currentValue, float maxValue)
    {
        UpdateFillAmount(currentValue, maxValue);
        UpdateUiSize(maxValue);
    }

    void UpdateFillAmount(float current, float max)
    {
        Debug.Log("UpdateFillAmount called with current: " + current + ", max: " + max);

        if (fillImage == null)
        {
            Debug.LogError($"DynamicGauge ({gameObject.name}): gaugeImage is null!", this);
            return;
        }

        if (max <= 0)
        {
            Debug.LogError($"DynamicGauge ({gameObject.name}): maxValue is zero or negative!", this);
            return;
        }

        float newValue = max > 0 ? current / max : 0;

        // 값이 변경되었는지 확인
        bool valueChanged = lastValue >= 0 && Mathf.Abs(newValue - lastValue) > 0.001f;

        fillImage.fillAmount = newValue;
        lastValue = newValue;

        // FillSparkUI 활성화/비활성화 토글 (값이 변경될 때만 활성화)
        if (fillSparkUI != null)
        {
            fillSparkUI.gameObject.SetActive(valueChanged);
        }
    }

    void UpdateUiSize(float max)
    {
        Debug.Log("UpdateSize called with max: " + max);

        if (rectTransform == null)
        {
            Debug.LogError($"DynamicGauge ({gameObject.name}): rectTransform is null!", this);
            return;
        } 

        if (baseSize == Vector2.zero)
        {
            Debug.LogError($"DynamicGauge ({gameObject.name}): baseSize is Vector2.zero!", this);
            return;
        }

        float sizeMultiplier = max / BASE_MAX_VALUE;
        Vector2 newSize = baseSize;

        if (scaleHorizontally)
            newSize.x *= sizeMultiplier;
        else
            newSize.y *= sizeMultiplier;

        rectTransform.sizeDelta = newSize;
    }

        // 현재 RectTransform 크기를 기준 크기로 저장    
    [Button("Save Base Size")]
    void SaveBaseSize()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        baseSize = rectTransform.sizeDelta;
    }

    // 테스트: 크기 2배
    [Button("테스트: 크기 2배")]
    void TestDouble()
    {
        UpdateUiSize(BASE_MAX_VALUE * 2);
    }

    // 테스트: 원래 크기
    [Button("테스트: 원래 크기")]
    void TestReset()
    {
        UpdateUiSize(BASE_MAX_VALUE);
    }

}
