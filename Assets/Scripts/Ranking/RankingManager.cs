using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RankingManager : MonoBehaviour
{
    // 플레이어 이름과 점수를 저장하는 리스트, Unique 제한 정렬 필요하면 rankingList.OrderByDescending(t => t.Value).ToList();
    private Dictionary<string, int> rankingList;

    private RankingSQL sql;

    public Dictionary<string, int> RankingList => rankingList;

    private void Awake()
    {
        sql = GetComponent<RankingSQL>();
        if (sql == null)
        {
            Debug.LogWarning("[RankingManager] RankingSQL 컴포넌트를 찾을 수 없습니다. 같은 GameObject에 RankingSQL 컴포넌트를 추가해주세요.");
        }

        rankingList = new Dictionary<string, int>();

        // 시작 시 데이터베이스에서 기존 랭킹 데이터 로드
        LoadRankingsFromDatabase();
    }

    public void Setup(RankingSQL sql)
    {
        this.sql = sql;
    }

    /// <summary>
    /// 데이터베이스에서 랭킹 데이터 로드
    /// </summary>
    private void LoadRankingsFromDatabase()
    {
        if (sql != null)
        {
            var dbRankings = sql.GetRankingsAsDictionary();
            rankingList = dbRankings;
            Debug.Log($"<color=cyan>[RankingManager] 데이터베이스에서 {rankingList.Count}개의 랭킹 데이터를 로드했습니다</color>");
        }
    }

    /// <summary>
    /// 현재 랭킹을 콘솔에 출력 (디버그용)
    /// </summary>
    public void DisplayCurrentRankings()
    {
        if (rankingList.Count == 0)
        {
            Debug.Log("[RankingManager] 현재 랭킹 데이터가 없습니다.");
            return;
        }

        var sortedRankings = rankingList.OrderByDescending(t => t.Value).ToList();
        Debug.Log("<color=yellow>=== 현재 랭킹 ===</color>");

        for (int i = 0; i < sortedRankings.Count; i++)
        {
            Debug.Log($"<color=yellow>{i + 1}위: {sortedRankings[i].Key} - {sortedRankings[i].Value}점</color>");
        }
    }

    /// <summary>
    /// Dictionary의 모든 데이터를 데이터베이스에 동기화
    /// </summary>
    public void UpdateSQL()
    {
        if (sql != null)
        {
            sql.SyncDictionaryToDatabase(rankingList);
        }
        else
        {
            Debug.LogWarning("[RankingManager] RankingSQL 컴포넌트가 없습니다. SQL 업데이트를 건너뜁니다.");
        }
    }

    /// <summary>
    /// rankingList에서 entity의 점수 업데이트
    /// </summary>
    public void UpdateEntity(Entity entity) // Score 변경되는 경우
    {
        string name = entity.Info.EntityName;

        if (rankingList.ContainsKey(name))
        {
            int oldScore = rankingList[name];
            rankingList[name] = entity.Data.Score;
            Debug.Log($"<color=blue>[RankingManager] 점수 업데이트| {entity.Info.EntityName}의 Score: {oldScore} → {entity.Data.Score}</color>");

            UpdateSQL();
        }
        else
        {
            Debug.LogWarning($"<color=yellow>[RankingManager] {name}이 랭킹 리스트에 없습니다. AddEntity를 먼저 호출해주세요.</color>");
        }
    }

    /// <summary>
    /// rankingList에 entity 추가
    /// </summary>
    public void AddEntity(Entity entity)
    {
        if (rankingList.TryAdd(entity.Info.EntityName, entity.Data.Score))
        {
            Debug.Log($"<color=green>[RankingManager] {entity.Info?.EntityName} 랭킹에 추가</color>");

            UpdateSQL();
        }
        else
        {
            Debug.LogWarning($"<color=yellow>[RankingManager] {entity.Info?.EntityName}은 이미 랭킹에 존재합니다. UpdateEntity를 사용해주세요.</color>");
        }
    }

    /// <summary>
    /// rankingList에서 entity 제거
    /// </summary>
    public void RemoveEntity(Entity entity)
    {
        if (rankingList.Remove(entity.Info.EntityName))
        {
            Debug.Log($"<color=red>[RankingManager] {entity.Info?.EntityName} 랭킹에서 삭제</color>");

            // 데이터베이스에서도 삭제
            if (sql != null)
            {
                sql.RemovePlayer(entity.Info.EntityName);
            }
        }
        else
        {
            Debug.LogWarning($"<color=yellow>[RankingManager] {entity.Info?.EntityName}을 랭킹에서 찾을 수 없습니다.</color>");
        }
    }

    /// <summary>
    /// 특정 플레이어의 순위 조회
    /// </summary>
    public int GetPlayerRank(string playerName)
    {
        if (sql != null)
        {
            return sql.GetPlayerRank(playerName);
        }

        // SQL이 없는 경우 로컬 Dictionary에서 계산
        if (!rankingList.ContainsKey(playerName))
        {
            return -1;
        }

        int playerScore = rankingList[playerName];
        int rank = rankingList.Values.Count(score => score > playerScore) + 1;

        return rank;
    }

    /// <summary>
    /// 상위 N명의 랭킹 조회
    /// </summary>
    public List<RankingData> GetTopRankings(int limit = 10)
    {
        if (sql != null)
        {
            return sql.GetTopRankings(limit);
        }

        // SQL이 없는 경우 로컬 Dictionary에서 변환
        var sortedRankings = rankingList.OrderByDescending(t => t.Value).Take(limit).Select(kvp => new RankingData(kvp.Key, kvp.Value)).ToList();
        return sortedRankings;
    }

    /// <summary>
    /// 데이터베이스와 로컬 데이터 강제 동기화
    /// </summary>
    public void ForceSyncWithDatabase()
    {
        if (sql != null)
        {
            Debug.Log("<color=cyan>[RankingManager] 데이터베이스와 강제 동기화를 시작합니다...</color>");

            // 현재 로컬 데이터를 모두 DB에 저장
            sql.SyncDictionaryToDatabase(rankingList);

            // DB에서 다시 로드
            LoadRankingsFromDatabase();

            Debug.Log("<color=cyan>[RankingManager] 데이터베이스 동기화가 완료되었습니다.</color>");
        }
    }
}
