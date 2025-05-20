using System.Collections.Generic;
using UnityEngine;


public abstract class Entity : MonoBehaviour
{
    //private EntityInput input;
    private FSM brain;
    //private EntityOutput output;

    private EntityInfo info;
    protected EntityData data;

    private RankingManager rankingManager;

    public EntityData Data => data;
    public EntityInfo Info => info;


    public void Setup(RankingManager rankingManager, EntityInfo info, EntityData data)
    {
        if(TryGetComponent<FSM>(out FSM brain))
        {
            this.brain = brain;
            this.brain.Setup(this);           
        }

        this.rankingManager = rankingManager;

        this.info = info;
        this.data = data;

        rankingManager?.AddEntity(this);
    }

    private void Update()
    {
        brain.Execute();
    }

    public void ChangeState(EntityStates nextState)
    {
        brain.ChangeState(nextState);
    }
    private void OnDisable()
    {
        //Enable�� Setup���� ���� �۵��Ͽ� AddEntity��ġ�� Setup���� ����   
        rankingManager?.RemoveEntity(this);
    }
};
