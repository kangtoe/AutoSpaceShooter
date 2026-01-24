# 업그레이드 시스템

> 로그라이크 스타일의 증분 기반 업그레이드 시스템

## 개요

레벨업 시 랜덤으로 제시된 **3가지 업그레이드 중 하나를 선택**합니다. 각 업그레이드는 최대 레벨까지 반복 선택 가능하며, PlayerStats를 통해 중앙 관리됩니다.

---

## 관련 문서

### 이 폴더의 문서
- **[BuildCombinations.md](BuildCombinations.md)** - 빌드 조합 가이드
  - Nova Drift 방식의 암묵적 시너지
  - 4가지 빌드 아키타입 상세 설명
  - 효과적인 업그레이드 조합 추천
  - Gear 시스템 확장 가능성 (TODO)

### 레퍼런스
- [../../References/NovaDrift_BuildSystem.md](../../References/NovaDrift_BuildSystem.md) - Nova Drift 빌드 시스템 분석

### 플레이어 시스템
- [../PlayerStats.md](../PlayerStats.md) - 플레이어 스탯 시스템
- [../PlayerLevelSystem.md](../PlayerLevelSystem.md) - 레벨 시스템

### 구현 파일
- `Assets/Scripts/UpgradeData.cs` - 업그레이드 데이터 정의
- `Assets/Scripts/UpgradeManager.cs` - 업그레이드 선택 로직
- `Assets/Scripts/Player/PlayerStats.cs` - 스탯 중앙 관리

---

## 구현된 업그레이드

| No. | 필드 | 한글 이름 | 증가량 | 최대 레벨 | 초기값 → 최대값 |
|-----|------|----------|--------|----------|----------------|
| 1 | MaxDurability | 최대 내구도 | +50 | 10 | 100 → 600 |
| 2 | MaxShield | 최대 실드 | +50 | 10 | 100 → 600 |
| 3 | ShieldRegenRate | 실드 재생 속도 | +5/초 | 8 | 20 → 60/초 |
| 4 | ShieldRegenDelay | 실드 재생 지연 | -0.2초 | 5 | 2초 → 1초 |
| 5 | MultiShot | 멀티샷 | +1발 | 4 | 1발 → 5발 |
| 6 | OnImpact | 충돌 피해 | +5 | 10 | 10 → 60 |

### 업그레이드 설명

**1~2. 최대 내구도/실드**: 최대치 증가 시 현재 값도 함께 증가

**3. 실드 재생 속도**: 마지막 피해 후 일정 시간 경과 후 자동 재생

**4. 실드 재생 지연**: 재생 시작까지 대기 시간 감소 (음수 증분)

**5. 멀티샷**: 각 FirePoint마다 확산 패턴으로 발사

**6. 충돌 피해**: 적과 충돌 시 적에게 가하는 피해 (플레이어가 받는 피해는 별개)

## 빌드 전략

### 탱커 빌드
**핵심**: #2 최대 실드, #3 실드 재생 속도, #4 실드 재생 지연 감소, #6 충돌 피해

**플레이**: 적과 부딪히면서 싸우는 근접 빌드

### 사격 빌드
**핵심**: #5 멀티샷, #7 연사 속도(TODO), #8 발사체 데미지(TODO)

**플레이**: 원거리에서 압도적인 화력으로 제압

### 충돌 빌드
**핵심**: #6 충돌 피해, #10 이동 속도(TODO), #2 최대 실드

**플레이**: 적에게 돌진하여 충돌로 격파

### 기동 빌드
**핵심**: #10 이동 속도(TODO), #11 회전 속도(TODO), #3 실드 재생

**플레이**: 빠른 움직임으로 적을 회피하며 견제

## 미구현 업그레이드 (Phase 2 우선순위)

### 우선순위 1 (필수 - 빌드 다양성)

| No. | 필드 | 한글 이름 | 증가량 | 최대 레벨 | 설명 |
|-----|------|----------|--------|----------|------|
| 7 | FireRate | 연사 속도 | -0.02초 | 10 | fireDelay 감소 (사격 빌드 핵심) |
| 8 | ProjectileDamage | 발사체 데미지 | +2 | 10 | 발사체 피해량 증가 (사격 빌드) |
| 9 | ProjectileSpeed | 발사체 속도 | +1 | 8 | 발사체 이동 속도 증가 |
| 10 | MoveSpeed | 이동 속도 | +1 | 10 | 플레이어 이동 속도 증가 (기동/충돌 빌드) |
| 11 | RotateSpeed | 회전 속도 | +20°/s | 8 | 플레이어 회전 속도 증가 (기동 빌드) |
| 12 | ImpactResist | 충돌 저항 | +5% | 10 | 충돌 시 받는 피해 감소 (충돌 빌드) |

### 우선순위 2 (강화 - 빌드 깊이)

