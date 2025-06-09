using Mono.Data.Sqlite;
using System;
using System.Data;
using System.IO;
using UnityEngine;

/// <summary>
/// SQLite 데이터베이스 연결 및 기본 관리를 담당하는 클래스
/// </summary>
public static class DatabaseManager
{
    private static IDbConnection connection;
    private static string dbPath;

    // 현재 데이터베이스 버전
    private const int CURRENT_DB_VERSION = 2;

    /// <summary>
    /// 데이터베이스 초기화
    /// </summary>
    public static void Initialize()
    {
        // StreamingAssets 폴더에 데이터베이스 파일 위치 설정
        dbPath = Path.Combine(Application.streamingAssetsPath, "GameDB.db");

        bool isNewDatabase = !File.Exists(dbPath);
        bool needsReset = false;

        // 연결이 없거나 닫혀있으면 새로 연결
        if (connection == null || connection.State != ConnectionState.Open)
        {
            connection = new SqliteConnection("URI=file:" + dbPath);
            connection.Open();

            if (!isNewDatabase)
            {
                // 기존 데이터베이스가 있으면 버전 체크
                int currentVersion = GetDatabaseVersion();
                if (currentVersion != CURRENT_DB_VERSION)
                {
                    Debug.Log($"데이터베이스 버전이 다릅니다. 현재: {currentVersion}, 필요: {CURRENT_DB_VERSION}");
                    needsReset = true;
                }
            }

            if (isNewDatabase || needsReset)
            {
                if (needsReset)
                {
                    Debug.Log("데이터베이스를 초기화합니다...");
                    ResetDatabase();
                }

                // 테이블 생성
                CreateTables();

                // 버전 정보 설정
                SetDatabaseVersion(CURRENT_DB_VERSION);

                Debug.Log($"데이터베이스가 성공적으로 {(isNewDatabase ? "생성" : "초기화")}되었습니다 (버전: {CURRENT_DB_VERSION})");
            }
            else
            {
                Debug.Log($"기존 데이터베이스를 사용합니다 (버전: {CURRENT_DB_VERSION})");
            }
        }
    }

    /// <summary>
    /// 데이터베이스 연결 종료
    /// </summary>
    public static void Close()
    {
        if (connection != null && connection.State == ConnectionState.Open)
        {
            connection.Close();
            connection = null;
            Debug.Log("데이터베이스 연결이 종료되었습니다");
        }
    }

    /// <summary>
    /// 데이터베이스 연결 객체 반환
    /// </summary>
    public static IDbConnection GetConnection()
    {
        if (connection == null || connection.State != ConnectionState.Open)
        {
            Initialize();
        }
        return connection;
    }

