# 게임 벨런싱 가이드

> Just 10 Minutes! 프로젝트의 벨런싱 작업 가이드

---

## 게임 벨런싱이란?

게임 수치(체력, 공격력, 난이도)를 조정하여 플레이어에게 **적절한 도전**과 **재미**를 제공하는 작업

### 핵심 목표
- **몰입감**: 적절한 난이도
- **공정성**: 실력으로 결과 차이
- **다양성**: 여러 빌드 가능
- **성장감**: 강해지는 느낌

### 조정 원칙
✅ **DO**: 10-20%씩, 하나씩, 기록
❌ **DON'T**: 극단적 변경, 동시 변경, 느낌만으로

---

## 관련 문서

- **[BalancingChecklist.md](BalancingChecklist.md)** - 플레이테스트 검증 항목

### 수치 데이터 (CSV)
- **[Assets/Data/Balancing/Upgrades.csv](../Assets/Data/Balancing/Upgrades.csv)** - 업그레이드 시스템 수치표
- **[Assets/Data/Balancing/EnemySpawnTimes.csv](../Assets/Data/Balancing/EnemySpawnTimes.csv)** - 적 등장/퇴장 시간
- **[Assets/Data/Balancing/EnemyStats.csv](../Assets/Data/Balancing/EnemyStats.csv)** - 적 스탯 (작성 필요)

---

## Just 10 Minutes! 벨런싱 목표

```
첫 플레이: 5-7분 생존
3-5회: 10분 보스 도달
10회: 보스 클리어
모든 빌드(사격/충돌/탱커) 유효
```

---

## 작업 흐름

```
1. 데이터 수집 → 2. 플레이테스트 → 3. 조정 → 반복
```

---

## 1. 데이터 수집

### 적 스탯 수집
```
Assets/Prefabs/Enemys/ → 각 프리팹 열기
- EnemyShip.point (스폰 비용)
- Damageable.maxDurability (체력)
- ShooterBase.damage (공격력)
→ Assets/Data/Balancing/EnemyStats.csv 파일에 기록
```

### 참고 데이터
- [Upgrades.csv](../Assets/Data/Balancing/Upgrades.csv) - 업그레이드 수치
- [EnemySpawnTimes.csv](../Assets/Data/Balancing/EnemySpawnTimes.csv) - 적 등장 시간
- [EnemyStats.csv](../Assets/Data/Balancing/EnemyStats.csv) - 적 스탯 (수집 필요)

---

## 2. 플레이테스트

### 자체 테스트 (5회)
```
기록 항목:
- 생존 시간
- 사망 원인
- 구간별 난이도 (0-3분/3-7분/7-10분)
- 선택 업그레이드
- 개선 의견
```

### 외부 테스터 (3-5명)
```
피드백 질문:
1. 생존 시간
2. 난이도 (1-5점)
3. 재미있었던/답답했던 순간
4. 선호 업그레이드
5. 다시 플레이 의향
```

---

## 3. 조정

### 문제 → 해결
```
예: 평균 3분 생존 (너무 어려움)
→ 플레이어 체력 +50
→ Git 커밋: "balance: 초기 체력 100→150"
```

### 조정 원칙
```
✅ DO:
- 10-20%씩 조정
- 하나씩 변경
- 변경 전후 기록

❌ DON'T:
- 극단적 변경 (2배, 1/2)
- 여러 요소 동시 변경
- 느낌만으로 판단
```

### 수치 수정 시 영향

**플레이어 버프**
- 체력/실드 ↑ → 생존 시간 ↑, 긴장감 ↓
- 공격력 ↑ → 처치 속도 ↑, 지루함 위험
- 이동속도 ↑ → 회피 용이, 난이도 ↓

**적 버프**
- 체력 ↑ → 전투 시간 ↑, 부담 ↑
- 공격력 ↑ → 위협도 ↑, 긴장감 ↑
- 스폰 빈도 ↑ → 밀도 ↑, 압박감 ↑

**시스템 조정**
- 예산 증가율 → 전체 난이도 곡선 변화
- 목표 존재 점수 → 화면 내 적 수 결정
- 레벨업 속도 → 파워 성장 속도 변화

---

## 유용한 도구

### 디버그 기능
```csharp
Time.timeScale = 2f;  // 2배속
public bool godMode;  // 무적
```

### 엑셀 공식
```
DPS = 발사체피해 × 멀티샷 × 연사속도
생존시간 = 플레이어HP ÷ 적DPS
```

### 현재 디버그
- [TimeBasedSpawnManager.cs](../Assets/Scripts/TimeBasedSpawnManager.cs): `showDebugLogs = true`
- [UiManager.cs](../Assets/Scripts/UIs/UiManager.cs): 예산/Phase 표시

---

## 다음 단계

플레이테스트 시 [BalancingChecklist.md](BalancingChecklist.md) 참고
