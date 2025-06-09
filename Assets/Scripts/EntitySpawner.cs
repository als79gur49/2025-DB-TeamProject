using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [SerializeField]
    public DamagePopupManager damagePopupManager;
    [SerializeField]
    public KillLogManager killLogManager;
    [SerializeField]
    public ScoreBlockSpawner scoreBlockSpawner;

    public void Setup(DamagePopupManager damagePopupManager, KillLogManager killLogManager, ScoreBlockSpawner scoreBlockSpawner)
    {
        this.damagePopupManager = damagePopupManager;
        this.killLogManager = killLogManager;
        this.scoreBlockSpawner = scoreBlockSpawner;
    }
}
