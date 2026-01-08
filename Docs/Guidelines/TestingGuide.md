# 테스트 가이드

> AI 기반 개발에서는 테스트가 필수입니다. 리팩토링 안전성과 회귀 버그 방지를 위해 핵심 로직에 단위 테스트를 작성합니다.

## Unity Test Framework

### Test Runner 사용
- 위치: `Window > General > Test Runner`
- Unity에 내장된 테스트 실행 도구

### 테스트 종류

**Edit Mode Tests** (`Assets/Tests/EditMode/`)
- Unity Editor에서 즉시 실행 (빠름)
- 순수 로직, 계산, 유틸리티 테스트
- 예시: 경험치 계산, 업그레이드 효과, 피해 공식

**Play Mode Tests** (`Assets/Tests/PlayMode/`)
- Play Mode 진입 필요 (느림)
- MonoBehaviour, 물리, 충돌, 씬 관련 테스트
- 예시: 플레이어 이동, 적 스폰, 충돌 감지

## 테스트 작성 대상

**필수 테스트:**
- 경험치/레벨 계산
- 업그레이드 시스템
- 피해/방어 계산

**선택 테스트:**
- UI, 입력 처리 (수동 테스트 가능)

## 개발 전략

### TDD (Test-Driven Development)
**복잡한 로직에 권장:**
1. 테스트를 먼저 작성 (실패하는 테스트)
2. AI에게 테스트 통과하는 코드 구현 요청
3. 리팩토링 (테스트가 계속 통과하는지 확인)

**적용 대상:** 업그레이드 시스템, 경험치 계산, 피해/방어 계산

### Test-After (구현 후 테스트)
**단순한 컴포넌트에 적용:**
1. 기능을 먼저 구현
2. 동작 확인 후 테스트 작성
3. 향후 변경 시 회귀 버그 방지

**적용 대상:** 물리 기반 이동, 오브젝트 풀링, 사운드 재생

## 테스트 작성 규칙

### 네이밍 컨벤션
테스트 메서드명은 영어로 작성하며, "MethodName_When_ExpectedResult__ForTest" 형식 사용:

```csharp
[Test]
public void MethodName_WhenCondition_ExpectedResult__ForTest()
{
    // Given: 테스트 준비 (초기화, 데이터 설정)
    // When: 테스트 실행 (메서드 호출)
    // Then: 결과 검증 (Assert)
}
```

**예시:**
- `GetExp_WhenReachLevelUpExp_IncreasesLevelBy1__ForTest()`
- `TakeDamage_WhenHealthBelow0_TriggersDeath__ForTest()`
- `ApplyUpgrade_WhenDamageUpgrade_IncreasesDamage__ForTest()`

**규칙:**
- 언더스코어 2개(`__`)로 테스트 설명과 `ForTest` 접미사 구분
- `When` 부분은 테스트 조건을 명확히 표현
- `ExpectedResult`는 동사로 시작 (Increases, Triggers, Returns 등)

### 테스트 격리
- `[SetUp]` - 각 테스트 실행 전 초기화
- `[TearDown]` - 각 테스트 실행 후 정리
- 테스트 간 의존성 금지 (독립 실행 보장)

## 커버리지 목표

**Phase 1 (최소 목표):**
- 핵심 로직 (경험치, 업그레이드, 피해 계산): 80% 이상
- UI/입력 처리: 테스트 없음 허용

**장기 목표:**
- 전체 코드: 60% 이상
- 핵심 로직: 90% 이상

## 테스트 실행

### Unity에서 자동화
Unity Test Framework는 **완전 자동 실행 가능**합니다:

**Edit Mode Tests:**
- Test Runner에서 `Run All` 클릭
- Unity Editor 재시작 없이 즉시 실행
- 수 초 내에 완료

**Play Mode Tests:**
- Unity가 자동으로 Play Mode 진입
- 테스트 실행 후 자동 종료
- 물리, 충돌 등 자동 검증

### 로컬 실행 방법
1. `Window > General > Test Runner` 열기
2. `Run All` 클릭 (또는 개별 테스트 실행)
3. 결과 확인 (Pass/Fail)
4. 실패한 테스트 수정 후 재실행

### 자동 vs 수동 테스트

**자동화 가능 (단위 테스트):**
- 로직, 계산 결과 검증
- 상태 변경 검증 (레벨업, HP 감소 등)
- 물리 충돌, 이동 결과 검증

**수동 확인 필요:**
- 비주얼/UI 품질 (색상, 레이아웃)
- 게임플레이 밸런스 (난이도, 재미)
- 사운드 믹싱

### AI에게 요청
- "모든 테스트를 실행하고 실패한 테스트가 있으면 수정해주세요"
- "LevelManager에 대한 단위 테스트를 작성해주세요"

### CI/CD 자동화 (향후)
- 커맨드라인에서 Batch Mode 실행 가능
- GitHub Actions, Jenkins 등과 연동 가능

## 참고 자료

- [Unity Test Framework 공식 문서](https://docs.unity3d.com/Packages/com.unity.test-framework@1.6/manual/index.html)
- [NUnit Assertions](https://docs.nunit.org/articles/nunit/writing-tests/assertions/assertion-models/constraint.html)
