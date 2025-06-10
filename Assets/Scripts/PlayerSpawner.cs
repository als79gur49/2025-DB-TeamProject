using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerSpawner : EntitySpawner
{
    [SerializeField]
    private Player player;
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;
    [SerializeField]
    private SkillIconManager skillIconManager;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            //Entity clone = GameObject.Instantiate(enemy[Random.Range(0, enemy.Length)]);
            Player clone = Instantiate(player, Vector3.zero, Quaternion.identity);

            string name = "Test_Enemy_" + Random.Range(0, 10000);

            clone.Setup(new EntityInfo(name, "Test_Image"), new EntityData(100, 10, 1), damagePopupManager, killLogManager, scoreBlockSpawner, skillIconManager);

            if (virtualCamera != null)
            {
                virtualCamera.Follow = clone.transform;
            }

            clone.onDeath.AddListener(SampleOnDeath);

            // 사용자가 이름을 입력하고 게임 시작 버튼 클릭 시
            var playerName = clone.Info.EntityName;
            var currentSession = EntityGameManager.StartNewGame(playerName);

            if (currentSession != null)
            {
                Debug.Log($"게임 시작! 세션 ID: {currentSession.SessionID}");

                // 플레이어 GameObject 매핑 등록
                int playerInstanceId = GetInstanceID(); // 실제 GameObject
                EntityGameManager.RegisterPlayerEntity(playerInstanceId);
            }
        }
    }

    /// <summary>
    /// 테스트 (죽었을 때 게임 종료) - 아직 확실한 process 미구현
    /// </summary>
    private void SampleOnDeath()
    {
        Debug.Log("SampleOnDeath : SampleOnDeath : SampleOnDeath : SampleOnDeath");
        GameSessionManager.OnPlayerDeath();
    }
}
