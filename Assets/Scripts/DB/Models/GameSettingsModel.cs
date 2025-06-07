using System;
using UnityEngine;

/// <summary>
/// 게임 설정 정보를 담는 데이터 모델 클래스
/// </summary>
[System.Serializable]
public class GameSettingsModel
{
    public int SettingID;
    public float MasterVolume;
    public float SFXVolume;
    public float BGMVolume;
    public int GraphicsQuality; // 0:Low, 1:Medium, 2:High
    public bool FullScreen;
    public DateTime UpdatedAt;

    public GameSettingsModel()
    {
        SettingID = 1;
        MasterVolume = 1.0f;
        SFXVolume = 1.0f;
        BGMVolume = 1.0f;
        GraphicsQuality = 2;
        FullScreen = true;
        UpdatedAt = DateTime.Now;
    }

    /// <summary>
    /// 그래픽 품질을 문자열로 반환
    /// </summary>
    public string GetGraphicsQualityString()
    {
        return GraphicsQuality switch
        {
            0 => "Low",
            1 => "Medium",
            2 => "High",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// 볼륨을 퍼센트로 반환
    /// </summary>
    public int GetMasterVolumePercent() => Mathf.RoundToInt(MasterVolume * 100);
    public int GetSFXVolumePercent() => Mathf.RoundToInt(SFXVolume * 100);
    public int GetBGMVolumePercent() => Mathf.RoundToInt(BGMVolume * 100);
}
