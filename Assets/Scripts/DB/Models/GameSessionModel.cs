using System;
using UnityEngine;

/// <summary>
/// 게임 세션 정보를 담는 데이터 모델 클래스
/// </summary>
[System.Serializable]
public class GameSessionModel
{
    public int SessionID;
    public int PlayerID;
    public int Score;
    public int Level;
    public int EnemiesKilled;
    public int DeathCount;
    public DateTime StartedAt;
    public DateTime? EndedAt; // Nullable - 게임이 진행중이면 null
    public int? PlayTime; // 초 단위, Nullable

    public GameSessionModel()
    {
        SessionID = 0;
        PlayerID = 0;
        Score = 0;
        Level = 1;
        EnemiesKilled = 0;
        DeathCount = 0;
        StartedAt = DateTime.Now;
        EndedAt = null;
        PlayTime = null;
    }

    public GameSessionModel(int playerID)
    {
        SessionID = 0;
        PlayerID = playerID;
        Score = 0;
        Level = 1;
        EnemiesKilled = 0;
        DeathCount = 0;
        StartedAt = DateTime.Now;
        EndedAt = null;
        PlayTime = null;
    }

    /// <summary>
    /// 게임이 진행중인지 확인
    /// </summary>
    public bool IsInProgress => EndedAt == null;

    /// <summary>
    /// 게임 종료
    /// </summary>
    public void EndGame()
    {
        EndedAt = DateTime.Now;
        PlayTime = (int)(EndedAt.Value - StartedAt).TotalSeconds;
    }

    /// <summary>
    /// 플레이 시간을 시간:분:초 형태로 반환
    /// </summary>
    public string GetFormattedPlayTime()
    {
        if (PlayTime == null) return "진행중";

        int hours = PlayTime.Value / 3600;
        int minutes = (PlayTime.Value % 3600) / 60;
        int seconds = PlayTime.Value % 60;
        return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    }
}
