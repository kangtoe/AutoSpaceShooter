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
    [SerializeField, ReadOnly] Vector2 baseSize = Vector2.zero;  // 기준 크기 (최대값 100에 해당하는 크기)
    [SerializeField] float sparkDuration = 0.3f;  // 파티클 유지 시간

    float lastValue = -1f;  // 이전 값 (-1은 초기화되지 않았음을 의미)
    float sparkTimer = 0f;  // 파티클 비활성화 타이머

    // 초기화
    public void Initialize()
    {
        if (baseSize == Vector2.zero)
        {            
            baseSize = rectTransform.sizeDelta;
        }
    }

    // 게이지 업데이트 (현재 값, 최대 값)
    public void UpdateGauge(float currentValue, float maxValue)
    {
        UpdateUiSize(maxValue);
        UpdateFillAmount(currentValue, maxValue);        
    }

    void UpdateFillAmount(float current, float max)
    {
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

        float newRatio = max > 0 ? current / max : 0;
        fillImage.fillAmount = newRatio;

        // 값이 변경되었는지 확인 (처음 호출은 제외, 값이 변경된 경우)
        bool isFirstCall = lastValue == -1f;
        bool valueChanged = !isFirstCall && !Mathf.Approximately(current, lastValue);
        lastValue = current;

        // 값이 변경되면 파티클 활성화 및 타이머 갱신
        if (valueChanged)
        {
            sparkTimer = sparkDuration;
            UpdateFillSpark(true);
        }
    }

    void Update()
    {
        // 파티클 비활성화 타이머
        if (sparkTimer > 0f)
        {
            sparkTimer -= Time.deltaTime;
            if (sparkTimer <= 0f)
            {
                UpdateFillSpark(false);
            }
        }
    }

    void UpdateFillSpark(bool emitting)
    {
        if (fillSparkUI == null) return;

        fillSparkUI.SetEmitting(emitting);

        // 게이지 끝 위치 계산 및 전달
        Vector3 fillEndPosition = CalculateFillEndPosition();
        fillSparkUI.MoveToPoint(fillEndPosition);
        fillSparkUI.SetParticleColor(fillImage.color);
    }

    Vector3 CalculateFillEndPosition()
    {
        RectTransform fillRect = fillImage.rectTransform;

        // 월드 코너 좌표 가져오기 (Canvas 스케일 반영됨)
        Vector3[] corners = new Vector3[4];
        fillRect.GetWorldCorners(corners);
        // corners[0] = bottom-left, corners[1] = top-left
        // corners[2] = top-right, corners[3] = bottom-right

        // fillAmount에 따른 끝 위치 계산 (왼쪽에서 오른쪽으로)
        float fillEndX = Mathf.Lerp(corners[0].x, corners[2].x, fillImage.fillAmount);
        float centerY = (corners[0].y + corners[1].y) / 2f;

        return new Vector3(fillEndX, centerY, corners[0].z);
    }

    void UpdateUiSize(float max)
    {
        if (rectTransform == null)
        {
            Debug.LogError($"DynamicGauge ({gameObject.name}): rectTransform is null!", this);
            return;
        } 

        if (baseSize == Vector2.zero)
        {
            baseSize = rectTransform.sizeDelta;
        }

        float sizeMultiplier = max / BASE_MAX_VALUE;
        Vector2 newSize = baseSize;

        if (scaleHorizontally)
            newSize.x *= sizeMultiplier;
        else
            newSize.y *= sizeMultiplier;

        rectTransform.sizeDelta = newSize;
    }
}
