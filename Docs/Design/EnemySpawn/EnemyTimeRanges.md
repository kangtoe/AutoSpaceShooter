# 적 시간 범위 정의

> **목적**: 시간 기반 스폰 시스템에서 각 적이 스폰 가능한 시간 범위를 정의합니다.
>
> **설정 방법**: TimeBasedSpawnManager Inspector에서 직렬화 구조체 배열로 관리

## 핵심 개념

- **Elapsed time 방식**: 게임은 0초에서 시작하여 840초(14분)까지 증가
- **spawnTimeMin**: 스폰 가능한 최소 시간 (작은 값, 게임 초반)
- **spawnTimeMax**: 스폰 가능한 최대 시간 (큰 값, 게임 후반)
- **시간 범위**: spawnTimeMin ≤ 현재 경과 시간 ≤ spawnTimeMax일 때 스폰 가능

---

## 적별 시간 범위

| # | 적 이름 | 등급 | 비용 | 시간 범위 | 초 단위 (min ~ max) | 스폰 기간 |
|---|---------|------|------|----------|---------------------|----------|
| 1 | Enemy_light_child | Light | 20 | 0:00 ~ 3:00 | 0 ~ 180 | 3분 |
| 2 | Enemy_light_kido | Light | 20 | 0:30 ~ 3:30 | 30 ~ 210 | 3분 |
| 3 | Enemy_light_thunder | Light | 25 | 1:00 ~ 4:00 | 60 ~ 240 | 3분 |
| 4 | Enemy_mid_Ghost | Mid | 80 | 2:00 ~ 7:00 | 120 ~ 420 | 5분 |
| 5 | Enemy_mid_Hornet | Mid | 90 | 2:30 ~ 7:30 | 150 ~ 450 | 5분 |
| 6 | Enemy_mid_master | Mid | 100 | 3:00 ~ 8:00 | 180 ~ 480 | 5분 |
| 7 | Enemy_mid_Knight | Mid+ | 120 | 3:30 ~ 9:00 | 210 ~ 540 | 5.5분 |
| 8 | Enemy_mid_sniper | Mid+ | 130 | 4:00 ~ 9:30 | 240 ~ 570 | 5.5분 |
| 9 | Enemy_mid_tank | Mid+ | 140 | 4:30 ~ 10:00 | 270 ~ 600 | 5.5분 |
| 10 | Enemy_mid_Spiral | Mid+ | 150 | 5:00 ~ 10:30 | 300 ~ 630 | 5.5분 |
| 11 | Enemy_heavy_mother | Heavy | 350 | 6:00 ~ 13:00 | 360 ~ 780 | 7분 |
| 12 | Enemy_heavy_Gunship | Heavy | 400 | 7:00 ~ 14:00 | 420 ~ 840 | 7분 |

---

## 설계 의도

### 1. 점진적 난이도 증가
- 비용이 낮은 적(Light)이 먼저 등장
- 비용이 높은 적(Heavy)은 게임 후반에 등장
- 30초~1분 간격으로 새로운 적 타입 등장

### 2. 자연스러운 전환
- 시간대가 겹치도록 설계 (급격한 변화 방지)
- 예: Light 적이 사라지기 전에 Mid 적이 등장 시작

### 3. Phase별 분포
**Phase 1 (0:00 ~ 4:00, 0~240초)**:
- Light 적 3종 전체 활성
- 게임 초반, 낮은 난이도

**Phase 2 (4:00 ~ 8:00, 240~480초)**:
- Light 적 일부 + Mid 적 전체 활성
- 중반, 중간 난이도

**Phase 3 (8:00 ~ 14:00, 480~840초)**:
- Mid 적 일부 + Heavy 적 전체 활성
- 후반, 높은 난이도

### 4. 다양성 확보
- 같은 등급이라도 각 적마다 고유한 시간 범위
- 플레이할 때마다 다른 적 조합 경험

---

## 시간대별 스폰 가능 적 (Elapsed Time)

### 0:00 (0초) - 게임 시작
- Enemy_light_child

### 0:30 (30초)
- Enemy_light_child, Enemy_light_kido

### 1:00 (60초)
- Enemy_light_child, Enemy_light_kido, Enemy_light_thunder

### 2:00 (120초)
- Light 3종 + Enemy_mid_Ghost

### 2:30 (150초)
- Light 3종 + Mid 2종 (Ghost, Hornet)

