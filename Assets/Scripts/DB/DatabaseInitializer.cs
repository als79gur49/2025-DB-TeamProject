using UnityEngine;

/// <summary>
/// 게임 시작 시 데이터베이스를 초기화하는 MonoBehaviour
/// </summary>
public class DatabaseInitializer : MonoBehaviour
{
    [Header("Database Settings")]
    [SerializeField] private bool initializeOnAwake = true;
    [SerializeField] private bool closeOnApplicationQuit = true;

    // Singleton 패턴으로 중복 실행 방지
    private static bool isInitialized = false;
    private static DatabaseInitializer instance;

    private void Awake()
    {
        // 이미 인스턴스가 있으면 확인
        if (instance != null && instance != this)
        {
            Debug.LogWarning($"DatabaseInitializer가 이미 존재합니다.");
            return;
        }

        instance = this;

        Debug.Log($"DatabaseInitializer Awake() 호출됨 - GameObject: {gameObject.name}");

        if (initializeOnAwake && !isInitialized)
        {
            Debug.Log("데이터베이스 초기화 시작...");
            InitializeDatabase();
            isInitialized = true;
        }
        else if (isInitialized)
        {
            Debug.Log("데이터베이스가 이미 초기화되어 있습니다.");
        }
        else
        {
            Debug.Log("initializeOnAwake가 false로 설정되어 있습니다");
        }
    }

    private void Start()
    {
        // 인스턴스가 본인인 경우에만 실행
        if (instance == this)
        {
            Debug.Log($"DatabaseInitializer Start() 호출됨 - GameObject: {gameObject.name}");

            // 게임 시작 시 데이터베이스 상태 확인
            TestDatabaseConnection();
        }
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
            // 간단한 테스트 쿼리 실행 (새로운 DB 구조에 맞게)
            var result = DatabaseManager.ExecuteScalar("SELECT COUNT(*) FROM Players");
            int playerCount = result != null ? (int)(long)result : 0;

            Debug.Log($"데이터베이스 연결 테스트 성공. 기록된 플레이어 수: {playerCount}");

            // 데이터베이스 버전도 확인
            int dbVersion = DatabaseManager.GetCurrentDatabaseVersion();
            Debug.Log($"데이터베이스 버전: {dbVersion}");
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
