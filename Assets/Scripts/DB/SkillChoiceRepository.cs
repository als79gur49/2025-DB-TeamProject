using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

/// <summary>
/// 세션별 스킬 선택 기록 관련 데이터베이스 작업을 담당하는 클래스
/// </summary>
public static class SkillChoiceRepository
{
    /// <summary>
    /// 스킬 선택 기록 추가
    /// </summary>
    public static bool AddSkillChoice(int sessionId, int skillId, int choiceOrder, int playerLevel)
    {
        try
        {
            string query = @"
                INSERT INTO SessionSkillChoices (SessionID, SkillID, ChoiceOrder, PlayerLevel, ChosenAt)
                VALUES (@sessionId, @skillId, @choiceOrder, @playerLevel, datetime('now', '+9 hours'))
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query,
                ("@sessionId", sessionId),
                ("@skillId", skillId),
                ("@choiceOrder", choiceOrder),
                ("@playerLevel", playerLevel));

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"스킬 선택 기록 추가 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 특정 세션의 모든 스킬 선택 기록 조회
    /// </summary>
    public static List<SkillChoiceModel> GetSessionSkillChoices(int sessionId)
    {
        var skillChoices = new List<SkillChoiceModel>();

        try
        {
            string query = @"
                SELECT ssc.ChoiceID, ssc.SessionID, ssc.SkillID, ssc.ChoiceOrder, 
                       ssc.PlayerLevel, ssc.ChosenAt, s.SkillName, s.SkillType
                FROM SessionSkillChoices ssc
                LEFT JOIN Skills s ON ssc.SkillID = s.SkillID
                WHERE ssc.SessionID = @sessionId
                ORDER BY ssc.ChoiceOrder
            ";

            using (var reader = DatabaseManager.ExecuteReader(query, ("@sessionId", sessionId)))
            {
                while (reader.Read())
                {
                    skillChoices.Add(new SkillChoiceModel
                    {
                        ChoiceID = (int)(long)reader["ChoiceID"],
                        SessionID = (int)(long)reader["SessionID"],
                        SkillID = (int)(long)reader["SkillID"],
                        ChoiceOrder = (int)(long)reader["ChoiceOrder"],
                        PlayerLevel = (int)(long)reader["PlayerLevel"],
                        ChosenAt = DateTime.Parse(reader["ChosenAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeLocal),
                        SkillName = reader["SkillName"]?.ToString() ?? "Unknown",
                        SkillType = reader["SkillType"]?.ToString() ?? "Unknown"
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"세션 스킬 선택 조회 오류: {ex.Message}");
        }

        return skillChoices;
    }

    /// <summary>
    /// 플레이어가 가장 자주 선택하는 스킬 조회 (전체)
    /// </summary>
    public static List<(int skillId, string skillName, int selectCount)> GetMostSelectedSkills(int playerId, int limit = 10)
    {
        var results = new List<(int, string, int)>();

        try
        {
            string query = @"
                SELECT s.SkillID, s.SkillName, COUNT(*) as SelectCount
                FROM SessionSkillChoices ssc
                JOIN Skills s ON ssc.SkillID = s.SkillID
                JOIN GameSessions gs ON ssc.SessionID = gs.SessionID
                WHERE gs.PlayerID = @playerId
                GROUP BY s.SkillID, s.SkillName
                ORDER BY SelectCount DESC
                LIMIT @limit
            ";

            using (var reader = DatabaseManager.ExecuteReader(query,
                ("@playerId", playerId),
                ("@limit", limit)))
            {
                while (reader.Read())
                {
                    results.Add((
                        (int)(long)reader["SkillID"],
                        reader["SkillName"].ToString(),
                        (int)(long)reader["SelectCount"]
                    ));
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"가장 많이 선택한 스킬 조회 오류: {ex.Message}");
        }

        return results;
    }

    /// <summary>
    /// 특정 순서에서 가장 자주 선택되는 스킬 조회
    /// </summary>
    public static List<(int skillId, string skillName, int selectCount)> GetMostSelectedSkillsByOrder(int playerId, int choiceOrder, int limit = 5)
    {
        var results = new List<(int, string, int)>();

        try
        {
            string query = @"
                SELECT s.SkillID, s.SkillName, COUNT(*) as SelectCount
                FROM SessionSkillChoices ssc
                JOIN Skills s ON ssc.SkillID = s.SkillID
                JOIN GameSessions gs ON ssc.SessionID = gs.SessionID
                WHERE gs.PlayerID = @playerId AND ssc.ChoiceOrder = @choiceOrder
                GROUP BY s.SkillID, s.SkillName
                ORDER BY SelectCount DESC
                LIMIT @limit
            ";

            using (var reader = DatabaseManager.ExecuteReader(query,
                ("@playerId", playerId),
                ("@choiceOrder", choiceOrder),
                ("@limit", limit)))
            {
                while (reader.Read())
                {
                    results.Add((
                        (int)(long)reader["SkillID"],
                        reader["SkillName"].ToString(),
                        (int)(long)reader["SelectCount"]
                    ));
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"순서별 스킬 선택 통계 조회 오류: {ex.Message}");
        }

        return results;
    }

    /// <summary>
    /// 전체 플레이어들의 스킬 선택 통계 (인기 스킬)
    /// </summary>
    public static List<(int skillId, string skillName, int selectCount, double selectRate)> GetGlobalSkillStats(int limit = 10)
    {
        var results = new List<(int, string, int, double)>();

        try
        {
            // 전체 선택 횟수를 먼저 구함
            var totalChoices = DatabaseManager.ExecuteScalar("SELECT COUNT(*) FROM SessionSkillChoices");
            int totalCount = totalChoices != null ? (int)(long)totalChoices : 1;

            string query = @"
                SELECT s.SkillID, s.SkillName, COUNT(*) as SelectCount,
                       CAST(COUNT(*) AS REAL) * 100.0 / @totalCount as SelectRate
                FROM SessionSkillChoices ssc
                JOIN Skills s ON ssc.SkillID = s.SkillID
                GROUP BY s.SkillID, s.SkillName
                ORDER BY SelectCount DESC
                LIMIT @limit
            ";

            using (var reader = DatabaseManager.ExecuteReader(query,
                ("@totalCount", totalCount),
                ("@limit", limit)))
            {
                while (reader.Read())
                {
                    results.Add((
                        (int)(long)reader["SkillID"],
                        reader["SkillName"].ToString(),
                        (int)(long)reader["SelectCount"],
                        (double)reader["SelectRate"]
                    ));
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"전체 스킬 통계 조회 오류: {ex.Message}");
        }

        return results;
    }

    /// <summary>
    /// 특정 세션의 스킬 선택 순서 요약 조회
    /// </summary>
    public static List<string> GetSessionSkillSequence(int sessionId)
    {
        var skillSequence = new List<string>();

        try
        {
            string query = @"
                SELECT s.SkillName
                FROM SessionSkillChoices ssc
                JOIN Skills s ON ssc.SkillID = s.SkillID
                WHERE ssc.SessionID = @sessionId
                ORDER BY ssc.ChoiceOrder
            ";

            using (var reader = DatabaseManager.ExecuteReader(query, ("@sessionId", sessionId)))
            {
                while (reader.Read())
                {
                    skillSequence.Add(reader["SkillName"].ToString());
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"세션 스킬 순서 조회 오류: {ex.Message}");
        }

        return skillSequence;
    }

    /// <summary>
    /// 세션의 스킬 선택 기록 삭제
    /// </summary>
    public static bool DeleteSessionSkillChoices(int sessionId)
    {
        try
        {
            string query = "DELETE FROM SessionSkillChoices WHERE SessionID = @sessionId";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query, ("@sessionId", sessionId));

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"세션 스킬 선택 기록 삭제 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 플레이어의 모든 스킬 선택 기록 삭제
    /// </summary>
    public static bool DeletePlayerSkillChoices(int playerId)
    {
        try
        {
            string query = @"
                DELETE FROM SessionSkillChoices 
                WHERE SessionID IN (
                    SELECT SessionID FROM GameSessions WHERE PlayerID = @playerId
                )
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query, ("@playerId", playerId));

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"플레이어 스킬 선택 기록 삭제 오류: {ex.Message}");
            return false;
        }
    }
}
