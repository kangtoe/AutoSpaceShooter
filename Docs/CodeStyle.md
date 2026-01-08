# 코드 작성 원칙

## 네이밍 컨벤션

### C# 클래스 및 파일명
- **PascalCase** 사용
- 예: `PlayerController.cs`, `EnemySpawner.cs`, `UpgradeManager.cs`

### 변수 및 필드
- **private 필드**: camelCase + 언더스코어 접두사
  - 예: `_currentHealth`, `_moveSpeed`
- **public 필드/프로퍼티**: PascalCase
  - 예: `MaxHealth`, `IsAlive`
- **로컬 변수**: camelCase
  - 예: `currentPosition`, `enemyCount`

### 함수/메서드
- **PascalCase** 사용
- 동사로 시작
- 예: `TakeDamage()`, `SpawnEnemy()`, `CalculateExperience()`

### 상수
- **UPPER_SNAKE_CASE** 또는 **PascalCase**
- 예: `MAX_ENEMIES` 또는 `MaxEnemies`

## 폴더 구조

```
Assets/
├── Scenes/           # 씬 파일
├── Scripts/          # C# 스크립트
│   ├── Player/       # 플레이어 관련
│   ├── Enemy/        # 적 관련
│   ├── Managers/     # 싱글톤 매니저들
│   ├── UI/           # UI 관련
│   └── Upgrades/     # 업그레이드 시스템
├── Prefabs/          # 프리팹
├── Sprites/          # 스프라이트/이미지
├── Materials/        # 머티리얼
└── Audio/            # 사운드/음악
```

## Unity 컴포넌트 작성 패턴

### MonoBehaviour 라이프사이클 순서
```csharp
// 1. 변수 선언
[SerializeField] private float _speed;
private Rigidbody2D _rb;

// 2. Unity 메시지 (순서대로)
private void Awake() { }
private void Start() { }
private void Update() { }
private void FixedUpdate() { }

// 3. Public 메서드
public void TakeDamage(int damage) { }

// 4. Private 메서드
private void Die() { }
```

### SerializeField 활용
- public 대신 `[SerializeField] private` 사용 권장
- Inspector에서 편집 필요한 변수만 SerializeField 지정

## 주석 작성 규칙

### 한 줄 주석
```csharp
// 경계를 벗어나면 즉사
if (IsOutOfBounds()) Die();
```

### 복잡한 로직
```csharp
/// <summary>
/// 경험치를 추가하고 레벨업 여부를 확인합니다.
/// </summary>
/// <param name="exp">획득한 경험치량</param>
/// <returns>레벨업 했으면 true</returns>
public bool AddExperience(int exp)
{
    // 구현...
}
```

### 주석 원칙
- 코드가 명확하면 주석 불필요
- "왜(Why)"를 설명, "무엇(What)"은 코드로 표현
- 임시 해결책이나 TODO는 명시
  - 예: `// TODO: 성능 최적화 필요`

## 코드 스타일

### 중괄호
```csharp
// Good - 항상 중괄호 사용
if (condition)
{
    DoSomething();
}

// Bad
if (condition)
    DoSomething();
```

### 공백
```csharp
// 연산자 양쪽에 공백
int result = a + b;

// 쉼표 뒤에 공백
Method(arg1, arg2, arg3);
```

## 디자인 패턴

### Singleton Manager
```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
```

### Object Pooling
- 적, 총알 등 반복 생성/파괴되는 오브젝트에 사용
- Instantiate/Destroy 대신 풀에서 가져오기/반환하기

## 성능 고려사항

### Update vs FixedUpdate
- **Update**: 입력 처리, UI 업데이트
- **FixedUpdate**: 물리 연산 (Rigidbody2D 조작)

### GetComponent 캐싱
```csharp
// Good - Awake/Start에서 캐싱
private Rigidbody2D _rb;
private void Awake()
{
    _rb = GetComponent<Rigidbody2D>();
}

// Bad - 매 프레임 호출
private void Update()
{
    GetComponent<Rigidbody2D>().AddForce(...);
}
```

### Find 함수 사용 최소화
- `FindObjectOfType`, `GameObject.Find` 등은 비용이 높음
- 참조를 미리 할당하거나 싱글톤 패턴 활용

## 예외 처리

```csharp
// null 체크
if (_rb == null)
{
    Debug.LogError("Rigidbody2D component missing!");
    return;
}

// 범위 체크
if (damage < 0)
{
    Debug.LogWarning("Damage cannot be negative!");
    damage = 0;
}
```

## 참고 자료
- [Unity C# Coding Standards](https://unity.com/how-to/naming-and-code-style-tips-c-scripting-unity)
- [Microsoft C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
