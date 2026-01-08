# Git 및 문서 작성 가이드

## 커밋 메시지 형식

### 기본 구조
```
<타입>: <제목> (한 줄 요약)

<본문> (선택사항)
- 변경 사항 설명
- 왜 변경했는지 설명

```

### 커밋 타입

| 타입 | 설명 | 예시 |
|------|------|------|
| `feat` | 새로운 기능 추가 | `feat: 플레이어 자동 이동 시스템 구현` |
| `fix` | 버그 수정 | `fix: 경계 충돌 시 즉사 처리 버그 수정` |
| `refactor` | 코드 리팩토링 (기능 변경 없음) | `refactor: EnemySpawner 로직 개선` |
| `chore` | 빌드, 설정 변경 | `chore: .gitignore에 UserSettings 추가` |
| `docs` | 문서 작업 | `docs: 게임 기획 문서 작성` |
| `style` | 코드 스타일 변경 (포맷팅 등) | `style: PlayerController 들여쓰기 수정` |
| `test` | 테스트 추가/수정 | `test: 충돌 피해 시스템 테스트 추가` |
| `perf` | 성능 개선 | `perf: Object Pooling 적용` |

### 커밋 메시지 작성 원칙

1. **제목은 50자 이내**로 간결하게
2. **제목은 명령형**으로 작성 (예: "추가함" ❌ → "추가" ✅)
3. **본문은 72자마다 줄바꿈**
4. **한글 사용** (프로젝트 언어 통일)
5. **무엇을, 왜** 변경했는지 명확히

### 좋은 커밋 예시
```
feat: 플레이어 회전 조작 시스템 구현

- 화면 좌/우 터치로 회전 방향 조절
- Input System 패키지 활용
- 모바일 터치 및 마우스 클릭 모두 지원

```

### 나쁜 커밋 예시
```
❌ update
❌ 작업중
❌ 기능 추가했음
❌ fix bug
```

## 브랜치 전략

### 기본 브랜치
- **main**: 안정적인 릴리즈 버전
- 작은 프로젝트이므로 main에서 직접 작업 가능

### 기능 브랜치 (선택사항)
큰 기능 개발 시:
```
feature/player-movement
feature/upgrade-system
feature/enemy-spawner
```

사용법:
```bash
git checkout -b feature/player-movement
# 작업 후
git checkout main
git merge feature/player-movement
```

## Pull Request (선택사항)

협업 시 PR 제목 형식:
```
[FEAT] 플레이어 자동 이동 시스템 구현
[FIX] 경계 충돌 버그 수정
[REFACTOR] 적 생성 로직 개선
```

## 마크다운 문서 작성 규칙

### 제목 계층
```markdown
# H1 - 문서 제목 (파일당 1개)
## H2 - 주요 섹션
### H3 - 하위 섹션
#### H4 - 세부 항목
```

### 코드 블록
````markdown
```csharp
public class Example
{
    // C# 코드
}
```
````

### 리스트
```markdown
- 순서 없는 리스트
  - 중첩 리스트

1. 순서 있는 리스트
2. 두 번째 항목
```

### 강조
```markdown
**굵게** (중요)
*기울임* (강조)
`코드` (인라인 코드)
```

### 링크
```markdown
[링크 텍스트](URL)
[파일 참조](../Scripts/PlayerController.cs)
```

### 표
```markdown
| 항목 | 설명 |
|------|------|
| A    | 설명 A |
| B    | 설명 B |
```

### 체크리스트
```markdown
- [ ] 미완료 항목
- [x] 완료 항목
```

## 문서 관리 원칙

### 문서 업데이트
- 코드 변경 시 관련 문서도 함께 업데이트
- 문서와 실제 구현이 일치하도록 유지

### 문서 위치
```
Docs/
├── GameDesignOverview.md    # 게임 디자인 전반
├── Roadmap.md               # 개발 로드맵
├── CodeStyle.md             # 코드 작성 원칙
└── CommitGuide.md           # Git 가이드 (이 문서)
```

### 문서 작성 시점
- **기획 단계**: 기능 설계 및 요구사항 정리
- **개발 중**: 중요한 구현 결정 사항 기록
- **완료 후**: 사용법 및 예제 추가

## .gitignore 관리

### 포함하지 않을 파일/폴더
```
/Library/           # Unity 캐시
/Temp/              # 임시 파일
/UserSettings/      # 사용자별 설정
.vscode/            # 에디터 설정
*.sln, *.slnx       # 솔루션 파일 (자동 생성)
```

### 반드시 커밋할 것
```
/Assets/            # 게임 에셋
/Packages/          # 패키지 매니페스트
/ProjectSettings/   # 프로젝트 설정
```

## 참고 자료
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Git 브랜치 전략](https://git-scm.com/book/ko/v2/Git-%EB%B8%8C%EB%9E%9C%EC%B9%98-%EB%B8%8C%EB%9E%9C%EC%B9%98-%EC%9B%8C%ED%81%AC%ED%94%8C%EB%A1%9C%EC%9A%B0)
- [Markdown Guide](https://www.markdownguide.org/)
