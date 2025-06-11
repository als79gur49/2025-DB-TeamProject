using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

/// <summary>
/// 게임 세션 관련 데이터베이스 작업을 담당하는 클래스
/// 새로운 구조: 세션 기본 정보만 관리, 엔티티는 별도 관리
/// </summary>
public static class GameSessionRepository
{
    /// <summary>
    /// 새 게임 세션 시작 (엔티티 독립적)
    /// </summary>
    public static GameSessionModel StartNewSession()
    {
        try
        {
            string query = @"
                INSERT INTO GameSessions (TotalEntities, PlayTimeSeconds, StartedAt, EndedAt, IsCompleted)
                VALUES (0, 0, datetime('now'), NULL, FALSE)
            ";

            DatabaseManager.ExecuteNonQuery(query);

            // 생성된 세션 ID 조회
            var sessionId = DatabaseManager.ExecuteScalar("SELECT last_insert_rowid()");

            if (sessionId != null)
            {
                Debug.Log($"새 게임 세션 시작: SessionID={sessionId}");
                return GetSessionById((int)(long)sessionId);
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"게임 세션 시작 오류: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// SessionID로 세션 조회
    /// </summary>
    public static GameSessionModel GetSessionById(int sessionId)
    {
        try
        {
            string query = @"
                SELECT SessionID, TotalEntities, PlayTimeSeconds, StartedAt, EndedAt, IsCompleted
                FROM GameSessions
                WHERE SessionID = @sessionId
            ";

            using (var reader = DatabaseManager.ExecuteReader(query, ("@sessionId", sessionId)))
            {
                if (reader.Read())
                {
                    return new GameSessionModel
                    {
                        SessionID = (int)(long)reader["SessionID"],
                        TotalEntities = (int)(long)reader["TotalEntities"],
                        PlayTimeSeconds = (int)(long)reader["PlayTimeSeconds"],
                        StartedAt = DatabaseManager.ConvertUtcToLocal(DateTime.Parse(reader["StartedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal)),
                        EndedAt = reader["EndedAt"] == DBNull.Value ? null : DatabaseManager.ConvertUtcToLocal(DateTime.Parse(reader["EndedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal)),
                        IsCompleted = (bool)reader["IsCompleted"]
                    };
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"게임 세션 조회 오류: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 세션의 엔티티 수 업데이트
    /// </summary>
    public static bool UpdateSessionEntityCount(int sessionId, int totalEntities)
    {
        try
        {
            string query = @"
                UPDATE GameSessions SET
                    TotalEntities = @totalEntities
                WHERE SessionID = @sessionId
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query,
                ("@totalEntities", totalEntities),
                ("@sessionId", sessionId));

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"세션 엔티티 수 업데이트 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 게임 세션 종료
    /// </summary>
    public static bool EndSession(int sessionId, int totalEntities)
    {
        try
        {
            var session = GetSessionById(sessionId);
            if (session == null)
            {
                Debug.LogError($"세션 ID {sessionId}를 찾을 수 없습니다.");
                return false;
            }

            DateTime endTime = DateTime.Now;
            DateTime startTime = session.StartedAt;
            int playTimeSeconds = (int)(endTime - startTime).TotalSeconds;

            string query = @"
                UPDATE GameSessions SET
                    TotalEntities = @totalEntities,
                    EndedAt = datetime('now'),
                    PlayTimeSeconds = @playTime,
                    IsCompleted = TRUE
                WHERE SessionID = @sessionId
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query,
                ("@totalEntities", totalEntities),
                ("@playTime", playTimeSeconds),
                ("@sessionId", sessionId));

            Debug.Log($"게임 세션 종료: SessionID={sessionId}, PlayTime={playTimeSeconds}초 ({playTimeSeconds / 60}분 {playTimeSeconds % 60}초)");

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"게임 세션 종료 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 완료된 모든 세션 조회
    /// </summary>
    public static List<GameSessionModel> GetCompletedSessions(int limit = 20)
    {
        var sessions = new List<GameSessionModel>();

        try
        {
            string query = @"
                SELECT SessionID, TotalEntities, PlayTimeSeconds, StartedAt, EndedAt, IsCompleted
                FROM GameSessions
                WHERE IsCompleted = TRUE
                ORDER BY EndedAt DESC
                LIMIT @limit
            ";

            using (var reader = DatabaseManager.ExecuteReader(query, ("@limit", limit)))
            {
                while (reader.Read())
                {
                    sessions.Add(new GameSessionModel
                    {
                        SessionID = (int)(long)reader["SessionID"],
                        TotalEntities = (int)(long)reader["TotalEntities"],
                        PlayTimeSeconds = (int)(long)reader["PlayTimeSeconds"],
                        StartedAt = DatabaseManager.ConvertUtcToLocal(DateTime.Parse(reader["StartedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal)),
                        EndedAt = DatabaseManager.ConvertUtcToLocal(DateTime.Parse(reader["EndedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal)),
                        IsCompleted = (bool)reader["IsCompleted"]
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"완료된 세션 조회 오류: {ex.Message}");
        }

        return sessions;
    }

    /// <summary>
    /// 진행 중인 세션 조회 (EndedAt이 NULL인 세션)
    /// </summary>
    public static List<GameSessionModel> GetActiveSessions()
    {
        var sessions = new List<GameSessionModel>();

        try
        {
            string query = @"
                SELECT SessionID, TotalEntities, PlayTimeSeconds, StartedAt, EndedAt, IsCompleted
                FROM GameSessions
                WHERE IsCompleted = FALSE AND EndedAt IS NULL
                ORDER BY StartedAt DESC
            ";

            using (var reader = DatabaseManager.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    sessions.Add(new GameSessionModel
                    {
                        SessionID = (int)(long)reader["SessionID"],
                        TotalEntities = (int)(long)reader["TotalEntities"],
                        PlayTimeSeconds = (int)(long)reader["PlayTimeSeconds"],
                        StartedAt = DatabaseManager.ConvertUtcToLocal(DateTime.Parse(reader["StartedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal)),
                        EndedAt = null,
                        IsCompleted = (bool)reader["IsCompleted"]
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"활성 세션 조회 오류: {ex.Message}");
        }

        return sessions;
    }

    /// <summary>
    /// 세션 삭제
    /// </summary>
    public static bool DeleteSession(int sessionId)
    {
        try
        {
            string query = "DELETE FROM GameSessions WHERE SessionID = @sessionId";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query, ("@sessionId", sessionId));

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"게임 세션 삭제 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 특정 세션의 상세 정보 조회 (엔티티 포함)
    /// </summary>
    public static SessionDetailModel GetSessionDetail(int sessionId)
    {
        try
        {
            var session = GetSessionById(sessionId);
            if (session == null) return null;

            var entities = SessionEntityRepository.GetSessionEntities(sessionId);

            return new SessionDetailModel
            {
                Session = session,
                Entities = entities,
                TotalEntities = entities.Count,
                AliveEntities = entities.FindAll(e => e.IsAlive).Count,
                PlayerEntities = entities.FindAll(e => e.EntityType == "Player").Count,
                AIEntities = entities.FindAll(e => e.EntityType == "AI").Count
            };
        }
        catch (Exception ex)
        {
            Debug.LogError($"세션 상세 정보 조회 오류: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 총 게임 수 조회
    /// </summary>
    public static int GetTotalCompletedGameCount()
    {
        try
        {
            string query = "SELECT COUNT(*) FROM GameSessions WHERE IsCompleted = TRUE";

            var result = DatabaseManager.ExecuteScalar(query);

            return result != null ? (int)(long)result : 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"총 게임 수 조회 오류: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 평균 플레이 시간 조회 (완료된 게임 기준)
    /// </summary>
    public static int GetAveragePlayTime()
    {
        try
        {
            string query = "SELECT AVG(PlayTimeSeconds) FROM GameSessions WHERE IsCompleted = TRUE";

            var result = DatabaseManager.ExecuteScalar(query);

            return result != null && result != DBNull.Value ? (int)(double)result : 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"평균 플레이 시간 조회 오류: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 가장 오래된 완료되지 않은 세션 강제 완료 (정리용)
    /// </summary>
    public static bool ForceCompleteOldestIncompleteSession()
    {
        try
        {
            string query = @"
                UPDATE GameSessions SET
                    EndedAt = datetime('now'),
                    IsCompleted = TRUE,
                    PlayTimeSeconds = COALESCE(
                        (julianday('now') - julianday(StartedAt)) * 86400,
                        0
                    )
                WHERE SessionID = (
                    SELECT SessionID FROM GameSessions 
                    WHERE IsCompleted = FALSE 
                    ORDER BY StartedAt ASC 
                    LIMIT 1
                )
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query);

            if (rowsAffected > 0)
            {
                Debug.Log("가장 오래된 미완료 세션을 강제 완료했습니다.");
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"미완료 세션 강제 완료 오류: {ex.Message}");
            return false;
        }
    }
}

/// <summary>
/// 세션 상세 정보 모델
/// </summary>
[System.Serializable]
public class SessionDetailModel
{
    public GameSessionModel Session;
    public List<SessionEntityModel> Entities;
    public int TotalEntities;
    public int AliveEntities;
    public int PlayerEntities;
    public int AIEntities;

    public SessionDetailModel()
    {
        Entities = new List<SessionEntityModel>();
    }

    /// <summary>
    /// 세션 요약 정보 반환
    /// </summary>
    public string GetSummary()
    {
        if (Session == null) return "세션 정보 없음";
        
        string status = Session.IsCompleted ? "완료" : "진행중";
        string playTime = Session.IsCompleted ? Session.GetFormattedPlayTime() : "진행중";
        
        return $"세션 {Session.SessionID}: {TotalEntities}개 엔티티 ({PlayerEntities}명 플레이어, {AIEntities}개 AI), {status} ({playTime})";
    }
}
