public enum Edge
{
    Undefined = 0,
    Up,
    Down,
    Right,
    Left,
    Random = 99,
}

public enum GameState
{ 
    Undefinded = 0,
    OnTitle,
    OnCombat,
    OnPaused,
    OnUpgrade,
    GameOver,
    GameClear,
}

public enum SoundType
{
    Bgm,
    Sfx
}

public enum UpgradeType
{
    Ship,
    Shooter,
    Missle,
    Pulse,
    EmergencyProtocol
}

public enum UpgradeField
{
    // 생존
    MaxDurability,       // 최대 내구도
    MaxShield,           // 최대 실드
    ShieldRegenRate,     // 실드 재생 속도
    ShieldRegenDelay,    // 실드 재생 지연 (감소)
    DurabilityRegenRate, // 내구도 재생 속도
    DurabilityRegenDelay,// 내구도 재생 지연

    // 사격
    FireRate,         // 연사 속도 (fireDelay 감소)
    ProjectileDamage, // 발사체 피해
    ProjectileSpeed,  // 발사체 속도
    ProjectileSize,   // 발사체 크기
    MultiShot,        // 멀티샷
    Spread,           // 분산도 (각도)
    HomingPower,      // 유도 성능 (0: 없음)
    ExplosionDamageRatio, // 폭발 피해 비율 (0: 비활성화, 0.2: 20% 폭발)

    // 충돌
    OnImpact,         // 충돌 피해 (공격)
    ImpactResist,     // 충돌 피해 (자신) 감소

    // 이동
    MoveSpeed,        // 이동 속도
    RotateSpeed,      // 회전 속도
    Mass,             // 기체 질량
}

/// <summary>
/// 업그레이드가 특정 스탯에 미치는 영향
/// </summary>
[System.Serializable]
public struct StatModifier
{
    public UpgradeField field;      // 영향받는 스탯
    public float valuePerLevel;     // 레벨당 증가량
}