| No. | 필드 | 한글 이름 | 증가량 | 최대 레벨 | 설명 |
|-----|------|----------|--------|----------|------|
| 13 | Penetration | 관통 | +1 | 5 | 발사체가 적을 관통하는 횟수 (사격 빌드) |
| 14 | ProjectileSize | 발사체 크기 | +10% | 5 | 발사체 크기 및 충돌 범위 증가 |
| 15 | KnockbackPower | 넉백 강도 | +10% | 8 | 충돌/발사체 넉백 효과 증가 (충돌 빌드) |
| 16 | ExpBonus | 경험치 획득량 | +10% | 5 | 적 격파 시 경험치 추가 획득 |
| 17 | DurabilityRegen | 내구도 재생 | +1/초 | 5 | 내구도 자동 재생 (현재 0) |
| 18 | Mass | 질량 | +0.5 | 8 | 질량 증가 (넉백 저항, 충돌 피해 증가) |

### 우선순위 3 (특수 - 유니크 메커니즘)

| No. | 필드 | 한글 이름 | 설명 | 구현 난이도 |
|-----|------|----------|------|------------|
| 19 | CriticalChance | 치명타 확률 | 발사체 치명타 확률 (10% → 50%) | 중 |
| 20 | CriticalDamage | 치명타 피해 | 치명타 데미지 배율 (150% → 250%) | 중 |
| 21 | LifeSteal | 생명력 흡수 | 피해량의 일부를 체력으로 회복 | 중 |
| 22 | Drone | 드론 | 플레이어를 따라다니며 자동 공격하는 드론 소환 | 높음 (DroneMaster 사용) |
| 23 | Shield Overcharge | 실드 과충전 | 실드가 최대치를 초과 가능 (150%까지) | 중 |
| 24 | Afterburner | 애프터버너 | 이동 시 잔상 이펙트 + 충돌 피해 증가 | 중 (AfterImage 사용) |

> **참고**: PlayerStats.cs에 기본 필드들이 이미 정의되어 있습니다. UpgradeData.cs에만 추가하면 바로 사용 가능합니다.
>
> **특수 업그레이드**는 기존 시스템을 확장해야 하므로 Phase 3 고려

## 업그레이드 확장 구현 순서

### 1단계: 우선순위 1 업그레이드 (7-12번)
- 예상 소요: 2-3시간
- 작업:
  1. UpgradeData.cs에 6개 업그레이드 추가
  2. 각 업그레이드 증분값, 최대 레벨, 한글 이름 설정
  3. PlayerStats 필드 연결 (이미 존재)
  4. Unity에서 테스트

### 2단계: 우선순위 2 업그레이드 (13-18번)
- 예상 소요: 3-4시간
- 작업:
  1. UpgradeData.cs에 6개 업그레이드 추가
  2. Penetration은 BulletBase에 로직 추가 필요
  3. KnockbackPower는 Impactable 수정 필요
  4. DurabilityRegen은 Damageable 수정 필요

### 3단계: 우선순위 3 업그레이드 (19-24번)
- Phase 3에서 고려
- 새로운 시스템 필요

## 구현 파일

- `Assets/Scripts/UpgradeData.cs` - 증분값, 최대 레벨, 한글 이름 정의
- `Assets/Scripts/UpgradeManager.cs` - 업그레이드 선택 로직
- `Assets/Scripts/Player/PlayerStats.cs` - 스탯 중앙 관리

---

## 구현 로드맵

### Phase 1: 기본 업그레이드 확장
**목표**: 빌드 다양성 확보 (우선순위 1 업그레이드)

- [ ] 연사 속도 (FireRate)
- [ ] 발사체 피해 (ProjectileDamage)
- [ ] 발사체 속도 (ProjectileSpeed)
- [ ] 이동 속도 (MoveSpeed)
- [ ] 회전 속도 (RotateSpeed)
- [ ] 충돌 저항 (ImpactResist)

### Phase 2: 암묵적 시너지 설계
**목표**: Nova Drift 방식의 자연스러운 조합 효과 - 자세한 내용은 [BuildCombinations.md](BuildCombinations.md) 참조

- [ ] 곱연산 효과 밸런싱 (멀티샷 × 연사 속도 × 발사체 피해)
- [ ] 빌드별 효과적인 조합 검증 (탱커/사격/충돌/기동)
- [ ] 약점 보완 메커니즘 (예: 탱커의 낮은 화력)
- [ ] 하이브리드 빌드 가능성 테스트

### Phase 3: 고급 업그레이드
**목표**: 빌드 완성도 (우선순위 2 업그레이드)

- [ ] 관통 (Penetration)
- [ ] 발사체 크기 (ProjectileSize)
- [ ] 넉백 강도 (KnockbackPower)
- [ ] 경험치 보너스 (ExpBonus)
- [ ] 내구도 재생 (DurabilityRegen)
- [ ] 질량 (Mass)

### Phase 4: 특수 업그레이드 (선택적)
**목표**: 유니크한 플레이 경험 (우선순위 3 업그레이드)

- [ ] 치명타 시스템 (CriticalChance, CriticalDamage)
- [ ] 생명력 흡수 (LifeSteal)
- [ ] 드론 시스템 (Drone)
- [ ] 실드 과충전 (ShieldOvercharge)
- [ ] 애프터버너 (Afterburner)