    /// <summary>
    /// 필요한 테이블들을 생성
    /// </summary>
    private static void CreateTables()
    {
        // 플레이어 기본 정보 테이블
        ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Players (
                PlayerID INTEGER PRIMARY KEY AUTOINCREMENT,
                PlayerName VARCHAR(100) NOT NULL,
                CurrentLevel INTEGER DEFAULT 1,
                CurrentExp INTEGER DEFAULT 0,
                ExpToNextLevel INTEGER DEFAULT 100,
                HighestScore INTEGER DEFAULT 0,
                TotalPlayTime INTEGER DEFAULT 0,
                CreatedAt DATETIME DEFAULT (datetime('now', '+9 hours')),
                LastPlayedAt DATETIME DEFAULT (datetime('now', '+9 hours'))
            )
        ");

        // 게임 세션 기록 테이블
        ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS GameSessions (
                SessionID INTEGER PRIMARY KEY AUTOINCREMENT,
                PlayerID INTEGER NOT NULL,
                Score INTEGER NOT NULL DEFAULT 0,
                Level INTEGER NOT NULL DEFAULT 1,
                EnemiesKilled INTEGER DEFAULT 0,
                DeathCount INTEGER DEFAULT 0,
                StartedAt DATETIME DEFAULT (datetime('now', '+9 hours')),
                EndedAt DATETIME,
                PlayTime INTEGER,
                FOREIGN KEY (PlayerID) REFERENCES Players(PlayerID) ON DELETE CASCADE
            )
        ");

        // 발사체 정보 테이블
        ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Projectiles (
                ProjectileID INTEGER PRIMARY KEY AUTOINCREMENT,
                ProjectileName VARCHAR(100) NOT NULL,
                BaseDamage REAL NOT NULL,
                BaseSpeed REAL NOT NULL,
                BaseRange REAL NOT NULL,
                BaseDuration REAL NOT NULL,
                BaseAttackRate REAL NOT NULL,
                BaseSize REAL NOT NULL,
                ProjectileType VARCHAR(50),
                EffectDescription TEXT
            )
        ");

        // 무기 정보 테이블
        ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Weapons (
                WeaponID INTEGER PRIMARY KEY AUTOINCREMENT,
                WeaponName VARCHAR(100) NOT NULL,
                BaseProjectileID INTEGER,
                DamageMultiplier REAL DEFAULT 1.0,
                SpeedMultiplier REAL DEFAULT 1.0,
                RangeMultiplier REAL DEFAULT 1.0,
                DurationMultiplier REAL DEFAULT 1.0,
                AttackRateMultiplier REAL DEFAULT 1.0,
                SizeMultiplier REAL DEFAULT 1.0,
                UnlockLevel INTEGER DEFAULT 1,
                WeaponDescription TEXT,
                FOREIGN KEY (BaseProjectileID) REFERENCES Projectiles(ProjectileID)
            )
        ");

        // 스킬 정보 테이블
        ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Skills (
                SkillID INTEGER PRIMARY KEY AUTOINCREMENT,
                SkillName VARCHAR(100) NOT NULL,
                MaxLevel INTEGER DEFAULT 5,
                SkillDescription TEXT,
                SkillType VARCHAR(50),
                UnlockLevel INTEGER DEFAULT 1,
                UpgradeCost INTEGER DEFAULT 1
            )
        ");

        // 스킬 레벨별 효과 테이블
        ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS SkillEffects (
                EffectID INTEGER PRIMARY KEY AUTOINCREMENT,
                SkillID INTEGER NOT NULL,
                SkillLevel INTEGER NOT NULL,
                EffectType VARCHAR(50),
                EffectValue REAL NOT NULL,
                EffectDescription TEXT,
                FOREIGN KEY (SkillID) REFERENCES Skills(SkillID) ON DELETE CASCADE,
                UNIQUE(SkillID, SkillLevel, EffectType)
            )
        ");

        // 게임 세션 중 스킬 선택 기록 테이블
        ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS SessionSkillChoices (
                ChoiceID INTEGER PRIMARY KEY AUTOINCREMENT,
                SessionID INTEGER NOT NULL,
                SkillID INTEGER NOT NULL,
                ChoiceOrder INTEGER NOT NULL,
                PlayerLevel INTEGER NOT NULL,
                ChosenAt DATETIME DEFAULT (datetime('now', '+9 hours')),
                FOREIGN KEY (SessionID) REFERENCES GameSessions(SessionID) ON DELETE CASCADE,
                FOREIGN KEY (SkillID) REFERENCES Skills(SkillID) ON DELETE CASCADE,
                UNIQUE(SessionID, ChoiceOrder)
            )
        ");

        // 플레이어가 보유한 스킬 테이블
        ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS PlayerSkills (
                PlayerID INTEGER NOT NULL,
                SkillID INTEGER NOT NULL,
                CurrentLevel INTEGER DEFAULT 1,
                AcquiredAt DATETIME DEFAULT (datetime('now', '+9 hours')),
                PRIMARY KEY (PlayerID, SkillID),
                FOREIGN KEY (PlayerID) REFERENCES Players(PlayerID) ON DELETE CASCADE,
                FOREIGN KEY (SkillID) REFERENCES Skills(SkillID) ON DELETE CASCADE
            )
        ");

        // 플레이어 무기 보유 테이블
        ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS PlayerWeapons (
                PlayerID INTEGER NOT NULL,
                WeaponID INTEGER NOT NULL,
                IsEquipped BOOLEAN DEFAULT FALSE,
                WeaponLevel INTEGER DEFAULT 1,
                UnlockedAt DATETIME DEFAULT (datetime('now', '+9 hours')),
                PRIMARY KEY (PlayerID, WeaponID),
                FOREIGN KEY (PlayerID) REFERENCES Players(PlayerID) ON DELETE CASCADE,
                FOREIGN KEY (WeaponID) REFERENCES Weapons(WeaponID) ON DELETE CASCADE
            )
        ");

        // 플레이어 발사체 보유 테이블
        ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS PlayerProjectiles (
                PlayerID INTEGER NOT NULL,
                ProjectileID INTEGER NOT NULL,
                IsActive BOOLEAN DEFAULT FALSE,
                UnlockedAt DATETIME DEFAULT (datetime('now', '+9 hours')),
                PRIMARY KEY (PlayerID, ProjectileID),
                FOREIGN KEY (PlayerID) REFERENCES Players(PlayerID) ON DELETE CASCADE,
                FOREIGN KEY (ProjectileID) REFERENCES Projectiles(ProjectileID) ON DELETE CASCADE
            )
        ");

        // 인덱스 생성
        CreateIndexes();

        // 뷰 생성
        CreateViews();

        Debug.Log("모든 데이터베이스 테이블이 성공적으로 생성되었습니다");
    }

