using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

/// <summary>
/// 플레이어 관련 데이터베이스 작업을 담당하는 클래스
/// </summary>
public static class PlayerRepository
{
    /// <summary>
    /// 새 플레이어 생성
    /// </summary>
    public static PlayerModel CreatePlayer(string playerName)
    {
        try
        {
            string query = @"
                INSERT INTO Players (PlayerName, CreatedAt, LastPlayedAt)
                VALUES (@playerName, datetime('now', '+9 hours'), datetime('now', '+9 hours'))
            ";

            DatabaseManager.ExecuteNonQuery(query, ("@playerName", playerName));

            // 생성된 플레이어 ID 조회
            var playerId = DatabaseManager.ExecuteScalar("SELECT last_insert_rowid()");

            if (playerId != null)
            {
                return GetPlayerById((int)(long)playerId);
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"플레이어 생성 오류: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// PlayerID로 플레이어 조회
    /// </summary>
    public static PlayerModel GetPlayerById(int playerId)
    {
        try
        {
            string query = @"
                SELECT PlayerID, PlayerName, CurrentLevel, CurrentExp, ExpToNextLevel,
                       HighestScore, TotalPlayTime, CreatedAt, LastPlayedAt
                FROM Players
                WHERE PlayerID = @playerId
            ";

            using (var reader = DatabaseManager.ExecuteReader(query, ("@playerId", playerId)))
            {
                if (reader.Read())
                {
                    return new PlayerModel
                    {
                        PlayerID = (int)(long)reader["PlayerID"],
                        PlayerName = reader["PlayerName"].ToString(),
                        CurrentLevel = (int)(long)reader["CurrentLevel"],
                        CurrentExp = (int)(long)reader["CurrentExp"],
                        ExpToNextLevel = (int)(long)reader["ExpToNextLevel"],
                        HighestScore = (int)(long)reader["HighestScore"],
                        TotalPlayTime = (int)(long)reader["TotalPlayTime"],
                        CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeLocal),
                        LastPlayedAt = DateTime.Parse(reader["LastPlayedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeLocal)
                    };
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"플레이어 조회 오류: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 플레이어 정보 업데이트
    /// </summary>
    public static bool UpdatePlayer(PlayerModel player)
    {
        try
        {
            string query = @"
                UPDATE Players SET
                    CurrentLevel = @currentLevel,
                    CurrentExp = @currentExp,
                    ExpToNextLevel = @expToNextLevel,
                    HighestScore = @highestScore,
                    TotalPlayTime = @totalPlayTime,
                    LastPlayedAt = datetime('now', '+9 hours')
                WHERE PlayerID = @playerId
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query,
                ("@currentLevel", player.CurrentLevel),
                ("@currentExp", player.CurrentExp),
                ("@expToNextLevel", player.ExpToNextLevel),
                ("@highestScore", player.HighestScore),
                ("@totalPlayTime", player.TotalPlayTime),
                ("@playerId", player.PlayerID));

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"플레이어 업데이트 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 플레이어의 최고 점수 업데이트 (새 점수가 더 높을 때만)
    /// </summary>
    public static bool UpdateHighestScore(int playerId, int newScore)
    {
        try
        {
            string query = @"
                UPDATE Players SET
                    HighestScore = @newScore,
                    LastPlayedAt = datetime('now', '+9 hours')
                WHERE PlayerID = @playerId AND HighestScore < @newScore
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query,
                ("@newScore", newScore),
                ("@playerId", playerId));

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"최고 점수 업데이트 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 모든 플레이어 목록 조회 (최근 플레이 순)
    /// </summary>
    public static List<PlayerModel> GetAllPlayers(int limit = 50)
    {
        var players = new List<PlayerModel>();

        try
        {
            string query = @"
                SELECT PlayerID, PlayerName, CurrentLevel, CurrentExp, ExpToNextLevel,
                       HighestScore, TotalPlayTime, CreatedAt, LastPlayedAt
                FROM Players
                ORDER BY LastPlayedAt DESC
                LIMIT @limit
            ";

            using (var reader = DatabaseManager.ExecuteReader(query, ("@limit", limit)))
            {
                while (reader.Read())
                {
                    players.Add(new PlayerModel
                    {
                        PlayerID = (int)(long)reader["PlayerID"],
                        PlayerName = reader["PlayerName"].ToString(),
                        CurrentLevel = (int)(long)reader["CurrentLevel"],
                        CurrentExp = (int)(long)reader["CurrentExp"],
                        ExpToNextLevel = (int)(long)reader["ExpToNextLevel"],
                        HighestScore = (int)(long)reader["HighestScore"],
                        TotalPlayTime = (int)(long)reader["TotalPlayTime"],
                        CreatedAt = DateTime.Parse(reader["CreatedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeLocal),
                        LastPlayedAt = DateTime.Parse(reader["LastPlayedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeLocal)
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"전체 플레이어 조회 오류: {ex.Message}");
        }

        return players;
    }

    /// <summary>
    /// 플레이어 삭제
    /// </summary>
    public static bool DeletePlayer(int playerId)
    {
        try
        {
            string query = "DELETE FROM Players WHERE PlayerID = @playerId";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query, ("@playerId", playerId));

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"플레이어 삭제 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 플레이어 총 수 조회
    /// </summary>
    public static int GetPlayerCount()
    {
        try
        {
            var result = DatabaseManager.ExecuteScalar("SELECT COUNT(*) FROM Players");
            return result != null ? (int)(long)result : 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"플레이어 수 조회 오류: {ex.Message}");
            return 0;
        }
    }
}
