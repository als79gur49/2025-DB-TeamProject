using System.Collections.Generic;
using System.Data;
using UnityEngine;

/// <summary>
/// 랭킹 관련 데이터베이스 작업을 담당하는 클래스
/// </summary>
public static class RankingRepository
{
    /// <summary>
    /// 플레이어 점수 추가 또는 업데이트
    /// </summary>
    public static bool UpsertPlayerScore(string playerName, int score)
    {
        try
        {
            string query = @"
                INSERT INTO Rankings (PlayerName, Score, UpdatedAt) 
                VALUES (@playerName, @score, CURRENT_TIMESTAMP)
                ON CONFLICT(PlayerName) DO UPDATE SET 
                    Score = @score, 
                    UpdatedAt = CURRENT_TIMESTAMP
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query,
                ("@playerName", playerName),
                ("@score", score));

            return rowsAffected > 0;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"플레이어 점수 업데이트 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 플레이어 점수 조회
    /// </summary>
    public static int GetPlayerScore(string playerName)
    {
        try
        {
            string query = "SELECT Score FROM Rankings WHERE PlayerName = @playerName";

            var result = DatabaseManager.ExecuteScalar(query, ("@playerName", playerName));

            return result != null ? (int)(long)result : 0;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"플레이어 점수 조회 오류: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 상위 N명의 랭킹 조회
    /// </summary>
    public static List<RankingData> GetTopRankings(int limit = 10)
    {
        var rankings = new List<RankingData>();

        try
        {
            string query = @"
                SELECT PlayerName, Score 
                FROM Rankings 
                ORDER BY Score DESC, UpdatedAt ASC 
                LIMIT @limit
            ";

            using (var reader = DatabaseManager.ExecuteReader(query, ("@limit", limit)))
            {
                while (reader.Read())
                {
                    string playerName = reader["PlayerName"].ToString();
                    int score = (int)(long)reader["Score"];

                    rankings.Add(new RankingData(playerName, score));
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"상위 랭킹 조회 오류: {ex.Message}");
        }

        return rankings;
    }

    /// <summary>
    /// 모든 랭킹 조회 (점수 내림차순)
    /// </summary>
    public static List<RankingData> GetAllRankings()
    {
        var rankings = new List<RankingData>();

        try
        {
            string query = @"
                SELECT PlayerName, Score 
                FROM Rankings 
                ORDER BY Score DESC, UpdatedAt ASC
            ";

            using (var reader = DatabaseManager.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    string playerName = reader["PlayerName"].ToString();
                    int score = (int)(long)reader["Score"];

                    rankings.Add(new RankingData(playerName, score));
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"전체 랭킹 조회 오류: {ex.Message}");
        }

        return rankings;
    }

    /// <summary>
    /// 특정 플레이어의 순위 조회
    /// </summary>
    public static int GetPlayerRank(string playerName)
    {
        try
        {
            string query = @"
                SELECT COUNT(*) + 1 as Rank
                FROM Rankings 
                WHERE Score > (
                    SELECT Score 
                    FROM Rankings 
                    WHERE PlayerName = @playerName
                )
            ";

            var result = DatabaseManager.ExecuteScalar(query, ("@playerName", playerName));

            return result != null ? (int)(long)result : -1;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"플레이어 순위 조회 오류: {ex.Message}");
            return -1;
        }
    }

    /// <summary>
    /// 플레이어 삭제
    /// </summary>
    public static bool DeletePlayer(string playerName)
    {
        try
        {
            string query = "DELETE FROM Rankings WHERE PlayerName = @playerName";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query, ("@playerName", playerName));

            return rowsAffected > 0;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"플레이어 삭제 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 전체 랭킹 데이터 삭제
    /// </summary>
    public static bool ClearAllRankings()
    {
        try
        {
            string query = "DELETE FROM Rankings";

            DatabaseManager.ExecuteNonQuery(query);

            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"랭킹 전체 삭제 오류: {ex.Message}");
            return false;
        }
    }
}