### 3:00 (180초)
- Light 3종 + Mid 3종 (Ghost, Hornet, master)

### 3:30 (210초)
- Light 2종 + Mid 4종 + Enemy_mid_Knight

### 4:00 (240초)
- Light 1종 + Mid 5종 + Enemy_mid_sniper

### 4:30 (270초)
- Mid 6종 + Enemy_mid_tank

### 5:00 (300초)
- Mid 6종 + Enemy_mid_Spiral

### 6:00 (360초)
- Mid 6종 + Enemy_heavy_mother

### 7:00 (420초)
- Mid 5종 + Heavy 2종 (mother, Gunship)

### 8:00 (480초)
- Mid 4종 + Heavy 2종

### 9:00 (540초)
- Mid 3종 + Heavy 2종

### 10:00 (600초)
- Mid+ 2종 + Heavy 2종

### 10:30 (630초)
- Mid+ 1종 + Heavy 2종

### 13:00 (780초)
- Enemy_heavy_Gunship (최종 보스급만 남음)

### 14:00 (840초)
- 게임 종료

---

## 구현 참고

### Inspector 설정 방법
**위치**: TimeBasedSpawnManager GameObject → Inspector

#### 방법 1: 자동 설정 (권장)
1. TimeBasedSpawnManager 컴포넌트 찾기
2. "Enemy Time Ranges" 섹션 상단의 **"자동으로 적 시간 범위 설정"** 버튼 클릭
3. 자동으로 12개 적 프리팹과 시간 범위 설정 완료

#### 방법 2: 수동 설정
1. TimeBasedSpawnManager 컴포넌트 찾기
2. "Enemy Time Ranges" 섹션
3. Size: 12 (적 개수)
4. 각 Element 설정:
   - Enemy Prefab: 프리팹 드래그 앤 드롭
   - Time Min: 최소 시간 (초)
   - Time Max: 최대 시간 (초)

**예시**:
```
Element 0:
  - Enemy Prefab: Enemy_light_child
  - Time Min: 0
  - Time Max: 180

Element 1:
  - Enemy Prefab: Enemy_light_kido
  - Time Min: 30
  - Time Max: 210
...
```

### EnemyTimeRange 구조체
```csharp
[System.Serializable]
public class EnemyTimeRange
{
    public EnemyShip enemyPrefab;  // 프리팹 직접 참조
    public float timeMin;
    public float timeMax;
}
```

### EnemyTimeRangeData.cs 사용법
```csharp
// TimeBasedSpawnManager에서 자동 초기화
EnemyTimeRangeData.Initialize(enemyTimeRanges);

// 사용 시 (elapsed time 전달):
List<string> spawnableEnemies = EnemyTimeRangeData.GetSpawnableEnemiesAtTime(120f); // 2분 경과
bool canSpawn = EnemyTimeRangeData.CanSpawnAtTime("Enemy_light_child", 120f);
```

### 스폰 가능 여부 확인
```csharp
// elapsed time 기준
bool canSpawn = elapsedTime >= min && elapsedTime <= max;
```

---

## 밸런싱 고려사항

### 조정 가능한 요소
1. **시간 범위 폭**: 현재 3~7분, 너무 길거나 짧으면 조정
2. **등장 시점**: 특정 적이 너무 일찍/늦게 등장하면 조정
3. **겹침 정도**: 시간대 겹침을 늘리거나 줄여서 난이도 곡선 조정

### 테스트 체크리스트
- [ ] 게임 초반 (0:00~2:00): 적절한 난이도인지
- [ ] 게임 중반 (4:00~8:00): 너무 쉽거나 어렵지 않은지
- [ ] 게임 후반 (8:00~14:00): 생존 가능한 난이도인지
- [ ] 전환 구간 (3:00, 5:00, 7:00): 자연스럽게 난이도 증가하는지

---

## 관련 문서
- [TimeBasedBudgetSpawnSystem.md](TimeBasedBudgetSpawnSystem.md) - 시간 기반 스폰 시스템 설계
- [Phases.md](Phases.md) - 구현 계획
- [EnemySpawnScaling.md](EnemySpawnScaling.md) - 적 스탯 스케일링

---

## 문서 정보
- **작성일**: 2026-01-20
- **버전**: 1.0
- **상태**: Phase 1 구현용 초안
