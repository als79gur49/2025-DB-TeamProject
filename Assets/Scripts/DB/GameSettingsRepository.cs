using System;
using System.Data;
using UnityEngine;

/// <summary>
/// 게임 설정 관련 데이터베이스 작업을 담당하는 클래스
/// </summary>
public static class GameSettingsRepository
{
    /// <summary>
    /// 게임 설정 조회
    /// </summary>
    public static GameSettingsModel GetGameSettings()
    {
        try
        {
            string query = @"
                SELECT SettingID, MasterVolume, SFXVolume, BGMVolume, 
                       GraphicsQuality, FullScreen, UpdatedAt
                FROM GameSettings
                WHERE SettingID = 1
            ";

            using (var reader = DatabaseManager.ExecuteReader(query))
            {
                if (reader.Read())
                {
                    return new GameSettingsModel
                    {
                        SettingID = (int)(long)reader["SettingID"],
                        MasterVolume = (float)(double)reader["MasterVolume"],
                        SFXVolume = (float)(double)reader["SFXVolume"],
                        BGMVolume = (float)(double)reader["BGMVolume"],
                        GraphicsQuality = (int)(long)reader["GraphicsQuality"],
                        FullScreen = (long)reader["FullScreen"] == 1,
                        UpdatedAt = DateTime.Parse(reader["UpdatedAt"].ToString())
                    };
                }
            }

            // 설정이 없으면 기본 설정 반환
            return new GameSettingsModel();
        }
        catch (Exception ex)
        {
            Debug.LogError($"게임 설정 조회 오류: {ex.Message}");
            return new GameSettingsModel();
        }
    }

    /// <summary>
    /// 게임 설정 업데이트
    /// </summary>
    public static bool UpdateGameSettings(GameSettingsModel settings)
    {
        try
        {
            string query = @"
                UPDATE GameSettings SET
                    MasterVolume = @masterVolume,
                    SFXVolume = @sfxVolume,
                    BGMVolume = @bgmVolume,
                    GraphicsQuality = @graphicsQuality,
                    FullScreen = @fullScreen,
                    UpdatedAt = CURRENT_TIMESTAMP
                WHERE SettingID = 1
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query,
                ("@masterVolume", settings.MasterVolume),
                ("@sfxVolume", settings.SFXVolume),
                ("@bgmVolume", settings.BGMVolume),
                ("@graphicsQuality", settings.GraphicsQuality),
                ("@fullScreen", settings.FullScreen ? 1 : 0));

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"게임 설정 업데이트 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 마스터 볼륨만 업데이트
    /// </summary>
    public static bool UpdateMasterVolume(float volume)
    {
        try
        {
            string query = @"
                UPDATE GameSettings SET
                    MasterVolume = @volume,
                    UpdatedAt = CURRENT_TIMESTAMP
                WHERE SettingID = 1
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query, ("@volume", volume));

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"마스터 볼륨 업데이트 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// SFX 볼륨만 업데이트
    /// </summary>
    public static bool UpdateSFXVolume(float volume)
    {
        try
        {
            string query = @"
                UPDATE GameSettings SET
                    SFXVolume = @volume,
                    UpdatedAt = CURRENT_TIMESTAMP
                WHERE SettingID = 1
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query, ("@volume", volume));

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"SFX 볼륨 업데이트 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// BGM 볼륨만 업데이트
    /// </summary>
    public static bool UpdateBGMVolume(float volume)
    {
        try
        {
            string query = @"
                UPDATE GameSettings SET
                    BGMVolume = @volume,
                    UpdatedAt = CURRENT_TIMESTAMP
                WHERE SettingID = 1
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query, ("@volume", volume));

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"BGM 볼륨 업데이트 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 그래픽 품질만 업데이트
    /// </summary>
    public static bool UpdateGraphicsQuality(int quality)
    {
        try
        {
            string query = @"
                UPDATE GameSettings SET
                    GraphicsQuality = @quality,
                    UpdatedAt = CURRENT_TIMESTAMP
                WHERE SettingID = 1
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query, ("@quality", quality));

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"그래픽 품질 업데이트 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 전체화면 설정만 업데이트
    /// </summary>
    public static bool UpdateFullScreen(bool fullScreen)
    {
        try
        {
            string query = @"
                UPDATE GameSettings SET
                    FullScreen = @fullScreen,
                    UpdatedAt = CURRENT_TIMESTAMP
                WHERE SettingID = 1
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query, ("@fullScreen", fullScreen ? 1 : 0));

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"전체화면 설정 업데이트 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 설정을 기본값으로 초기화
    /// </summary>
    public static bool ResetToDefaults()
    {
        try
        {
            var defaultSettings = new GameSettingsModel();
            return UpdateGameSettings(defaultSettings);
        }
        catch (Exception ex)
        {
            Debug.LogError($"설정 초기화 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 설정이 존재하는지 확인
    /// </summary>
    public static bool SettingsExist()
    {
        try
        {
            string query = "SELECT COUNT(*) FROM GameSettings WHERE SettingID = 1";

            var result = DatabaseManager.ExecuteScalar(query);

            return result != null && (long)result > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"설정 존재 확인 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 설정이 없으면 기본 설정 생성
    /// </summary>
    public static bool EnsureSettingsExist()
    {
        try
        {
            if (!SettingsExist())
            {
                string query = @"
                    INSERT INTO GameSettings (SettingID, MasterVolume, SFXVolume, BGMVolume, 
                                            GraphicsQuality, FullScreen, UpdatedAt)
                    VALUES (1, 1.0, 1.0, 1.0, 2, 1, CURRENT_TIMESTAMP)
                ";

                int rowsAffected = DatabaseManager.ExecuteNonQuery(query);
                return rowsAffected > 0;
            }
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"설정 생성 확인 오류: {ex.Message}");
            return false;
        }
    }
}
