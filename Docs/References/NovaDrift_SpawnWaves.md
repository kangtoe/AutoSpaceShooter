# Nova Drift 적 스폰 웨이브 분석

## 개요
Nova Drift는 로그라이크 슈팅 게임으로, 웨이브 기반의 적 스폰 시스템을 통해 점진적으로 난이도가 증가하는 게임플레이를 제공합니다. 이 문서는 Nova Drift의 웨이브 시스템, 스폰 메커니즘, 난이도 스케일링을 분석합니다.

**관련 문서**: 적 타입별 상세 정보는 [NovaDrift_EnemyTypes.md](NovaDrift_EnemyTypes.md)를 참조하세요.

---

## 1. 웨이브 구조

### 1.1 기본 웨이브 시스템
- **웨이브 길이**: 각 웨이브는 약 30-60초 정도 지속
- **웨이브 간 휴식**: 웨이브 클리어 후 짧은 휴식 시간 제공 (업그레이드 선택)
- **무한 진행**: 웨이브는 이론적으로 무한히 진행되며, 난이도가 계속 증가

### 1.2 웨이브 타입
1. **일반 웨이브** (Wave 1-9, 11-19, 21-29...)
   - 다양한 적 타입의 조합
   - 점진적 난이도 증가

2. **보스 웨이브** (Wave 10, 20, 30...)
   - 강력한 보스 적 등장
   - 보스와 함께 일반 적들도 소환
   - 특수 패턴과 높은 체력

3. **엘리트 웨이브** (특정 조건)
   - 강화된 적들의 조합
   - 더 높은 보상

---

## 2. 적 스폰 메커니즘

### 2.1 스폰 패턴

#### 화면 경계 스폰
- 대부분의 적은 화면 경계 밖에서 스폰
- 플레이어의 시야 바깥에서 자연스럽게 등장
- 스폰 위치는 랜덤이지만 플레이어와 일정 거리 유지

#### 그룹 스폰

- 특정 적 타입은 그룹으로 스폰 (예: 소형 적 3-5마리)
- 대형 적은 단독 또는 소수로 스폰
- 그룹 구성은 웨이브 난이도에 따라 변화

#### 연속 스폰
- 웨이브 내에서 적들이 한 번에 모두 나오지 않음
- 시간차를 두고 지속적으로 스폰
- 플레이어가 압도당하지 않도록 조절

### 2.2 동적 웨이브 생성 시스템 (Dynamic Wave Generation)

Nova Drift는 미리 정의된 웨이브 대신 **절차적 생성 시스템**을 사용합니다.

#### 가중치 기반 적 선택 (Weighted Enemy Selection)

**핵심 메커니즘**: 스폰 간격(spawnInterval)마다 **가중치 리스트**에서 적을 선택

```
스폰 체크 시점
├─ 가중치 리스트에서 적 선택
│  ├─ 소형 적 (Swarmling): 가중치 100 → 선택 확률 높음
│  ├─ 중형 적 (Beamcaster): 가중치 40 → 선택 확률 보통
│  └─ 대형 적 (Bulwark): 가중치 10 → 선택 확률 낮음
│
├─ 선택된 적의 스폰 수량 결정 (삼각 분포)
└─ 스폰 실행
```

**디자인 포인트**:
- 같은 spawnInterval이라도 **적 크기별로 등장 빈도가 다름**
- 소형 적은 자주, 대형 적은 드물게 선택됨
- 단순한 "적 크기별 간격" 대신 **확률 기반 시스템** 사용

#### 적 수량 결정 - 삼각 분포 (Triangular Distribution)

```
삼각 분포 파라미터
├─ Low: 최소 스폰 수 (예: 3)
├─ Mean: 평균 스폰 수 (예: 7)
└─ High: 최대 스폰 수 (예: 12)
  ↓
기본 값 계산 (삼각 분포)
  ↓
+ 랜덤 요소 추가
  ↓
× 웨이브 진행도 스케일링
  ↓
최종 스폰 수량
```

