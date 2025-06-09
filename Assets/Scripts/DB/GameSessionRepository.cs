using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

/// <summary>
/// 게임 세션 관련 데이터베이스 작업을 담당하는 클래스
/// </summary>
public static class GameSessionRepository
{
    /// <summary>
    /// 새 게임 세션 시작
    /// </summary>
    public static GameSessionModel StartNewSession(int playerId)
    {
        try
        {
            string query = @"
                INSERT INTO GameSessions (PlayerID, Score, Level, EnemiesKilled, DeathCount, StartedAt)
                VALUES (@playerId, 0, 1, 0, 0, datetime('now', '+9 hours'))
            ";

            DatabaseManager.ExecuteNonQuery(query, ("@playerId", playerId));

            // 생성된 세션 ID 조회
            var sessionId = DatabaseManager.ExecuteScalar("SELECT last_insert_rowid()");

            if (sessionId != null)
            {
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
                SELECT SessionID, PlayerID, Score, Level, EnemiesKilled, DeathCount,
                       StartedAt, EndedAt, PlayTime
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
                        PlayerID = (int)(long)reader["PlayerID"],
                        Score = (int)(long)reader["Score"],
                        Level = (int)(long)reader["Level"],
                        EnemiesKilled = (int)(long)reader["EnemiesKilled"],
                        DeathCount = (int)(long)reader["DeathCount"],
                        StartedAt = DateTime.Parse(reader["StartedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeLocal),
                        EndedAt = reader["EndedAt"] == DBNull.Value ? null : DateTime.Parse(reader["EndedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeLocal),
                        PlayTime = reader["PlayTime"] == DBNull.Value ? null : (int)(long)reader["PlayTime"]
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
    /// 세션 정보 업데이트 (진행 중)
    /// </summary>
    public static bool UpdateSession(GameSessionModel session)
    {
        try
        {
            string query = @"
                UPDATE GameSessions SET
                    Score = @score,
                    Level = @level,
                    EnemiesKilled = @enemiesKilled,
                    DeathCount = @deathCount
                WHERE SessionID = @sessionId
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query,
                ("@score", session.Score),
                ("@level", session.Level),
                ("@enemiesKilled", session.EnemiesKilled),
                ("@deathCount", session.DeathCount),
                ("@sessionId", session.SessionID));

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"게임 세션 업데이트 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 게임 세션 종료
    /// </summary>
    public static bool EndSession(int sessionId, int finalScore, int finalLevel, int enemiesKilled, int deathCount)
    {
        try
        {
            // PlayTime은 C#에서 계산해서 정수로 저장 (부동소수점 문제)
            var session = GetSessionById(sessionId);
            if (session == null)
            {
                Debug.LogError($"세션 ID {sessionId}를 찾을 수 없습니다.");
                return false;
            }

            DateTime endTime = DateTime.Now;
            DateTime startTime = session.StartedAt;

            if (startTime.Kind != endTime.Kind)
            {
                // StartedAt이 Unspecified라면 Local로 가정
                if (startTime.Kind == DateTimeKind.Unspecified)
                {
                    startTime = DateTime.SpecifyKind(startTime, DateTimeKind.Local);
                }
            }

            int playTimeSeconds = (int)(endTime - startTime).TotalSeconds;

            string query = @"
                UPDATE GameSessions SET
                    Score = @finalScore,
                    Level = @finalLevel,
                    EnemiesKilled = @enemiesKilled,
                    DeathCount = @deathCount,
                    EndedAt = datetime('now', '+9 hours'),
                    PlayTime = @playTime
                WHERE SessionID = @sessionId
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query,
                ("@finalScore", finalScore),
                ("@finalLevel", finalLevel),
                ("@enemiesKilled", enemiesKilled),
                ("@deathCount", deathCount),
                ("@playTime", playTimeSeconds),
                ("@sessionId", sessionId));

            Debug.Log($"게임 세션 종료: PlayTime = {playTimeSeconds}초 ({playTimeSeconds / 60}분 {playTimeSeconds % 60}초)");

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"게임 세션 종료 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 플레이어의 모든 완료된 세션 조회
    /// </summary>
    public static List<GameSessionModel> GetPlayerSessions(int playerId, int limit = 20)
    {
        var sessions = new List<GameSessionModel>();

        try
        {
            string query = @"
                SELECT SessionID, PlayerID, Score, Level, EnemiesKilled, DeathCount,
                       StartedAt, EndedAt, PlayTime
                FROM GameSessions
                WHERE PlayerID = @playerId AND EndedAt IS NOT NULL
                ORDER BY StartedAt DESC
                LIMIT @limit
            ";

            using (var reader = DatabaseManager.ExecuteReader(query,
                ("@playerId", playerId),
                ("@limit", limit)))
            {
                while (reader.Read())
                {
                    sessions.Add(new GameSessionModel
                    {
                        SessionID = (int)(long)reader["SessionID"],
                        PlayerID = (int)(long)reader["PlayerID"],
                        Score = (int)(long)reader["Score"],
                        Level = (int)(long)reader["Level"],
                        EnemiesKilled = (int)(long)reader["EnemiesKilled"],
                        DeathCount = (int)(long)reader["DeathCount"],
                        StartedAt = DateTime.Parse(reader["StartedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeLocal),
                        EndedAt = DateTime.Parse(reader["EndedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeLocal),
                        PlayTime = (int)(long)reader["PlayTime"]
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"플레이어 세션 조회 오류: {ex.Message}");
        }

        return sessions;
    }

    /// <summary>
    /// 진행 중인 세션 조회 (EndedAt이 NULL인 세션)
    /// </summary>
    public static GameSessionModel GetActiveSession(int playerId)
    {
        try
        {
            string query = @"
                SELECT SessionID, PlayerID, Score, Level, EnemiesKilled, DeathCount,
                       StartedAt, EndedAt, PlayTime
                FROM GameSessions
                WHERE PlayerID = @playerId AND EndedAt IS NULL
                ORDER BY StartedAt DESC
                LIMIT 1
            ";

            using (var reader = DatabaseManager.ExecuteReader(query, ("@playerId", playerId)))
            {
                if (reader.Read())
                {
                    return new GameSessionModel
                    {
                        SessionID = (int)(long)reader["SessionID"],
                        PlayerID = (int)(long)reader["PlayerID"],
                        Score = (int)(long)reader["Score"],
                        Level = (int)(long)reader["Level"],
                        EnemiesKilled = (int)(long)reader["EnemiesKilled"],
                        DeathCount = (int)(long)reader["DeathCount"],
                        StartedAt = DateTime.Parse(reader["StartedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeLocal),
                        EndedAt = null,
                        PlayTime = null
                    };
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"활성 세션 조회 오류: {ex.Message}");
            return null;
        }
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
    /// 플레이어의 세션 통계 조회
    /// </summary>
    public static (int totalGames, int totalPlayTime, int averageScore, int bestScore) GetPlayerStats(int playerId)
    {
        try
        {
            string query = @"
                SELECT 
                    COUNT(*) as TotalGames,
                    COALESCE(SUM(PlayTime), 0) as TotalPlayTime,
                    COALESCE(AVG(Score), 0) as AverageScore,
                    COALESCE(MAX(Score), 0) as BestScore
                FROM GameSessions
                WHERE PlayerID = @playerId AND EndedAt IS NOT NULL
            ";

            using (var reader = DatabaseManager.ExecuteReader(query, ("@playerId", playerId)))
            {
                if (reader.Read())
                {
                    return (
                        totalGames: (int)(long)reader["TotalGames"],
                        totalPlayTime: (int)(long)reader["TotalPlayTime"],
                        averageScore: (int)(double)reader["AverageScore"],
                        bestScore: (int)(long)reader["BestScore"]
                    );
                }
            }

            return (0, 0, 0, 0);
        }
        catch (Exception ex)
        {
            Debug.LogError($"플레이어 통계 조회 오류: {ex.Message}");
            return (0, 0, 0, 0);
        }
    }
}
