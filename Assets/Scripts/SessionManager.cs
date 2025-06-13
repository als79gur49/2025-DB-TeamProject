using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SessionManager : MonoBehaviour
{
    // Spawner�� UnityEvent�� ����

    [Header("�ٽ� �Ŵ�����")]
    [SerializeField] 
    private DamagePopupManager damagePopupManager; // ���ݹ��� ��� ������ ���
    [SerializeField] 
    private KillLogManager killLogManager; // ���� ��� ų�α� ���
    [SerializeField] 
    private ScoreBlockSpawner scoreBlockSpawner; // ���� ��� ������ ����
    [SerializeField] 
    private UIController uiController; // �÷��̾� UI, ��ŷ���� UI
    [SerializeField] 
    private CinemachineVirtualCamera virtualCamera; // �÷��̾ ����� ī�޶�
    [SerializeField] 
    private SkillIconManager skillIconManager; // ��ų ����� // �۵�X

    [Header("Spawners")]
    [SerializeField] 
    private PlayerSpawner playerSpawner;
    [SerializeField] 
    private EnemySpawner enemySpawner;

    [Header("Game Session")]
    private GameSessionModel currentSession;
    private bool isGameActive = false;

    public GameSessionModel CurrentSession => currentSession;
    public bool IsGameActive => isGameActive;

    private string playerName;
    private void Awake()
    {
        ValidateComponents(); // �ٽ� �Ŵ��� �Ҵ�Ǿ����� ����
        InitializeSpawners();
    }

    private void Start()
    {
        // ���� ����
        StartGame();
    }


    private void ValidateComponents()
    {
        if (damagePopupManager == null || killLogManager == null || scoreBlockSpawner == null)
        {
            Debug.LogError("Core Manager ������ �����ϴ�.");
            return;
        }

        if (playerSpawner == null || enemySpawner == null)
        {
            Debug.LogError("Player, EnemySpawner ������ �����ϴ�.");
            return;
        }

        if (uiController == null || virtualCamera == null)
        {
            Debug.LogError("UI �Ǵ� Camera ������ �����ϴ�.");
            return;
        }
    }

    private void InitializeSpawners()
    {
        // ������ ����
        playerSpawner.Setup(damagePopupManager, killLogManager, scoreBlockSpawner);
        enemySpawner.Setup(damagePopupManager, killLogManager, scoreBlockSpawner);

        // �÷��̾� ������ ������ ����
        playerSpawner.SetupPlayer(uiController, virtualCamera, skillIconManager, GetPlayerName());

        // �̺�Ʈ ����
        playerSpawner.onPlayerSpawned.AddListener(OnPlayerSpawned);
        playerSpawner.onPlayerDeath.AddListener(OnPlayerDeath);

        enemySpawner.onEnemySpawned.AddListener(OnEnemySpawned);
        enemySpawner.onEnemyDeath.AddListener(OnEnemyDeath);
    }


    private string GetPlayerName()
    {
        if (string.IsNullOrEmpty(playerName))
        {
            // Ÿ��Ʋ������ �Էµ� �̸�
            playerName = PlayerDataManager.Instance?.PlayerName;
            if (string.IsNullOrEmpty(playerName))
            {
                playerName = $"Test_Player_{Random.Range(0, 1000)}";
            }
        }
        return playerName;
    }

    public void StartGame()
    {
        if (isGameActive)
        {
            Debug.LogWarning("�̹� ������ ���� ���Դϴ�.");
            return;
        }
        RankingManager.ClearAllSessionRanking();
        string playerName = GetPlayerName();
        currentSession = EntityGameManager.StartNewGame(playerName);

        if (currentSession != null)
        {
            isGameActive = true;
            Debug.Log($"���� ����. ���Ӽ��� ID: {currentSession.SessionID}");

            // �÷��̾� ����
            playerSpawner.SpawnPlayer();
        }
        else
        {
            Debug.LogError("���� ���� ������ �����߽��ϴ�.");
        }
    }

    public void EndGame()
    {
        if (!isGameActive)
        {
            Debug.LogWarning("������ ������ �����ϴ�.");
            return;
        }

        isGameActive = false;
        GameSessionManager.OnPlayerDeath();

        Debug.Log("���� ����");
    }

    private void OnPlayerSpawned(Player player)
    {
        if (currentSession != null)
        {
            // UI ����
            uiController.Setup(player, player.Info.EntityName, currentSession.SessionID);

            // ī�޶� ����
            if (virtualCamera != null)
            {
                virtualCamera.Follow = player.transform;
            }

            // �÷��̾� ��ƼƼ ���
            int playerInstanceId = player.GetInstanceID();
            EntityGameManager.RegisterPlayerEntity(playerInstanceId);
        }
    }

    private void OnPlayerDeath(Player player)
    {
        EndGame();
    }


    private void OnEnemySpawned(Enemy enemy)
    {
        // �� ���� �� �ʿ��� ����
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        // �� ��� �� �ʿ��� ����
    }

    private void OnDestroy()
    {
        // �̺�Ʈ ���� ����
        if (playerSpawner != null)
        {
            playerSpawner.onPlayerSpawned.RemoveAllListeners();
            playerSpawner.onPlayerDeath.RemoveAllListeners();
        }

        if (enemySpawner != null)
        {
            enemySpawner.onEnemySpawned.RemoveAllListeners();
            enemySpawner.onEnemyDeath.RemoveAllListeners();
        }
    }
}