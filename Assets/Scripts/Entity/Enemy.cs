using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : Entity
{
    // FSM ����
    private AIInput input;
    private FSM brain;
    //private EntityOutput output;

    private MemoryPool<Enemy> memoryPool;
    public MemoryPool<Enemy> MemoryPool => memoryPool;

    private bool isInit = false;

    public void Setup(
        EntityInfo info,
        EntityData data,
        MemoryPool<Enemy> memoryPool,
        DamagePopupManager damagePopupManager, KillLogManager killLogManager,
        ScoreBlockSpawner scoreBlockSpawner)
    {
        Setup(info, data, damagePopupManager, killLogManager, scoreBlockSpawner);
        this.memoryPool = memoryPool;
    }

    protected override void Setup()
    {
        base.Setup();

        if (TryGetComponent<FSM>(out FSM brain))
        {
            this.brain = brain;
            this.brain.Setup(this);
        }

        input = GetComponent<AIInput>();
        input.self = this.gameObject;
        input.SetEntity(this);
        input.SetAnimation(animation);



        if (!isInit)
        {
            isInit = true;
        }
            EntityGameManager.AddAIEntity(GetInstanceID(), Info.EntityName);



        onDeath.AddListener(StartDespawnTimer); // n�� �� memoryPool�� ��ȯ
    }

    private void Update()
    {
        brain.Execute(input);
    }

    public void ChangeState(EntityStates nextState)
    {
        brain.ChangeState(nextState, input);
    }

    public void StartDespawnTimer()
    {
        StartCoroutine(ReturnToPoolAfterDelay(5));
    }

    private IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        onDeath.RemoveAllListeners();
        // �޸� Ǯ�� ��ȯ
        memoryPool.DeactivatePoolItem(this);
        EntityGameManager.OnEntityDeath(GetInstanceID(), Info.EntityName);
    }

    protected override void levelup()
    {
    }

    private void OnDisable()
    {
    }
}
