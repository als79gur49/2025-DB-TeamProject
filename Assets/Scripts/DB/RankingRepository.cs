using System.Collections.Generic;
using System.Data;
using UnityEngine;

/// <summary>
/// 랭킹 관련 데이터베이스 작업을 담당하는 클래스
/// </summary>
public static class RankingRepository
{
    /// <summary>
    /// 상위 N명의 랭킹 조회 (게임 세션 기반)
    /// </summary>
    public static List<RankingData> GetTopRankings(int limit = 10)
    {
        var rankings = new List<RankingData>();

        try
        {
            string query = @"
                SELECT PlayerID, PlayerName, Score, Level, PlayTime, StartedAt, EndedAt, Rank
                FROM RankingView
                LIMIT @limit
            ";

            using (var reader = DatabaseManager.ExecuteReader(query, ("@limit", limit)))
            {
                while (reader.Read())
                {
                    rankings.Add(new RankingData
                    {
                        PlayerID = (int)(long)reader["PlayerID"],
                        PlayerName = reader["PlayerName"].ToString(),
                        Score = (int)(long)reader["Score"],
                        Level = (int)(long)reader["Level"],
                        PlayTime = reader["PlayTime"] != System.DBNull.Value ? (int)(long)reader["PlayTime"] : 0,
                        StartedAt = System.DateTime.Parse(reader["StartedAt"].ToString()),
                        EndedAt = System.DateTime.Parse(reader["EndedAt"].ToString()),
                        Rank = (int)(long)reader["Rank"]
                    });
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
                SELECT PlayerID, PlayerName, Score, Level, PlayTime, StartedAt, EndedAt, Rank
                FROM RankingView
            ";

            using (var reader = DatabaseManager.ExecuteReader(query))
            {
                while (reader.Read())
                {
                    rankings.Add(new RankingData
                    {
                        PlayerID = (int)(long)reader["PlayerID"],
                        PlayerName = reader["PlayerName"].ToString(),
                        Score = (int)(long)reader["Score"],
                        Level = (int)(long)reader["Level"],
                        PlayTime = reader["PlayTime"] != System.DBNull.Value ? (int)(long)reader["PlayTime"] : 0,
                        StartedAt = System.DateTime.Parse(reader["StartedAt"].ToString()),
                        EndedAt = System.DateTime.Parse(reader["EndedAt"].ToString()),
                        Rank = (int)(long)reader["Rank"]
                    });
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
    /// 특정 점수보다 높은 게임 수 조회 (순위 계산용)
    /// </summary>
    public static int GetRankByScore(int score)
    {
        try
        {
            string query = @"
                SELECT COUNT(*) + 1 as Rank
                FROM GameSessions gs
                WHERE gs.Score > @score AND gs.EndedAt IS NOT NULL
            ";

            var result = DatabaseManager.ExecuteScalar(query, ("@score", score));

            return result != null ? (int)(long)result : -1;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"점수별 순위 조회 오류: {ex.Message}");
            return -1;
        }
    }

    /// <summary>
    /// 특정 플레이어의 최고 순위 조회
    /// </summary>
    public static int GetPlayerBestRank(int playerId)
    {
        try
        {
            string query = @"
                SELECT MIN(Rank) as BestRank
                FROM (
                    SELECT ROW_NUMBER() OVER (ORDER BY gs.Score DESC, gs.EndedAt ASC) as Rank
                    FROM GameSessions gs
                    WHERE gs.EndedAt IS NOT NULL AND gs.PlayerID = @playerId
                )
            ";

            var result = DatabaseManager.ExecuteScalar(query, ("@playerId", playerId));

            return result != System.DBNull.Value && result != null ? (int)(long)result : -1;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"플레이어 최고 순위 조회 오류: {ex.Message}");
            return -1;
        }
    }

    /// <summary>
    /// 특정 플레이어의 모든 게임 순위 조회
    /// </summary>
    public static List<RankingData> GetPlayerRankings(int playerId)
    {
        var rankings = new List<RankingData>();

        try
        {
            string query = @"
                SELECT PlayerID, PlayerName, Score, Level, PlayTime, StartedAt, EndedAt, Rank
                FROM RankingView
                WHERE PlayerID = @playerId
                ORDER BY Score DESC
            ";

            using (var reader = DatabaseManager.ExecuteReader(query, ("@playerId", playerId)))
            {
                while (reader.Read())
                {
                    rankings.Add(new RankingData
                    {
                        PlayerID = (int)(long)reader["PlayerID"],
                        PlayerName = reader["PlayerName"].ToString(),
                        Score = (int)(long)reader["Score"],
                        Level = (int)(long)reader["Level"],
                        PlayTime = reader["PlayTime"] != System.DBNull.Value ? (int)(long)reader["PlayTime"] : 0,
                        StartedAt = System.DateTime.Parse(reader["StartedAt"].ToString()),
                        EndedAt = System.DateTime.Parse(reader["EndedAt"].ToString()),
                        Rank = (int)(long)reader["Rank"]
                    });
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"플레이어 랭킹 조회 오류: {ex.Message}");
        }

        return rankings;
    }

    /// <summary>
    /// 특정 점수 범위의 랭킹 조회
    /// </summary>
    public static List<RankingData> GetRankingsByScoreRange(int minScore, int maxScore)
    {
        var rankings = new List<RankingData>();

        try
        {
            string query = @"
                SELECT PlayerID, PlayerName, Score, Level, PlayTime, StartedAt, EndedAt, Rank
                FROM RankingView
                WHERE Score BETWEEN @minScore AND @maxScore
                ORDER BY Score DESC
            ";

            using (var reader = DatabaseManager.ExecuteReader(query,
                ("@minScore", minScore),
                ("@maxScore", maxScore)))
            {
                while (reader.Read())
                {
                    rankings.Add(new RankingData
                    {
                        PlayerID = (int)(long)reader["PlayerID"],
                        PlayerName = reader["PlayerName"].ToString(),
                        Score = (int)(long)reader["Score"],
                        Level = (int)(long)reader["Level"],
                        PlayTime = reader["PlayTime"] != System.DBNull.Value ? (int)(long)reader["PlayTime"] : 0,
                        StartedAt = System.DateTime.Parse(reader["StartedAt"].ToString()),
                        EndedAt = System.DateTime.Parse(reader["EndedAt"].ToString()),
                        Rank = (int)(long)reader["Rank"]
                    });
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"점수 범위별 랭킹 조회 오류: {ex.Message}");
        }

        return rankings;
    }

    /// <summary>
    /// 전체 게임 수 조회
    /// </summary>
    public static int GetTotalGameCount()
    {
        try
        {
            string query = "SELECT COUNT(*) FROM GameSessions WHERE EndedAt IS NOT NULL";

            var result = DatabaseManager.ExecuteScalar(query);

            return result != null ? (int)(long)result : 0;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"전체 게임 수 조회 오류: {ex.Message}");
            return 0;
        }
    }
}
