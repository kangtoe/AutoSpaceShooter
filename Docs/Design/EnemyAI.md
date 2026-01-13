# 적 AI 시스템

## 개요

AutoSpaceShooter의 적은 **컴포넌트 조합 방식**으로 다양한 AI 패턴을 구현합니다.
각 적은 `FindTarget`, `MoveStandard`, `RotateToTarget`, `ShooterBase` 등의 컴포넌트를 조합하여 고유한 행동 패턴을 가집니다.

**참고**: 적의 기본 스탯 및 목록은 [EnemyList.md](EnemyList.md)를 참고하세요. 이 문서는 AI 패턴에 집중합니다.

## AI 패턴 분류

### 1. 직진 추적형 (Tracking Pattern)
**구성**: `FindTarget` + `RotateToTarget` + `MoveStandard`

플레이어를 탐색하고, 천천히 회전하면서 바라보는 방향으로 자동 전진합니다.

**특징**:
- 가장 기본적이고 예측 가능한 패턴
- 회전 속도(`turnSpeed`)로 난이도 조절
- 이동 속도(`movePower`)가 낮아 회피 가능

**해당 적**:
- **Enemy_light_child**: Mass 0.5, Power 2 - 가장 기본적인 추적형
- **Enemy_mid_hornet**: Mass 1.5, Power 2 + 사격 - 멀티샷 장착
- **Enemy_mid_tank**: Mass 4, Power 0.5 + 사격 - 느리지만 내구도 높음
- **Enemy_mid_sniper**: Mass 1, Power 0.5 + 사격 - 원거리 저속 사격
- **Enemy_heavy_mother**: Mass 5, Power 0.5 + 소환 - Child 생성
- **Enemy_Boss**: Mass 100, Power 0.1 + 멀티 사격 - 거의 고정

### 2. 돌격형 (Pursuit Pattern)
**구성**: `FindTarget` + `MoveToTarget` 또는 `MoveTowardTarget`

현재 바라보는 방향과 무관하게 플레이어 방향으로 직진합니다.

**특징**:
- 회전 없이 플레이어 방향으로 즉시 이동
- 충돌 회피가 어려운 위협적인 패턴
- 임펄스 기반(`MoveToTarget`)과 부드러운 접근(`MoveTowardTarget`)으로 나뉨

**해당 적**:
- **Enemy_light_thunder**: Mass 0.5, Power 5 (임펄스) - 빠른 돌진
- **Enemy_mid_knight**: Mass 8, Power 10 (임펄스) - 무거운 돌격
- **Enemy_mid_spiral**: Mass 2, Power 2 (부드러운 접근) + 4방향 사격
- **Enemy_mid_master**: Mass 1, Power 1.5 (부드러운 접근) + 드론 소환

### 3. 거리 유지형 (Kiting Pattern)
**구성**: `FindTarget` + `MoveAwayTarget` + `RotateToTarget` + `ShooterBase`

플레이어와 일정 거리를 유지하며 사격합니다.

**특징**:
- 플레이어가 접근하면 후퇴
- 일정 거리 이상 멀어지면 추적 중단
- 원거리 공격자로 설계됨

**해당 적**:
- **Enemy_heavy_Gunship**: Mass 4, Power 3, 유지 거리 6 + 2방향 사격

### 4. 파동 이동형 (Wave Pattern)
**구성**: `WaveMovement` + `FindTarget` + `ShooterBase`

사인파 궤적으로 이동하며 사격합니다.

**특징**:
- 좌우 또는 상하로 흔들리며 이동
- 탄도 예측 어려움
- amplitude(진폭), frequency(주파수) 파라미터로 조절

**해당 적**:
- **Enemy_mid_Ghost**: Mass 1, Power 1, 진폭 1, 주파수 1

### 5. 단순 직진형 (Direct Pattern)
**구성**: `MoveStandard` (FindTarget 없음)

회전 없이 한 방향으로 직진만 합니다.

**특징**:
- 가장 단순한 패턴
- 예측 가능하고 회피 쉬움
- 대량 생성 시 위협적

**해당 적**:
- **Enemy_light_shield**: Mass 1.5, Power 0.5 - 높은 HP (120)

## AI 컴포넌트 분류

### 사격 컴포넌트 (ShooterBase)
원거리 공격 능력을 제공하는 컴포넌트:
- **단발 사격**: kido, Ghost, tank, sniper
- **멀티샷**: Hornet (여러 발 동시 발사)
- **다방향 사격**: Spiral (4방향), Gunship (2방향)
- **복합 무기**: Boss (3개 무기 시스템)

### 소환 컴포넌트 (ShipFactory)
자식 유닛을 생성하는 컴포넌트:
- **대량 소환**: mother (light_child 지속 생성)
- **지원 소환**: master (드론 소환)

## 자동 전진 시스템과의 호환성

### ✅ 호환 가능 패턴
1. **직진 추적형**: 플레이어도 자동 전진하므로 서로 추격전 형성
2. **돌격형**: 빠른 접근으로 회피 긴장감 유지
3. **거리 유지형**: 플레이어 전진을 역이용하는 패턴
4. **파동 이동형**: 독립적인 궤적으로 예측 어려움

### ⚠️ 조정 필요 사항
- **회전 속도 밸런싱**: 너무 빠르면 항상 정면 대치만 발생
- **이동 속도 밸런싱**: 플레이어보다 너무 빠르면 도망 불가
- **소환 빈도**: 자동 전진으로 화면이 복잡해지기 쉬움

## 파라미터 조정 가이드

### turnSpeed (회전 속도)
- **느림 (0.5-1.0)**: 회피 가능한 추적
- **중간 (1.0-2.0)**: 긴장감 있는 추적
- **빠름 (2.0+)**: 항상 정면 대치

### movePower (이동 힘)
- **느림 (0.1-0.5)**: 거의 고정, 보스/탱크용
- **중간 (1.0-2.0)**: 표준 추적
- **빠름 (5.0-10.0)**: 돌격형, 임펄스용

### searchRadius (탐색 범위)
- **좁음 (5-10)**: 근접에서만 반응
- **중간 (15-20)**: 화면 중간부터 추적
- **넓음 (30+)**: 화면 전체 추적

### fireDelay (사격 간격)
- **느림 (1.0+초)**: 저위협 원거리
- **중간 (0.5-1.0초)**: 표준 사격
- **빠름 (0.2-0.5초)**: 고위협 사격

## 컴포넌트 조합 예시

### 새로운 적 만들기
```
기본 추적형 적:
- EnemyShip (점수, 설명)
- Damageable (HP, 사운드)
- BoundaryJump (화면 경계 처리)
- FindTarget (targetLayer: Player)
- RotateToTarget (turnSpeed: 1.0)
- MoveStandard (movePower: 2.0)
- Rigidbody2D (mass, linearDamping)
- Collider2D

사격 추가:
+ ShooterBase (fireDelay, damage, projectilePrefab)

돌격형으로 변경:
- RotateToTarget 제거
+ MoveToTarget (movePower: 5.0)

거리 유지형으로 변경:
+ MoveAwayTarget (distance: 6, relaxZone: 0.5)
```

## 참고

- [EnemyList.md](EnemyList.md): 적 스탯 및 목록
- [WaveSystem.md](WaveSystem.md): 웨이브 구성 가이드
- [Architecture.md](../Architecture.md#22-게임플레이-레이어): 전체 시스템 구조
- [GameDesignOverview.md](GameDesignOverview.md): 게임 디자인 개요