**예시**:
- **소형 적 (Swarmling)**: Low=5, Mean=10, High=15
- **중형 적 (Beamcaster)**: Low=2, Mean=4, High=6
- **대형 적 (Bulwark)**: Low=1, Mean=1, High=2

#### 페어드 웨이브 (Paired Waves)

특정 조건에서 **두 개의 동적 스폰이 동시에 발생**:

```
페어드 웨이브 구조
├─ 첫 번째 웨이브: 원래 수량의 52.5%
├─ 두 번째 웨이브: 원래 수량의 32.5%
└─ 합계: 85% (난이도는 높지만 총 적 수는 조절)
```

**디자인 포인트**:
- 두 방향에서 동시 압박
- 숫자는 줄여서 압도당하지 않게 조절
- 전략적 우선순위 선택 강제

#### 후반 웨이브 적 반복 증가

고난이도 웨이브에서 특정 적 타입의 **반복 등장 빈도** 증가:
- 예: Saucers, Interceptors, Shredder Mines
- 웨이브가 진행될수록 같은 적이 여러 번 등장 가능

### 2.3 스폰 타이밍
```
웨이브 시작
├─ 0초: 첫 번째 적 그룹 (소형)
├─ 5초: 두 번째 적 그룹 (소형 + 중형)
├─ 10초: 세 번째 적 그룹 (중형)
├─ 15초: 네 번째 적 그룹 (소형 + 대형)
├─ 20초: 다섯 번째 적 그룹 (엘리트)
└─ 웨이브 종료 (모든 적 처치 시)
```

---

## 3. 난이도 스케일링

### 3.1 웨이브 진행에 따른 변화

#### 적 수 증가
- 초반 웨이브: 5-10마리
- 중반 웨이브: 15-25마리
- 후반 웨이브: 30-50마리+

#### 적 체력 증가
- 웨이브가 진행될수록 적의 기본 체력이 선형/지수적으로 증가
- 공식 예시: `BaseHP * (1 + WaveNumber * 0.15)`

#### 적 공격력 증가
- 피해량도 웨이브에 비례하여 증가
- 공식 예시: `BaseDamage * (1 + WaveNumber * 0.10)`

#### 적 이동 속도 증가
- 후반으로 갈수록 적들이 더 빠르게 이동
- 회피 난이도 상승

### 3.2 적 타입 다양화

적의 종류가 웨이브가 진행됨에 따라 점진적으로 추가되고 복잡해집니다. 각 적의 특성은 [NovaDrift_EnemyTypes.md](NovaDrift_EnemyTypes.md)를 참조하세요.

#### 초반 (Wave 1-5)
- 기본 적: Asteroid, Dart
- 단순한 추적 패턴
- 낮은 체력, 학습 단계

#### 중반 (Wave 6-15)
- 중급 적: Carrier, Bomber, Splitter
- 복잡한 패턴 (원거리 공격, 소환, 분열)
- 중간 체력, 역할 다양화

#### 후반 (Wave 16+)
- 고급 적: Serpent, Dreadnought, Leviathan
- 매우 복잡한 패턴
- 높은 체력, 다단계 공격

---

## 4. 적 조합 전략

### 4.1 역할 기반 조합

#### 탱커 + 딜러 조합
- 고체력 적(탱커)이 앞에서 시선을 끌고
- 후방에서 원거리 적(딜러)이 공격
- 예: Dreadnought + Bomber

#### 스워밍(Swarming)
- 다수의 소형 적이 플레이어를 에워싸는 전략
- 개별 적은 약하지만 집단으로 위협적
- 예: Dart x10-15

#### 지원 + 공격 조합
- 소환 능력을 가진 적이 지속적으로 증원
- 동시에 다른 적들이 공격
- 예: Carrier + Splitter

