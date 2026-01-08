# 프로젝트 마이그레이션 노트

> 기존 스페이스 슈터 프로젝트를 AutoSpaceShooter로 전환하는 과정의 변경사항 및 작업 내역

## 프로젝트 현황

### 기존 프로젝트 구조

```
Assets/Scripts/
├── Managers (싱글톤 패턴)
│   ├── GameManager - 게임 상태 관리
│   ├── InputManager - 입력 처리
│   ├── LevelManager - 레벨/경험치 시스템 ✅
│   ├── UpgradeManager - 업그레이드 시스템 ⚠️
│   ├── UiManager - UI 관리 ✅
│   ├── SoundManager - 사운드 시스템 ✅
│   └── EnemySpawner - 적 생성 ✅
├── Player/
│   ├── PlayerShip - 플레이어 기체 메인
│   ├── HeatSystem - 과열 시스템
│   ├── BrakeSystem - 브레이크 시스템
│   ├── Impactable - 충돌 피해 시스템 ✅
│   └── StackSystem - 스택 시스템
├── SpaceShips/
│   ├── Damageable - HP 시스템 ✅
│   ├── ShooterBase - 사격 기본 클래스 ✅
│   ├── EnemyShip - 적 기체
│   └── FindTarget - 타겟 찾기
├── Movements/
│   ├── MoveStandard - 표준 이동 ❌
│   ├── RotateToTarget - 타겟 회전
│   └── 기타 이동 컴포넌트들
├── Projectiles/ - 발사체 시스템 ✅
├── UIs/ - UI 컴포넌트들 ✅
└── Drone/ - 드론 시스템 (미사용 예정)
```

범례:
- ✅ 재사용 가능
- ⚠️ 수정 필요
- ❌ 새로 구현 필요

## 주요 변경사항

### 1. 조작 시스템 (핵심 변경)

**현재 상태:**
```csharp
// InputManager.cs
public bool MoveForwardInput => false;  // 자동 전진 미사용
public Vector2 MoveDirectionInput => WASD 입력;  // 수동 방향 입력
```

**목표:**
- 자동 전진: transform.up 방향으로 지속적인 힘 적용
- 회전 제어: 화면 좌/우 클릭으로 회전 방향 조절
- 물리 기반 이동: Rigidbody2D.AddForce() 사용

**작업 필요:**
- [ ] InputManager 수정 - 터치/클릭 입력으로 변경
- [ ] MoveStandard → 새로운 자동 전진 컴포넌트 작성
- [ ] 회전 시스템 구현

### 2. 경계 처리 시스템

**현재 상태:**
- 확인 필요: BoundaryJump.cs 존재 (텔레포트 기능?)

**목표:**
- 화면 경계를 벗어나면 즉사

**작업 필요:**
- [ ] 현재 경계 처리 방식 확인
- [ ] 경계 즉사 시스템 구현

### 3. 업그레이드 시스템 (대폭 수정)

**현재 상태:**
```csharp
// UpgradeManager.cs
// 3가지 고정 타입만 존재
enum UpgradeType {
    Ship,
    Shooter,
    EmergencyProtocol
}
```

**목표:**
- 노바 드리프트 스타일
- 레벨업 시 여러 업그레이드 중 선택
- 다양한 업그레이드 옵션 (Phase 1: 3-5개)

**작업 필요:**
- [ ] UpgradeData 구조 재설계
- [ ] 선택지 UI 구현
- [ ] 업그레이드 풀 시스템 구현

### 4. 레벨/경험치 시스템 (재사용)

**현재 상태:**
```csharp
// LevelManager.cs
- 적 격파 → 경험치 획득 ✅
- 경험치 누적 → 레벨업 ✅
- 레벨업 시 UpgradePoint 지급 ✅
```

**목표:**
- 동일한 시스템 유지
- 밸런싱만 조정

**작업 필요:**
- [x] 시스템 확인 완료
- [ ] 경험치량 밸런싱 (Phase 1 후반)

### 5. 전투 시스템 (일부 재사용)

**재사용 가능:**
- Damageable.cs - HP 시스템
- ShooterBase.cs - 사격 기본 클래스
- Impactable.cs - 충돌 피해 시스템
- 발사체 시스템

**작업 필요:**
- [ ] 자동 사격 vs 수동 사격 결정
- [ ] 충돌 피해량 밸런싱
- [ ] 사격 빌드 vs 충돌 빌드 차별화

### 6. 적 생성 시스템 (재사용)

**현재 상태:**
```csharp
// EnemySpawner.cs
- 시간 기반 스폰 ✅
- 무한 스폰 모드 ✅
- Edge 별 스폰 위치 ✅
```

**작업 필요:**
- [ ] 적 종류 확인 및 정리
- [ ] 스폰 패턴 조정

## 삭제/미사용 예정 시스템

- [ ] Drone 시스템 (DroneMaster, DroneSevant)
- [ ] BrakeSystem (자동 전진 방식에서 불필요)
- [ ] HeatSystem (과열 시스템 - 필요성 재검토)
- [ ] StackSystem (용도 불명 - 확인 필요)

## 확인 필요 사항

### 물리 시스템
- [ ] MoveStandard.cs - 현재 물리 기반인지 확인
- [ ] BoundaryJump.cs - 경계 처리 방식 확인
- [ ] 충돌 레이어 설정 확인

### UI 시스템
- [ ] 현재 UI 구조 확인
- [ ] 업그레이드 선택 UI 재사용 가능 여부
- [ ] HUD 요소들 확인

### 발사체 시스템
- [ ] BulletBase, BulletChase, BulletCurve 등 확인
- [ ] 발사체 종류 정리
- [ ] Object Pooling 구현 여부 확인

## 다음 작업 단계

### Phase 1A: 핵심 시스템 분석
1. [ ] 물리 이동 시스템 상세 분석
2. [ ] 경계 처리 시스템 확인
3. [ ] UI 구조 파악
4. [ ] 발사체 시스템 파악

### Phase 1B: 조작 시스템 구현
1. [ ] 자동 전진 구현
2. [ ] 회전 제어 구현
3. [ ] 입력 시스템 수정

### Phase 1C: 게임플레이 루프
1. [ ] 경계 즉사 구현
2. [ ] 기본 적 AI 조정
3. [ ] 기본 업그레이드 3-5개 구현

## 참고사항

- 기존 코드의 MonoSingleton 패턴은 유지
- SoundManager, UiManager 등은 최대한 재사용
- 불필요한 기능은 과감히 제거하여 단순화
