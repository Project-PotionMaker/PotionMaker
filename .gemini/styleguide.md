# Potion Maker - C# and Unity Style Guide

**All reviews, summaries, and comments must be written in Korean.**

---

## 1. C# 코딩 컨벤션

### 1-a. 네이밍 컨벤션 및 줄바꿈

* 본 프로젝트의 네이밍 컨벤션은 **C# at Google Style Guide**를 준수합니다.
* **특정 기능 메서드 명명 규칙**:
    * **초기화**: `Init` + `[초기화 대상]` (예: `InitPlayerData()`)
    * **변경/할당**: `Set` + `[변경 대상]` (예: `SetQuest()`)
    * **UI 데이터 반영**: `Refresh` + `[대상]` (예: `RefreshAdventurerCard()`)
* **`if`문**: 내부 로직이 한 줄이더라도 반드시 중괄호 `{}`를 사용합니다.
    ```csharp
    // 좋은 예시
    if (player == null)
    {
        return;
    }

    // 나쁜 예시
    if (player == null) return;
    ```
* **제네릭 컬렉션**: 필드명에 컨테이너명을 접미사로 붙입니다. (`Dictionary`는 `Dict`로 축약). `Sorted` 컨테이너는 `Sorted`를 생략합니다. (예: `playerList`, `itemDict`)
* **인터페이스**: 접두사 `I`를 명시합니다. (예: `IDamageable`)
* **열거형(enum)**: 접두사 `E`를 명시합니다. (예: `EItemType`)
* **UI 클래스**: 접두사 `UI_`를 명시합니다. (예: `UI_HealthBar`)

---

### 1-b. 필드 및 프로퍼티

* 클래스와 구조체 내 모든 필드는 `[SerializeField]` 속성이 추가된 `private` 필드로 선언합니다.
* `[SerializeField]` 속성은 항상 필드 선언부의 윗줄에 단독으로 작성합니다.
    ```csharp
    // 좋은 예시
    [SerializeField]
    private int maxHealth;

    // 나쁜 예시
    [SerializeField] private int maxHealth;
    ```
* `public` 필드는 `Serializable` 클래스, `ScriptableObject`(SO) 클래스, DTO(Data Transfer Object) 클래스에 한해서만 허용됩니다.
* 대리자(`delegate`, `Action`, `Func` 등) 필드는 항상 `public`으로 선언합니다. (1-f 규칙 참고)
* 프로퍼티는 필드를 클래스 외부에서 접근해야 하는 경우에만 선택적으로 선언합니다.
* **선언 순서**: 필드와 프로퍼티는 클래스 최상단에 선언하며, 프로퍼티는 연관된 필드의 바로 아래에 선언합니다.

---

### 1-c. 이벤트 메서드(생명 주기 메서드)

* 이벤트 메서드의 접근 제한자는 반드시 명시해야 합니다.
* **선언 위치**: Unity 이벤트 메서드(생명 주기 메서드)는 일반 메서드보다 **위쪽**에 작성해야 합니다.
* **선언 순서**: 스크립트 상단부터 아래 순서를 지켜 작성합니다.
    1.  **초기화**: `Awake()`, `OnEnable()`, `Start()` 등
    2.  **게임 로직**: `Update()`, `FixedUpdate()`, 충돌 이벤트 메서드 등
    3.  **해체**: `OnApplicationQuit()`, `OnDisable()`, `OnDestroy()`

---

### 1-d. null 체크

* **순수 C# 객체(POCO)**: `is null` 또는 `ReferenceEquals()`를 통해 null 체크를 진행합니다.
* **Unity 객체**:
    * `==`와 `!=` 연산자는 초기화 메서드(`Awake`, `Start`)나 최초 검증 시에만 제한적으로 사용합니다.
    * 프레임 단위의 빈번한 null 체크가 필요할 경우, 성능 저하를 피하기 위해 **반드시 `ReferenceEquals()`를 사용**해야 합니다.
    ```csharp
    // 좋은 예시: Update 메서드 내에서의 null 체크
    private void Update()
    {
        if (!ReferenceEquals(target, null))
        {
            // ...
        }
    }
    ```

---

### 1-e. 싱글톤

