using UnityEngine;

/// <summary>
/// 발사체 데이터 구조체 (외부 데이터 확장 가능)
/// </summary>
[System.Serializable]
public struct BulletData
{
    [Header("=== Basic Stats ===")]
    [Tooltip("발사체 데미지")]
    public int damage;
    [Tooltip("발사체 속도")]
    public float speed;

    [Header("=== Shooting ===")]
    [Tooltip("발사체 충돌 시 넉백 힘 (Knockback force)")]
    public float knockback;
    [Tooltip("폭발 피해 비율 (Explosion damage ratio: 0 = disabled, 0.2 = 20% of original damage)")]
    [Range(0f, 1f)]
    public float explosionDamageRatio;
    [Tooltip("폭발 반지름 (Explosion radius: 0 = no explosion)")]
    public float explosionRadius;

    [Header("=== Aircraft Collision ===")]
    [Tooltip("기체 충돌 시 처리")]
    public int impact;

    [Header("=== Behavior ===")]
    [Tooltip("충돌 시 발사체 파괴")]
    public bool destroyOnHit;
    [Tooltip("다른 발사체와 충돌 시 제거")]
    public bool removeOtherBullet;

    [Header("=== Effects & Sounds ===")]
    [Tooltip("충돌 이펙트 프리팹")]
    public GameObject hitEffect;
    [Tooltip("충돌 사운드")]
    public AudioClip onHitSound;

    [Header("=== Transformation System ===")]
    [Tooltip("변형 시작 시간 (초, -1 = 비활성화)")]
    public float transformStartTime;
    [Tooltip("변형 진행 시간 (초, -1 = 비활성화)")]
    public float transformDuration;

    [Space(5)]
    [Tooltip("시작 크기 배율")]
    [Range(0f, 10f)]
    public float scaleMultiplierStart;
    [Tooltip("끝 크기 배율")]
    [Range(0f, 10f)]
    public float scaleMultiplierEnd;

    [Space(5)]
    [Tooltip("시작 투명도")]
    [Range(0f, 1f)]
    public float alphaStart;
    [Tooltip("끝 투명도")]
    [Range(0f, 1f)]
    public float alphaEnd;
    [Tooltip("페이드 시작 비율 (0~1)")]
    [Range(0f, 1f)]
    public float alphaFadeStartRatio;

    [Space(5)]
    [Tooltip("끝 데미지 배율 (1.0 = 변화없음)")]
    [Range(0f, 2f)]
    public float damageMultiplierEnd;
    [Tooltip("시작 속도 배율")]
    [Range(0f, 2f)]
    public float speedMultiplierStart;
    [Tooltip("끝 속도 배율")]
    [Range(0f, 2f)]
    public float speedMultiplierEnd;

    [Space(5)]
    [Tooltip("충돌 판정 비활성화 진행도 (1.0 = 비활성화 안함)")]
    [Range(0f, 1f)]
    public float colliderDisableThreshold;
    [Tooltip("변형 완료 시 파괴")]
    public bool destroyOnComplete;
    [Tooltip("시작 회전 속도 (도/초)")]
    public float rotationSpeedStart;
    [Tooltip("끝 회전 속도 (도/초)")]
    public float rotationSpeedEnd;

    [Header("=== Homing System ===")]
    [Tooltip("추적 회전 속도 (0 = 추적 안함)")]
    public float homingTurnSpeed;
    [Tooltip("추적 시작 시간 (초)")]
    public float homingStartTime;
    [Tooltip("최대 추적 시간 (-1 = 무제한)")]
    public float homingMaxDuration;

    /// <summary>
    /// BulletData 생성자 - 모든 매개변수는 기본값을 가짐
    /// </summary>
    public BulletData(
        // Basic Stats
        int damage = 10,
        float speed = 10f,

        // Shooting
        float knockback = 5f,
        float explosionDamageRatio = 0f,
        float explosionRadius = 2f,

        // Aircraft Collision
        int impact = 5,

        // Behavior
        bool destroyOnHit = true,
        bool removeOtherBullet = false,

        // Effects & Sounds
        GameObject hitEffect = null,
        AudioClip onHitSound = null,

        // Transformation System
        float transformStartTime = 0f,
        float transformDuration = -1f,
        float scaleMultiplierStart = 1f,
        float scaleMultiplierEnd = 1f,
        float alphaStart = 1f,
        float alphaEnd = 1f,
        float alphaFadeStartRatio = 0f,
        float damageMultiplierEnd = 1f,
        float speedMultiplierStart = 1f,
        float speedMultiplierEnd = 1f,
        float colliderDisableThreshold = 1f,
        bool destroyOnComplete = true,
        float rotationSpeedStart = 0f,
        float rotationSpeedEnd = 0f,

        // Homing System
        float homingTurnSpeed = 0f,
        float homingStartTime = 0f,
        float homingMaxDuration = -1f
    )
    {
        // Basic Stats
        this.damage = damage;
        this.speed = speed;

        // Shooting
        this.knockback = knockback;
        this.explosionDamageRatio = explosionDamageRatio;
        this.explosionRadius = explosionRadius;

        // Aircraft Collision
        this.impact = impact;

        // Behavior
        this.destroyOnHit = destroyOnHit;
        this.removeOtherBullet = removeOtherBullet;

        // Effects & Sounds
        this.hitEffect = hitEffect;
        this.onHitSound = onHitSound;

        // Transformation System
        this.transformStartTime = transformStartTime;
        this.transformDuration = transformDuration;
        this.scaleMultiplierStart = scaleMultiplierStart;
        this.scaleMultiplierEnd = scaleMultiplierEnd;
        this.alphaStart = alphaStart;
        this.alphaEnd = alphaEnd;
        this.alphaFadeStartRatio = alphaFadeStartRatio;
        this.damageMultiplierEnd = damageMultiplierEnd;
        this.speedMultiplierStart = speedMultiplierStart;
        this.speedMultiplierEnd = speedMultiplierEnd;
        this.colliderDisableThreshold = colliderDisableThreshold;
        this.destroyOnComplete = destroyOnComplete;
        this.rotationSpeedStart = rotationSpeedStart;
        this.rotationSpeedEnd = rotationSpeedEnd;

        // Homing System
        this.homingTurnSpeed = homingTurnSpeed;
        this.homingStartTime = homingStartTime;
        this.homingMaxDuration = homingMaxDuration;
    }

    /// <summary>
    /// BulletData의 특정 필드만 변경합니다. null인 파라미터는 변경하지 않습니다.
    /// </summary>
    public void SetStats(
        int? damage = null,
        float? speed = null,
        float? knockback = null,
        float? homingPower = null,
        float? explosionDamageRatio = null,
        float? explosionRadius = null,
        GameObject hitEffect = null,
        AudioClip onHitSound = null)
    {
        if (damage.HasValue) this.damage = damage.Value;
        if (speed.HasValue) this.speed = speed.Value;
        if (knockback.HasValue) this.knockback = knockback.Value;
        if (homingPower.HasValue) this.homingTurnSpeed = homingPower.Value;
        if (explosionDamageRatio.HasValue) this.explosionDamageRatio = explosionDamageRatio.Value;
        if (explosionRadius.HasValue) this.explosionRadius = explosionRadius.Value;
        if (hitEffect != null) this.hitEffect = hitEffect;
        if (onHitSound != null) this.onHitSound = onHitSound;
    }
}
