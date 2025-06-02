using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [SerializeField]
    public RankingManager rankingManager;
    [SerializeField]
    public DamagePopupManager damagePopupManager;
    [SerializeField]
    public KillLogManager killLogManager;

    public void Setup(RankingManager rankingManager, DamagePopupManager damagePopupManager, KillLogManager killLogManager)
    {
        this.rankingManager = rankingManager;
        this.damagePopupManager = damagePopupManager;
        this.killLogManager = killLogManager;
    }
}
