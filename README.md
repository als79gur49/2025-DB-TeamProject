네, 알겠습니다. 제공해주신 여러 HTML 슬라이드 파일의 내용을 종합하여, GitHub 프로젝트에 바로 사용할 수 있는 전문적인 `README.md` 파일 형식으로 변환해 드리겠습니다.

프로젝트의 개요, 팀원 소개, 아키텍처, 각 테이블의 상세 설명, 그리고 게임 플레이 시나리오까지 모든 내용을 논리적인 순서로 구성했습니다.

---

# Arrow.io 클론 프로젝트: DB 연동 시스템

이 프로젝트는 인기 모바일 게임 'Arrow.io'의 핵심 게임 플레이를 클론하여, Unity 엔진과 SQLite 데이터베이스를 연동하는 시스템을 구축하는 것을 목표로 합니다. 오프라인 환경에서 다수의 AI와 벌이는 실시간 전투를 구현하고, 플레이어의 모든 성장 기록과 성과를 로컬 데이터베이스에 영구적으로 관리하는 데 중점을 두었습니다.

## 목차

1.  [프로젝트 개요](#1-프로젝트-개요)
2.  [팀원 및 역할](#2-팀원-및-역할)
3.  [시스템 아키텍처](#3-시스템-아키텍처)
    -   [3.1. 데이터베이스 스키마](#31-데이터베이스-스키마)
    -   [3.2. 코드 설계 (Manager와 Repository 패턴)](#32-코드-설계-manager와-repository-패턴)
4.  [데이터베이스 상세 설명](#4-데이터베이스-상세-설명)
    -   [4.1. Players 테이블](#41-players-테이블)
    -   [4.2. Entities 테이블](#42-entities-테이블)
    -   [4.3. GameSessions 테이블](#43-gamesessions-테이블)
    -   [4.4. SessionEntities 테이블](#44-sessionentities-테이블)
    -   [4.5. SessionRanking 테이블](#45-sessionranking-테이블)
5.  [게임 플레이 시나리오](#5-게임-플레이-시나리오)

---

## 1. 프로젝트 개요

### 주요 컨셉
-   **실시간 전투:** 다수의 AI와 함께하는 빠른 템포의 전투
-   **성장과 기록:** 플레이어의 레벨, 점수, 통계를 영구적으로 관리
-   **랭킹 시스템:** 실시간 및 전체 순위 제공으로 경쟁 요소 강화

### 기술 스택
-   **게임 엔진:** Unity
-   **데이터베이스:** SQLite (로컬)

### 프로젝트 목표
-   Arrow.io의 핵심 게임 플레이(이동, 발사, 성장) 구현
-   확장과 관리가 용이한 **모듈형 데이터베이스 스키마** 설계
-   게임 내 주요 이벤트와 데이터베이스 간의 명확한 연동 로직 구축

---

## 2. 팀원 및 역할

| 이름 | 역할 | 주요 업무 |
| :--- | :--- | :--- |
| **권민혁** | Developer | 메인 게임 플레이 로직 개발<br>데이터베이스 기능 연동 및 테스트<br>UI 시스템과 게임 로직 연결 |
| **김지호** | Database Architect | DB 스키마 구조 설계<br>테이블 및 뷰(View) 생성<br>데이터 무결성 및 성능 최적화<br>SQL 쿼리 작성 및 관리 |
| **전민균** | UI/UX Designer | 게임 내 UI/UX 디자인 |

---

## 3. 시스템 아키텍처

### 3.1. 데이터베이스 스키마

본 프로젝트의 데이터베이스는 5개의 핵심 테이블로 구성되어, 각자의 역할을 명확히 수행하며 유기적으로 상호작용합니다.

-   `Players`: 플레이어의 계정 정보와 누적 통계를 영구적으로 관리합니다.
-   `Entities`: 플레이어와 AI를 '엔티티'라는 단일 개념으로 통합하여 관리하는 시스템의 중심 허브입니다.
-   `GameSessions`: 게임 한 판의 시작, 종료, 플레이 시간 등 메타데이터를 기록합니다.
-   `SessionEntities`: 특정 세션에서 각 엔티티가 달성한 최종 성과(점수, 순위 등)를 스냅샷 형태로 저장합니다.
-   `SessionRanking`: 게임 진행 중 발생하는 실시간 점수와 순위 변동을 관리하여 성능을 최적화합니다.

### 3.2. 코드 설계 (Manager와 Repository 패턴)

> `Repository` 클래스는 SQL과 연결하고, `Manager` 클래스는 Repository와 게임 로직을 연결하는 브릿지 역할을 수행합니다.

-   **Repository Layer** (`PlayerRepository`, `EntityRepository` 등)
    -   **역할:** 데이터베이스와 직접 통신하며 SQL 쿼리를 실행하는 저수준 계층입니다.
    -   **책임:** CRUD(Create, Read, Update, Delete) 작업을 담당하며, 데이터 모델 객체를 반환합니다.

-   **Manager Layer** (`GameSessionManager`, `EntityGameManager` 등)
    -   **역할:** 게임의 비즈니스 로직을 처리하고, 여러 Repository를 조합하여 하나의 트랜잭션을 완성하는 고수준 계층입니다.
    -   **책임:** 게임 이벤트(예: 게임 시작, 플레이어 사망)에 따라 어떤 Repository의 어떤 함수를 호출할지 결정합니다.

---

## 4. 데이터베이스 상세 설명

### 4.1. Players 테이블

-   **핵심 역할:** 플레이어의 영구적인 정보와 누적 통계를 저장하는 마스터 테이블입니다.
-   **주요 컬럼:** `PlayerID`, `HighestScore`, `TotalPlayTime`, `TotalGames`, `LastPlayedAt`
-   **관련 코드 (C#):** 게임 종료 시 `GameSessionManager`가 `PlayerRepository`의 아래 함수들을 호출하여 누적 데이터를 갱신합니다.

    ```csharp
    // PlayerRepository.cs
    // 새 점수가 기존 최고 점수보다 높을 때만 업데이트
    public static bool UpdateHighestScore(int playerId, int newScore)
    {
        string query = @"
            UPDATE Players SET
                HighestScore = @newScore,
                LastPlayedAt = datetime('now')
            WHERE PlayerID = @playerId AND HighestScore < @newScore
        ";
        // ...
    }

    // 게임 종료 시 통계 업데이트 (플레이 횟수, 시간)
    public static bool UpdateGameStats(int playerId, int playTimeSeconds)
    {
        string query = @"
            UPDATE Players SET
                TotalGames = TotalGames + 1,
                TotalPlayTime = TotalPlayTime + @playTimeSeconds
            WHERE PlayerID = @playerId
        ";
        // ...
    }
    ```

### 4.2. Entities 테이블

-   **핵심 역할:** 플레이어와 AI를 '엔티티'라는 단일 개념으로 통합하여 관리하는 중심 허브입니다.
-   **주요 컬럼:** `EntityID`, `EntityName`, `EntityType`, `PlayerID` (FK)
-   **관련 코드 (C#):** 게임 시작 시 `GameSessionManager`가 `EntityRepository`를 호출하여 플레이어 또는 AI 엔티티를 생성합니다.

    ```csharp
    // EntityRepository.cs
    // 플레이어 타입의 엔티티 생성
    public static EntityModel CreatePlayerEntity(int playerId, string playerName)
    {
        string query = @"
            INSERT INTO Entities (EntityName, EntityType, PlayerID)
            VALUES (@entityName, 'Player', @playerId)
        ";
        // ...
    }

    // AI 타입의 엔티티 생성
    public static EntityModel CreateAIEntity(string aiName)
    {
        string query = @"
            INSERT INTO Entities (EntityName, EntityType, PlayerID)
            VALUES (@entityName, 'AI', NULL)
        ";
        // ...
    }
    ```

### 4.3. GameSessions 테이블

-   **핵심 역할:** 게임 한 판의 시작부터 끝까지의 생명주기와 메타데이터를 관리합니다.
-   **주요 컬럼:** `SessionID`, `TotalEntities`, `PlayTimeSeconds`, `StartedAt`, `EndedAt`, `IsCompleted`
-   **관련 코드 (C#):** `GameSessionRepository`는 세션의 생성과 종료를 담당합니다.

    ```csharp
    // GameSessionRepository.cs
    // 새로운 게임 세션 시작
    public static GameSessionModel StartNewSession()
    {
        string query = @"
            INSERT INTO GameSessions (StartedAt, IsCompleted)
            VALUES (datetime('now'), FALSE)
        ";
        // ...
    }

    // 게임 세션 종료 처리
    public static bool EndSession(int sessionId, int totalEntities)
    {
        // ... 플레이 시간 계산 ...
        string query = @"
            UPDATE GameSessions SET
                TotalEntities = @totalEntities,
                EndedAt = datetime('now'),
                PlayTimeSeconds = @playTime,
                IsCompleted = TRUE
            WHERE SessionID = @sessionId
        ";
        // ...
    }
    ```

### 4.4. SessionEntities 테이블

-   **핵심 역할:** 특정 세션에 참여한 각 엔티티의 최종 성과를 '스냅샷' 형태로 영구 기록합니다.
-   **주요 컬럼:** `SessionID`(FK), `EntityID`(FK), `Score`, `Level`, `EnemiesKilled`, `IsAlive`, `FinalRank`
-   **관련 코드 (C#):** 게임 시작 시 참여 기록을 생성하고, 엔티티 사망 시 최종 결과를 업데이트합니다.

    ```csharp
    // SessionEntityRepository.cs
    // 세션에 엔티티 참여 기록 생성
    public static SessionEntityModel AddEntityToSession(
        int sessionId, int entityId, string entityName, string entityType)
    {
        string query = @"
            INSERT INTO SessionEntities 
            (SessionID, EntityID, EntityName, EntityType)
            VALUES (@sessionId, @entityId, @entityName, @entityType)
        ";
        // ...
    }

    // 엔티티 사망 시 최종 순위와 상태 업데이트
    public static bool SetEntityDead(int sessionId, int entityId, int finalRank)
    {
        var sessionEntity = GetSessionEntity(sessionId, entityId);
        sessionEntity.SetDead(finalRank); // IsAlive=false, DiedAt=now
        return UpdateSessionEntity(sessionEntity);
    }
    ```

### 4.5. SessionRanking 테이블

-   **핵심 역할:** 게임 중 빈번하게 발생하는 점수/레벨 업데이트를 전담하여 성능을 최적화합니다.
-   **주요 컬럼:** `SessionID`(FK), `EntityID`(FK), `CurrentScore`, `CurrentLevel`, `IsActive`
-   **관련 코드 (C#):** `RankingManager`가 랭킹 데이터의 생명주기를 관리합니다.

    ```csharp
    // RankingManager.cs
    // 실시간 랭킹에 엔티티 추가 (게임 시작 시)
    public static bool AddToLiveRanking(int sessionId, int entityId, ...)
    {
        string query = @"
            INSERT OR REPLACE INTO SessionRanking 
            (SessionID, EntityID, ..., IsActive)
            VALUES (@sessionId, @entityId, ..., TRUE)
        ";
        // ...
    }

    // 실시간 랭킹 업데이트 (점수 획득 시)
    public static bool UpdateLiveRanking(int sessionId, int entityId, int score, ...)
    {
        string query = @"
            UPDATE SessionRanking SET
                CurrentScore = @currentScore,
                LastUpdated = datetime('now')
            WHERE SessionID = @sessionId AND EntityID = @entityId AND IsActive = TRUE
        ";
        // ...
    }
    ```

---

## 5. 게임 플레이 시나리오

게임의 주요 이벤트에 따른 데이터 흐름은 다음과 같습니다.

1.  **게임 시작**
    -   `GameSessionManager.StartSession()` 호출
    -   `GameSessions`, `Players`, `Entities`, `SessionEntities`, `SessionRanking` 테이블에 초기 데이터 생성

2.  **적 처치 / 점수 획득**
    -   `EntityGameManager.OnEntityScoreUpdate()` 호출
    -   `SessionRanking` 테이블의 `CurrentScore` 실시간 업데이트

3.  **레벨업**
    -   `EntityGameManager.OnPlayerLevelUp()` 호출
    -   `SessionRanking` 테이블의 `CurrentLevel` 실시간 업데이트

4.  **게임 종료 (플레이어 사망)**
    -   `GameSessionManager.OnPlayerDeath()` 호출
    -   `SessionEntities`에 최종 성과(점수, 순위 등) 기록
    -   `Players` 테이블의 누적 통계(`HighestScore`, `TotalGames` 등) 갱신
    -   `GameSessions` 테이블을 '완료' 상태로 변경
