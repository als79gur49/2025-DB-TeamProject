using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public abstract class Entity : MonoBehaviour, IAttack, IDamageable
{
    public UnityEvent<int, int> onTakeDamage;
    public UnityEvent onDeath;

    private AIInput input;
    private FSM brain;
    //private EntityOutput output;

    private EntityAnimation animation;

    private EntityInfo info;
    protected EntityData data;
    public bool IsDead => (data.HP <= 0);

    private RankingManager rankingManager;

    public EntityData Data => data;
    public EntityInfo Info => info;

    [SerializeField]
    private Transform firePoint;
    [SerializeField]
    private GameObject projectile;

    public void Setup(RankingManager rankingManager, EntityInfo info, EntityData data)
    {
        this.info = info;
        this.data = data;

        this.rankingManager = rankingManager;
        rankingManager?.AddEntity(this);

        Setup();
    }

    private void Setup()
    {
        if (TryGetComponent<FSM>(out FSM brain))
        {
            this.brain = brain;
            this.brain.Setup(this);
        }

        animation = GetComponent<EntityAnimation>();

        input = GetComponent<AIInput>();
        input.self = this.gameObject;
        input.SetEntity(this);
        input.SetAnimation(animation);      
    }


    private void Update()
    {
        brain.Execute(input);
    }


    public void ChangeState(EntityStates nextState)
    {
        brain.ChangeState(nextState, input);
    }
    private void OnDisable()
    {
        //Enable이 Setup보다 빨리 작동하여 AddEntity위치는 Setup으로 변경   
        rankingManager?.RemoveEntity(this);
    }

    // IAttack
    public void Attack()
    {
        GameObject clone;

        if (firePoint == null || projectile == null)
        {
            Debug.Log("발사 위치 혹은 투사체 x");

            return;
        }

        clone = Instantiate(projectile, firePoint.position, Quaternion.identity);
        clone.transform.localRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
    }

    // IDamageable
    public void TakeDamage(int amount)
    {
        int prevHp = data.HP;

        //defense 추가할 꺼면 로직 수정
        data.TakeDamage(amount);

        //2가지 이벤트, hpUI 수정, 데미지 출력
        onTakeDamage?.Invoke(prevHp, data.HP);
    }
};
