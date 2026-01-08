# 문서 작성 가이드

> 일반적인 마크다운 문법은 [Markdown Guide](https://www.markdownguide.org/)를 참조하세요.
>
> 이 문서는 **AutoSpaceShooter 프로젝트의 문서 작성 규칙**만 다룹니다.

## 문서 구조

```
Docs/
├── Roadmap.md               # 개발 로드맵
├── Design/                  # 게임 디자인
│   └── GameDesignOverview.md
└── Guidelines/              # 개발 가이드라인
    ├── CodeStyle.md
    ├── CommitGuide.md
    └── DocumentationGuide.md    # 이 문서
```

## 문서 작성 원칙

### 1. 문서 길이 관리
- 문서가 **100줄을 넘어가면 분리를 고려**
- 분리 기준: 주제별, 시스템별로 독립적인 문서 생성
- 예: `GameDesignOverview.md` → 시스템별로 `CombatSystem.md`, `ProgressionSystem.md` 등

### 2. 파일명 규칙
- **PascalCase** 사용 (예: `GameDesignOverview.md`)
- 명확하고 설명적인 이름
- 약어 사용 지양

### 3. 내용 작성
- **프로젝트 특화 내용**에 집중
- 일반적인 규칙은 외부 링크로 대체
- "당연한" 내용은 과감히 제거

### 4. 업데이트 원칙
- 코드 변경 시 관련 문서도 함께 업데이트
- 문서와 실제 구현이 일치하도록 유지
- 오래된 정보는 주기적으로 정리

## 참고 자료
- [Markdown Guide](https://www.markdownguide.org/)
- [Writing Good Documentation](https://docs.github.com/en/get-started/writing-on-github)
