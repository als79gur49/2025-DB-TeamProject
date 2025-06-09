using System;
using UnityEngine;

/// <summary>
/// 랭킹 정보를 담는 데이터 모델 클래스
/// </summary>
[System.Serializable]
public class RankingData
{
    public int PlayerID;
    public string PlayerName;
    public int Score;
    public int Level;
    public int PlayTime; // 초 단위
    public DateTime StartedAt;
    public DateTime EndedAt;
    public int Rank;

    public RankingData()
    {
        PlayerID = 0;
        PlayerName = "";
        Score = 0;
        Level = 1;
        PlayTime = 0;
        StartedAt = DateTime.Now;
        EndedAt = DateTime.Now;
        Rank = 0;
    }

    public RankingData(int playerID, string playerName, int score, int rank = 0)
    {
        PlayerID = playerID;
        PlayerName = playerName;
        Score = score;
        Level = 1;
        PlayTime = 0;
        StartedAt = DateTime.Now;
        EndedAt = DateTime.Now;
        Rank = rank;
    }

    /// <summary>
    /// 플레이 시간을 시간:분:초 형태로 반환
    /// </summary>
    public string GetFormattedPlayTime()
    {
        int hours = PlayTime / 3600;
        int minutes = (PlayTime % 3600) / 60;
        int seconds = PlayTime % 60;
        return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    }

    /// <summary>
    /// 점수를 천단위 구분자로 포맷
    /// </summary>
    public string GetFormattedScore()
    {
        return Score.ToString("N0");
    }
}