### 4.2 패턴 믹스
- 직선 이동 적 + 원형 회전 적
- 근접 적 + 원거리 적
- 느린 대형 적 + 빠른 소형 적

---

## 5. 보스 웨이브 설계

### 5.1 보스 특성
- **고체력**: 일반 적의 20-50배
- **다단계 패턴**: 체력에 따라 패턴 변화
- **소환 능력**: 전투 중 일반 적 소환
- **특수 공격**: 강력한 광역 공격

### 5.2 보스 웨이브 구성
```
보스 웨이브 (Wave 10)
├─ 보스 등장 (중앙)
├─ Phase 1 (100%-70% HP)
│  ├─ 기본 패턴 공격
│  └─ 소형 적 소환 (5마리)
├─ Phase 2 (70%-40% HP)
│  ├─ 강화된 패턴
│  └─ 중형 적 소환 (3마리)
└─ Phase 3 (40%-0% HP)
   ├─ 광역 공격
   └─ 엘리트 적 소환 (2마리)
```

### 5.3 보스 진행 예시
- **Wave 10**: 첫 보스 (입문 난이도)
- **Wave 20**: 두 번째 보스 (중간 난이도, 더 복잡한 패턴)
- **Wave 30**: 세 번째 보스 (고난이도, 다단계 공격)

---

## 6. 플레이어 성장과의 균형

### 6.1 적 스케일링 vs 플레이어 파워
Nova Drift는 플레이어의 성장 속도와 적의 강화 속도를 균형있게 조절합니다.

#### 플레이어 성장 요소
- 레벨업당 업그레이드 획득
- 시너지 빌드 구축
- 무기/방어 강화

#### 스케일링 균형
- 초반: 플레이어가 약간 유리 (학습 단계)
- 중반: 균형 잡힌 난이도 (최적화 단계)
- 후반: 빌드가 완성되면 다시 유리, 미완성 시 어려움

### 6.2 적응형 난이도
- 플레이어가 너무 쉽게 클리어하면 적이 더 강해짐
- 반대로 어려워하면 약간 조정
- 하지만 명확한 진행 곡선 유지

---

## 7. 우리 프로젝트에 적용할 점

### 7.1 핵심 메커니즘
1. **점진적 난이도 증가**
   - 선형 스케일링 구현
   - 웨이브 수에 비례한 적 강화

2. **다양한 적 조합**
   - 역할별 적 타입 설계 (탱커, 딜러, 서포터)
   - 조합 테이블 작성

3. **보스 웨이브**
   - 10웨이브마다 보스 등장
   - 다단계 패턴 구현

4. **스폰 분산**
   - 한 번에 모든 적을 스폰하지 않음
   - 시간차 스폰으로 압박감 조절

### 7.2 구현 우선순위
1. **Phase 1**: 기본 웨이브 시스템
   - 웨이브 번호 추적
   - 선형 난이도 증가
   - 단순한 적 조합

2. **Phase 2**: 다양화
   - 여러 적 타입 추가
   - 조합 테이블 구현
   - 스폰 패턴 다양화

3. **Phase 3**: 보스 시스템
   - 보스 적 추가
   - 다단계 패턴
   - 소환 메커니즘

4. **Phase 4**: 밸런싱
   - 플레이어 성장과 균형
   - 적응형 난이도 조정

---

## 8. 참고 데이터

### 8.1 웨이브별 권장 적 구성 (예시)

| 웨이브 | 소형 적 | 중형 적 | 대형 적 | 엘리트 | 보스 |
|--------|---------|---------|---------|--------|------|
| 1-3    | 5-8     | 0-1     | 0       | 0      | 0    |
| 4-6    | 8-12    | 1-2     | 0-1     | 0      | 0    |
| 7-9    | 10-15   | 2-3     | 1       | 0-1    | 0    |
| 10     | 5-8     | 2-3     | 1-2     | 1      | 1    |
| 11-15  | 12-18   | 3-4     | 1-2     | 1      | 0    |
| 20     | 10-15   | 4-5     | 2-3     | 2      | 1    |

