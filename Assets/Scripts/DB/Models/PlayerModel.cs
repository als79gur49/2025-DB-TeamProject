using System;
using UnityEngine;

/// <summary>
/// 플레이어 정보를 담는 데이터 모델 클래스
/// </summary>
[System.Serializable]
public class PlayerModel
{
    public int PlayerID;
    public string PlayerName;
    public int CurrentLevel;
    public int CurrentExp;
    public int ExpToNextLevel;
    public int HighestScore;
    public int TotalPlayTime; // 초 단위
    public DateTime CreatedAt;
    public DateTime LastPlayedAt;

    public PlayerModel()
    {
        PlayerID = 0;
        PlayerName = "";
        CurrentLevel = 1;
        CurrentExp = 0;
        ExpToNextLevel = 100;
        HighestScore = 0;
        TotalPlayTime = 0;
        CreatedAt = DateTime.Now;
        LastPlayedAt = DateTime.Now;
    }

    public PlayerModel(string playerName)
    {
        PlayerID = 0;
        PlayerName = playerName;
        CurrentLevel = 1;
        CurrentExp = 0;
        ExpToNextLevel = 100;
        HighestScore = 0;
        TotalPlayTime = 0;
        CreatedAt = DateTime.Now;
        LastPlayedAt = DateTime.Now;
    }

    /// <summary>
    /// 총 플레이 시간을 시간:분:초 형태로 반환
    /// </summary>
    public string GetFormattedPlayTime()
    {
        int hours = TotalPlayTime / 3600;
        int minutes = (TotalPlayTime % 3600) / 60;
        int seconds = TotalPlayTime % 60;
        return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    }
}
