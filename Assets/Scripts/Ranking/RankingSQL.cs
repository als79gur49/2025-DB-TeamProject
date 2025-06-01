using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 랭킹 시스템의 SQL 작업을 담당하는 클래스
/// RankingManager와 연동하여 데이터베이스 작업 수행
/// </summary>
public class RankingSQL : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugLogs = true;

    private void Awake()
    {
        // 데이터베이스 초기화 확인
        DatabaseManager.Initialize();
    }

    /// <summary>
    /// 플레이어 점수 업데이트 (추가 또는 갱신)
    /// </summary>
    public bool UpdatePlayerScore(string playerName, int score)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            LogError("플레이어 이름이 비어있거나 null입니다");
            return false;
        }

        bool success = RankingRepository.UpsertPlayerScore(playerName, score);

        if (success && enableDebugLogs)
        {
            Log($"{playerName}의 점수가 업데이트되었습니다: {score}");
        }
        else if (!success)
        {
            LogError($"{playerName}의 점수 업데이트에 실패했습니다");
        }

        return success;
    }

    /// <summary>
    /// 플레이어 점수 조회
    /// </summary>
    public int GetPlayerScore(string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            LogError("플레이어 이름이 비어있거나 null입니다");
            return 0;
        }

        return RankingRepository.GetPlayerScore(playerName);
    }

    /// <summary>
    /// 상위 랭킹 조회
    /// </summary>
    public List<RankingData> GetTopRankings(int limit = 10)
    {
        var rankings = RankingRepository.GetTopRankings(limit);

        if (enableDebugLogs)
        {
            Log($"상위 {limit}명 랭킹 조회 완료: {rankings.Count}개 결과");
        }

        return rankings;
    }

    /// <summary>
    /// 모든 랭킹 조회
    /// </summary>
    public List<RankingData> GetAllRankings()
    {
        var rankings = RankingRepository.GetAllRankings();

        if (enableDebugLogs)
        {
            Log($"전체 랭킹 조회 완료: {rankings.Count}개 결과");
        }

        return rankings;
    }

    /// <summary>
    /// 플레이어 순위 조회
    /// </summary>
    public int GetPlayerRank(string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            LogError("플레이어 이름이 비어있거나 null입니다");
            return -1;
        }

        return RankingRepository.GetPlayerRank(playerName);
    }

    /// <summary>
    /// 플레이어 삭제
    /// </summary>
    public bool RemovePlayer(string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            LogError("플레이어 이름이 비어있거나 null입니다");
            return false;
        }

        bool success = RankingRepository.DeletePlayer(playerName);

        if (success && enableDebugLogs)
        {
            Log($"플레이어 삭제 완료: {playerName}");
        }
        else if (!success)
        {
            LogError($"플레이어 삭제 실패: {playerName}");
        }

        return success;
    }

    /// <summary>
    /// 랭킹 리스트를 Dictionary 형태로 반환 (기존 RankingManager 호환성)
    /// </summary>
    public Dictionary<string, int> GetRankingsAsDictionary()
    {
        var rankings = GetAllRankings();
        var dictionary = new Dictionary<string, int>();

        foreach (var ranking in rankings)
        {
            dictionary[ranking.Name] = ranking.Score;
        }

        return dictionary;
    }

    /// <summary>
    /// Dictionary를 데이터베이스에 동기화 (기존 RankingManager 호환성)
    /// </summary>
    public void SyncDictionaryToDatabase(Dictionary<string, int> rankingDict)
    {
        if (rankingDict == null)
        {
            LogError("랭킹 딕셔너리가 null입니다");
            return;
        }

        foreach (var kvp in rankingDict)
        {
            UpdatePlayerScore(kvp.Key, kvp.Value);
        }

        if (enableDebugLogs)
        {
            Log($"{rankingDict.Count}개 데이터를 데이터베이스에 동기화했습니다");
        }
    }

    /// <summary>
    /// 전체 랭킹 초기화
    /// </summary>
    public bool ClearAllRankings()
    {
        bool success = RankingRepository.ClearAllRankings();

        if (success && enableDebugLogs)
        {
            Log("모든 랭킹이 삭제되었습니다");
        }
        else if (!success)
        {
            LogError("랭킹 삭제에 실패했습니다");
        }

        return success;
    }

    #region Debug Methods
    private void Log(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[RankingSQL] {message}");
        }
    }

    private void LogError(string message)
    {
        Debug.LogError($"[RankingSQL] {message}");
    }
    #endregion
}
