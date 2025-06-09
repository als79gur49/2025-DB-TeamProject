using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq.Expressions;

public class PlayerSpawner : EntitySpawner
{
    [SerializeField]
    private Player player;
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;
    [SerializeField]
    private SkillIconManager skillIconManager;

    [SerializeField]
    private PlayerModel currentPlayer;
    [SerializeField]
    private GameSessionModel currentSession;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            //Entity clone = GameObject.Instantiate(enemy[Random.Range(0, enemy.Length)]);
            Player clone = Instantiate(player, Vector3.zero, Quaternion.identity);

            string name = "Test_Enemy_" + Random.Range(0, 10000);

            clone.Setup(new EntityInfo("Player", "Test_Image"), new EntityData(100, 10, 1), rankingManager, damagePopupManager, killLogManager, scoreBlockSpawner, skillIconManager);

            currentPlayer = PlayerRepository.CreatePlayer(clone.Info.EntityName);
            currentSession = GameSessionRepository.StartNewSession(currentPlayer.PlayerID);

            clone.onDeath.AddListener(SampleOnDeath);

            // ����, ����, ų �� ����
            currentSession.Score = 1500;
            currentSession.Level = 5;
            currentSession.EnemiesKilled = 120;
            GameSessionRepository.UpdateSession(currentSession);

            if (virtualCamera != null)
            {
                virtualCamera.Follow = clone.transform;
            }
        }
    }

    /// <summary>
    /// �׽�Ʈ (�׾��� �� ���� ����) - ���� Ȯ���� process �̱���
    /// </summary>
    private void SampleOnDeath()
    {
        Debug.Log("SampleOnDeath : SampleOnDeath : SampleOnDeath : SampleOnDeath");
        GameSessionRepository.EndSession(currentSession.SessionID, 0, 0, currentSession.DeathCount, 0);
    }
}