* **상속**: `POCOSingleton<T>` (순수 C# 객체용) 또는 `MonoBehaviourSingleton<T>` (Unity 객체용) 제네릭 클래스를 상속하여 사용합니다.
* `MonoBehaviourSingleton<T>` 상속 시, `Awake` 메서드를 `override` 하고 내부에서 `base.Awake()`를 호출해야 합니다.
* **해체 시 주의사항**: `OnDestroy`나 `OnApplicationQuit` 같은 해체 메서드에서는 싱글톤 인스턴스에 직접 접근하는 것을 지양합니다.

---

### 1-f. 대리자

* 대리자 필드는 항상 `public` 접근 제한자와 `PascalCase`로 작성합니다.
* 대리자 필드명은 항상 접두사 `On`을 사용합니다. (예: `public Action OnPlayerDeath;`)
* **선언 위치**: 대리자 필드는 클래스 최상단, 일반 필드 및 프로퍼티보다 위에 작성합니다.

---

### 1-g. Inspector 가독성

* Inspector에서 직접 할당하는 필드는 `VInspector`의 `FoldOut` 어트리뷰트를 추가하여 그룹화합니다.
    * `[FoldOut("Hierarchy")]`: Hierarchy 창에서 할당하는 필드
    * `[FoldOut("Project")]`: Project 폴더에서 할당하는 필드
    * `Header` 어트리뷰트를 통해 추가적인 분류를 진행합니다.
    ```csharp
    [FoldOut("Hierarchy")]
    [Header("UI Elements")]
    [SerializeField]
    private Button confirmButton;

    [FoldOut("Project")]
    [Header("Data")]
    [SerializeField]
    private PlayerSO playerData;
    ```

---

### 1-h. 스크립트 파일 분리

* `interface`, `enum`, `class`는 모두 별도의 스크립트 파일로 분리합니다. 한 파일에 두 개 이상의 타입 정의가 존재해서는 안 됩니다.
* 스크립트 파일명과 내부의 타입 이름은 반드시 동일해야 합니다.

---

### 1-i. Photon Pun 동기화

* `[PunRPC]` 속성 메서드는 접두사 `RPC_`를 명시합니다. (예: `RPC_UpdateHealth()`)
* 비마스터 클라이언트가 마스터 클라이언트의 로직을 요청할 때는 `Request[요청내용]` 형식의 일반 메서드를 호출합니다.
* `Request` 메서드는 호출자가 마스터인지 확인하고, 비마스터일 경우 마스터에게 RPC로 실제 로직 수행을 "요청"합니다. 마스터는 이 RPC를 받아 실제 로직을 수행합니다.

---

## 2. GitHub 컨벤션

### 2-a. 커밋 메시지 컨벤션

* **구조**: `태그 | 제목` 형식으로 작성하며, 태그와 제목은 필수입니다.
* **태그 종류**:
    * `Add`: 에셋, 패키지 등 파일 추가
    * `Feat`: 새로운 기능 구현
    * `Fix`: 버그 수정
    * `Style`: 코드 수정 없는 속성값 변경
    * `Refactor`: 기능 변경 없는 코드 리팩토링
    * `Docs`: 문서, 파일, 폴더명 수정 및 정리
    * `Chore`: 빌드 설정 등 프로젝트 관리 작업
    * `Remove`: 파일 삭제
* **제목**: 50자 이내의 간결한 현재 시제로 작성하며, 마침표를 사용하지 않습니다.

---

### 2-b. PR(Pull Request) 컨벤션

* 브랜치를 병합하기 위해서는 PR을 의무적으로 게시해야 합니다.
* PR은 Task 단위로 작성합니다.
* PR 게시자를 포함하여 최소 2인 이상이 리뷰를 진행합니다.
* PR 제목은 커밋 메시지 형식과 동일합니다.
* PR 템플릿의 체크리스트를 반드시 확인하고 조건을 만족해야 합니다.
* `Assignees`에 본인을 추가하고, 제목 태그와 동일한 `Label`을 의무적으로 추가합니다.

---

### 2-c. 브랜치 컨벤션

* 브랜치는 작업 단위로 생성합니다.
* 브랜치명은 `태그/작업명` 형식으로 합니다. (예: `feat/player-movement`)
* `main`에 병합된 브랜치와 동일한 이름의 로컬 브랜치는 병합 후 반드시 삭제합니다.

---

## 3. Unity Editor 컨벤션

### 3-a. 폴더 및 에셋 정리

* **네이밍 규칙**:
    * **PascalCase**: Scene, 스크립트 (예: `GameScene`, `PlayerManager.cs`)
    * **접두사_이름**: 프리팹, 이미지, 사운드, 애니메이션 (예: `Prefab_UI_ResultInfo`, `Sprite_Adventurer_1`)
* 외부에서 임포트한 에셋 폴더는 `20.External Assets` 하위로 옮깁니다.
* 폴더 구조는 기능 중심이 아닌 콘텐츠 중심 네이밍을 지향합니다.
* 모든 게임 오브젝트와 프리팹은 역할을 명확히 알 수 있도록 의미 있는 이름을 부여합니다.