### 8.2 스케일링 공식 및 가중치 시스템 (참고용)

#### 기본 스케일링 공식
```csharp
// 적 체력 스케일링
float enemyHP = baseHP * (1 + waveNumber * 0.15f);

// 적 공격력 스케일링
float enemyDamage = baseDamage * (1 + waveNumber * 0.10f);

// 적 크기/속도 스케일링 (Curated Waves)
float enemySize = baseSize * sizeMultiplier;
float enemySpeed = baseSpeed * speedMultiplier;
```

#### 가중치 기반 스폰 시스템 (Nova Drift 방식)

```csharp
// 1. 적 타입별 가중치 설정
Dictionary<EnemyType, float> spawnWeights = new Dictionary<EnemyType, float>
{
    // 소형 적 - 높은 가중치 (자주 등장)
    { EnemyType.Swarmling, 100 },
    { EnemyType.ScatterDrone, 80 },
    { EnemyType.Tracer, 60 },

    // 중형 적 - 중간 가중치 (보통)
    { EnemyType.Beamcaster, 40 },
    { EnemyType.Gyrogun, 35 },
    { EnemyType.Minelayer, 30 },
    { EnemyType.Interceptor, 25 },

    // 대형 적 - 낮은 가중치 (드물게)
    { EnemyType.Bulwark, 15 },
    { EnemyType.Hive, 12 },
    { EnemyType.Hammerhead, 10 },
    { EnemyType.Serpent, 5 }
};

// 2. 스폰 간격 (웨이브 난이도에 따라 감소)
float spawnInterval = baseInterval * Mathf.Max(0.5f, 1 - waveNumber * 0.02f);

// 3. spawnInterval마다 가중치 기반 적 선택
EnemyType selectedEnemy = WeightedRandomSelection(spawnWeights);

// 4. 삼각 분포로 스폰 수량 결정
int spawnCount = TriangularDistribution(
    low: enemyData[selectedEnemy].minSpawn,
    mean: enemyData[selectedEnemy].avgSpawn,
    high: enemyData[selectedEnemy].maxSpawn
) * progressionMultiplier;

// 예시: 적 타입별 스폰 수량 범위
// 소형: Low=5, Mean=10, High=15
// 중형: Low=2, Mean=4, High=6
// 대형: Low=1, Mean=1, High=2
```

#### 삼각 분포 구현 예시
```csharp
float TriangularDistribution(float low, float mean, float high)
{
    float u = Random.value;

    if (u < (mean - low) / (high - low))
    {
        return low + Mathf.Sqrt(u * (high - low) * (mean - low));
    }
    else
    {
        return high - Mathf.Sqrt((1 - u) * (high - low) * (high - mean));
    }
}
```

#### 페어드 웨이브 구현 예시
```csharp
if (IsPairedWave())
{
    int firstWaveCount = Mathf.FloorToInt(spawnCount * 0.525f);
    int secondWaveCount = Mathf.FloorToInt(spawnCount * 0.325f);

    SpawnWave(selectedEnemy, firstWaveCount);
    SpawnWave(GetRandomEnemy(), secondWaveCount); // 다른 적 타입
}
```

#### 적 크기별 스폰 전략 테이블

| 적 크기 | 가중치 범위 | 스폰 수량 (Low-Mean-High) | 등장 빈도 | 역할 |
|---------|-------------|---------------------------|-----------|------|
| 소형 | 60-100 | 5-10-15 | 매우 자주 | 압박, 수적 우위 |
| 중형 | 20-40 | 2-4-6 | 보통 | 전술적 위협 |
| 대형 | 5-15 | 1-1-2 | 드물게 | 보스급 위협 |