    /// <summary>
    /// 성능 최적화를 위한 인덱스 생성
    /// </summary>
    private static void CreateIndexes()
    {
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_players_name ON Players(PlayerName)");
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_gamesessions_playerid ON GameSessions(PlayerID)");
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_gamesessions_score ON GameSessions(Score DESC)");
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_gamesessions_dates ON GameSessions(StartedAt, EndedAt)");
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_sessionskillchoices_sessionid ON SessionSkillChoices(SessionID)");
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_sessionskillchoices_choiceorder ON SessionSkillChoices(SessionID, ChoiceOrder)");
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_playerskills_playerid ON PlayerSkills(PlayerID)");
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_playerweapons_playerid ON PlayerWeapons(PlayerID)");
    }

    /// <summary>
    /// 편의를 위한 뷰 생성
    /// </summary>
    private static void CreateViews()
    {
        // 랭킹 뷰
        ExecuteNonQuery(@"
            CREATE VIEW IF NOT EXISTS RankingView AS
            SELECT 
                gs.PlayerID,
                p.PlayerName,
                gs.Score,
                gs.Level,
                gs.PlayTime,
                gs.StartedAt,
                gs.EndedAt,
                ROW_NUMBER() OVER (ORDER BY gs.Score DESC, gs.EndedAt ASC) as Rank
            FROM GameSessions gs
            JOIN Players p ON gs.PlayerID = p.PlayerID
            WHERE gs.EndedAt IS NOT NULL
            ORDER BY gs.Score DESC
        ");

        // 세션별 스킬 선택 순서 조회 뷰
        ExecuteNonQuery(@"
            CREATE VIEW IF NOT EXISTS SessionSkillChoicesView AS
            SELECT 
                ssc.SessionID,
                ssc.ChoiceOrder,
                s.SkillName,
                s.SkillType,
                ssc.PlayerLevel,
                ssc.ChosenAt,
                gs.PlayerID,
                p.PlayerName
            FROM SessionSkillChoices ssc
            JOIN Skills s ON ssc.SkillID = s.SkillID
            JOIN GameSessions gs ON ssc.SessionID = gs.SessionID
            JOIN Players p ON gs.PlayerID = p.PlayerID
            ORDER BY ssc.SessionID, ssc.ChoiceOrder
        ");
    }

    /// <summary>
    /// SELECT 쿼리 실행 후 DataReader 반환
    /// </summary>
    public static IDataReader ExecuteReader(string query, params (string name, object value)[] parameters)
    {
        var cmd = GetConnection().CreateCommand();
        cmd.CommandText = query;

        // 파라미터 추가
        foreach (var param in parameters)
        {
            var dbParam = cmd.CreateParameter();
            dbParam.ParameterName = param.name;
            dbParam.Value = param.value ?? DBNull.Value;
            cmd.Parameters.Add(dbParam);
        }

        return cmd.ExecuteReader();
    }

    /// <summary>
    /// INSERT, UPDATE, DELETE 등의 쿼리 실행
    /// </summary>
    public static int ExecuteNonQuery(string query, params (string name, object value)[] parameters)
    {
        using (var cmd = GetConnection().CreateCommand())
        {
            cmd.CommandText = query;

            // 파라미터 추가
            foreach (var param in parameters)
            {
                var dbParam = cmd.CreateParameter();
                dbParam.ParameterName = param.name;
                dbParam.Value = param.value ?? DBNull.Value;
                cmd.Parameters.Add(dbParam);
            }

            return cmd.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// 단일 값 반환하는 쿼리 실행
    /// </summary>
    public static object ExecuteScalar(string query, params (string name, object value)[] parameters)
    {
        using (var cmd = GetConnection().CreateCommand())
        {
            cmd.CommandText = query;

            // 파라미터 추가
            foreach (var param in parameters)
            {
                var dbParam = cmd.CreateParameter();
                dbParam.ParameterName = param.name;
                dbParam.Value = param.value ?? DBNull.Value;
                cmd.Parameters.Add(dbParam);
            }

            return cmd.ExecuteScalar();
        }
    }

    /// <summary>
    /// 데이터베이스 버전 정보를 가져옵니다
    /// </summary>
    private static int GetDatabaseVersion()
    {
        try
        {
            // 버전 테이블이 있는지 확인
            var result = ExecuteScalar(@"
                SELECT name FROM sqlite_master 
                WHERE type='table' AND name='DatabaseVersion'
            ");

            if (result == null)
            {
                return 0; // 버전 테이블이 없으면 구버전으로 간주
            }

            // 버전 조회
            var version = ExecuteScalar("SELECT Version FROM DatabaseVersion WHERE ID = 1");
            return version != null ? (int)(long)version : 0;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"데이터베이스 버전 조회 오류: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 데이터베이스 버전 정보를 설정합니다
    /// </summary>
    private static void SetDatabaseVersion(int version)
    {
        try
        {
            // 버전 테이블 생성
            ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS DatabaseVersion (
                    ID INTEGER PRIMARY KEY DEFAULT 1,
                    Version INTEGER NOT NULL,
                    UpdatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                    CHECK (ID = 1)
                )
            ");

            // 버전 정보 삽입 또는 업데이트
            ExecuteNonQuery(@"
                INSERT OR REPLACE INTO DatabaseVersion (ID, Version, UpdatedAt) 
                VALUES (1, @version, datetime('now', '+9 hours'))
            ", ("@version", version));
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"데이터베이스 버전 설정 오류: {ex.Message}");
        }
    }

    /// <summary>
    /// 데이터베이스를 완전히 초기화합니다 (파일 삭제 후 재생성)
    /// </summary>
    private static void ResetDatabase()
    {
        try
        {
            // 1. 기존 연결 종료
            Close();

            // 2. 데이터베이스 파일과 관련 파일들 삭제
            if (File.Exists(dbPath))
            {
                File.Delete(dbPath);
                Debug.Log("기존 데이터베이스 파일 삭제됨");
            }

            // 3. SQLite WAL 및 SHM 파일도 삭제 (있다면)
            string walPath = dbPath + "-wal";
            string shmPath = dbPath + "-shm";

            if (File.Exists(walPath))
            {
                File.Delete(walPath);
                Debug.Log("WAL 파일 삭제됨");
            }

            if (File.Exists(shmPath))
            {
                File.Delete(shmPath);
                Debug.Log("SHM 파일 삭제됨");
            }

            // 4. 새로운 연결 생성
            connection = new SqliteConnection("URI=file:" + dbPath);
            connection.Open();

            Debug.Log("데이터베이스가 완전히 초기화되었습니다");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"데이터베이스 초기화 오류: {ex.Message}");
        }
    }

    /// <summary>
    /// 현재 데이터베이스 버전을 반환합니다
    /// </summary>
    public static int GetCurrentDatabaseVersion()
    {
        return CURRENT_DB_VERSION;
    }
}
