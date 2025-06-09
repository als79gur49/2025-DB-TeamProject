using UnityEngine;

/// <summary>
/// 게임 시작 시 데이터베이스를 초기화하는 MonoBehaviour
/// </summary>
public class DatabaseInitializer : MonoBehaviour
{
    [Header("Database Settings")]
    [SerializeField] private bool initializeOnAwake = true;
    [SerializeField] private bool closeOnApplicationQuit = true;

    private void Awake()
    {
        if (initializeOnAwake)
        {
            InitializeDatabase();
        }
    }

    private void Start()
    {
        // 게임 시작 시 데이터베이스 상태 확인
        TestDatabaseConnection();
    }

    /// <summary>
    /// 데이터베이스 초기화
    /// </summary>
    public void InitializeDatabase()
    {
        try
        {
            DatabaseManager.Initialize();
            Debug.Log("데이터베이스 초기화가 성공적으로 완료되었습니다");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"데이터베이스 초기화 실패: {ex.Message}");
        }
    }

    /// <summary>
    /// 데이터베이스 연결 테스트
    /// </summary>
    private void TestDatabaseConnection()
    {
        try
        {
            // 간단한 테스트 쿼리 실행
            var result = DatabaseManager.ExecuteScalar("SELECT COUNT(*) FROM Rankings");
            int rankingCount = result != null ? (int)(long)result : 0;

            Debug.Log($"데이터베이스 연결 테스트 성공. 현재 랭킹 수: {rankingCount}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"데이터베이스 연결 테스트 실패: {ex.Message}");
        }
    }

    private void OnApplicationQuit()
    {
        if (closeOnApplicationQuit)
        {
            DatabaseManager.Close();
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        // 모바일에서 앱이 백그라운드로 갈 때 데이터베이스 연결 관리
        if (pauseStatus && closeOnApplicationQuit)
        {
            DatabaseManager.Close();
        }
        else if (!pauseStatus)
        {
            InitializeDatabase();
        }
    }
}