**핵심 차이점**:
- ❌ 기존: 모든 적에 동일한 spawnInterval 적용
- ✅ Nova Drift: spawnInterval은 "체크 주기"이고, 실제 등장은 가중치로 결정

---

## 9. 추가 고려사항

### 9.1 플레이어 피드백
- 웨이브 시작 시 명확한 시각적 피드백
- 웨이브 클리어 시 만족감 제공
- 남은 적 수 표시

### 9.2 밸런스 테스트
- 각 웨이브별 클리어 시간 측정
- 플레이어 사망률 분석
- 특정 적 조합의 효과 검증

### 9.3 리플레이 가치
- 랜덤 요소 추가로 매번 다른 경험
- 다양한 빌드로 대응 가능한 설계
- 도전 과제 및 목표 설정

---

## 참고 자료

### 공식 리소스
- [Nova Drift 공식 웹사이트](https://novadrift.io/)
- [Nova Drift Steam 페이지](https://store.steampowered.com/app/858210/Nova_Drift/)
- [Nova Drift 공식 Wiki](https://novadrift.fandom.com/wiki/Nova_Drift_Wiki)
- [공식 Discord 서버](https://discord.gg/novadrift)

### 커뮤니티 & 가이드
- [r/NovaDrift (Reddit 커뮤니티)](https://www.reddit.com/r/NovaDrift/)
- [Nova Drift 빌드 가이드 모음](https://steamcommunity.com/app/858210/guides/)

### 플레이 영상 & 분석
- [Wanderbots - Nova Drift 플레이리스트](https://www.youtube.com/playlist?list=PLordXx8iNEyVt9N4cWZUgZJ_lRZYVYbzI)
- [Retromation - Nova Drift 시리즈](https://www.youtube.com/results?search_query=retromation+nova+drift)
- [게임 메커니즘 분석 영상](https://www.youtube.com/results?search_query=nova+drift+mechanics)

### 개발자 인터뷰 & 포스트모템
- [개발자 AMA (Reddit)](https://www.reddit.com/r/NovaDrift/comments/d4vqxp/were_chimeric_the_dev_team_behind_nova_drift_ama/)
- [GDC Talks - 로그라이크 디자인](https://www.gdcvault.com/search.php#&query=roguelike)

### 개발자 블로그 - 웨이브 시스템 상세
- [Nova Drift Dev Deep Dive: Dynamic Waves](https://blog.novadrift.io/nova-drift-dev-deep-dive-dynamic-waves/) - 동적 웨이브 생성 시스템 개발자 분석
- [Nova Drift: Wave Rework](https://blog.novadrift.io/wave-rework/) - 웨이브 시스템 리워크 설명

### 유사 게임 참고
- **Vampire Survivors**: 대규모 적 스폰 시스템
- **Brotato**: 웨이브 기반 생존 게임
- **20 Minutes Till Dawn**: 점진적 난이도 증가
- **Geometry Wars**: 동적 적 스폰 패턴

---

## 문서 정보
- **작성일**: 2026-01-17
- **최종 수정일**: 2026-01-17
- **버전**: 1.2 (동적 웨이브 생성 시스템 및 가중치 기반 스폰 메커니즘 추가)
- **변경 이력**:
  - v1.2: 동적 웨이브 생성, 가중치 기반 적 선택, 삼각 분포, 페어드 웨이브 추가
  - v1.1: 적 타입 분석 분리
  - v1.0: 초기 문서 작성
- **관련 문서**:
  - [NovaDrift_EnemyTypes.md](NovaDrift_EnemyTypes.md) - 적 타입 상세 분석
  - [WaveSystem.md](../Design/WaveSystem.md) - 우리 프로젝트 웨이브 시스템
  - [ProceduralWaveGeneration.md](../Design/ProceduralWaveGeneration.md) - 절차적 웨이브 생성
  - [EnemyList.md](../Design/EnemyList.md) - 우리 프로젝트 적 리스트
