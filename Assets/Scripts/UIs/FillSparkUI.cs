using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class FillSparkUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] ParticleSystem sparkParticle;

    ParticleSystem.EmissionModule emission;
    float originalEmissionRate;
    bool isInitialized;
    RectTransform sparkRectTransform;  // ParticleSystem의 RectTransform

    void Awake()
    {
        Initialize();
    }

    public void SetEmitting(bool emitting)
    {
        if (!isInitialized) return;

        emission.rateOverTime = emitting ? originalEmissionRate : 0;
    }

    public void MoveToPoint(Vector3 worldPosition)
    {
        if (!isInitialized) return;

        sparkRectTransform.position = worldPosition;
    }

    public void SetParticleColor(Color color)
    {
        if (!isInitialized) return;

        var main = sparkParticle.main;
        main.startColor = color;
    }

    void Initialize()
    {
        if (isInitialized) return;

        if (sparkParticle == null)
        {
            Debug.LogError($"FillSparkUI ({gameObject.name}): sparkParticle is null!", this);
            return;
        }

        sparkRectTransform = sparkParticle.GetComponent<RectTransform>();
        if (sparkRectTransform == null)
        {
            Debug.LogError($"FillSparkUI ({gameObject.name}): sparkParticle doesn't have RectTransform!", this);
            return;
        }

        emission = sparkParticle.emission;
        originalEmissionRate = emission.rateOverTime.constant;
        isInitialized = true;

        SetEmitting(false);
    }
}

