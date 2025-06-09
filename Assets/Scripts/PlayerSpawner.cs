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
    private PlayerModel currentPlayer;
    [SerializeField]
    private GameSessionModel currentSession;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            //Entity clone = GameObject.Instantiate(enemy[Random.Range(0, enemy.Length)]);
            Player clone = Instantiate(player, Vector3.zero, Quaternion.identity);

            string name = "Test_Player_" + Random.Range(0, 10000);

            clone.Setup(new EntityInfo(name, "Test_Image"), new EntityData(100, 10, 1), damagePopupManager, killLogManager, scoreBlockSpawner);

            currentPlayer = PlayerRepository.CreatePlayer(clone.Info.EntityName);
            currentSession = GameSessionRepository.StartNewSession(currentPlayer.PlayerID);

            clone.onDeath.AddListener(SampleOnDeath);

            // 점수, 레벨, 킬 수 변경
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
    /// 테스트 (죽었을 때 게임 종료) - 아직 확실한 process 미구현
    /// </summary>
    private void SampleOnDeath()
    {
        Debug.Log("SampleOnDeath : SampleOnDeath : SampleOnDeath : SampleOnDeath");
        GameSessionRepository.EndSession(currentSession.SessionID, 0, 0, currentSession.DeathCount, 0);
    }
}
