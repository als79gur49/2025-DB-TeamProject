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

            clone.Setup(new EntityInfo("Player", "Test_Image"), new EntityData(100, 10, 1), rankingManager, damagePopupManager, killLogManager, scoreBlockSpawner, skillIconManager);

            if(virtualCamera != null)
            {
                virtualCamera.Follow = clone.transform;
            }
        }
    }
}
