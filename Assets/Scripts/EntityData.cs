using UnityEngine;
using UnityEngine.Events;

public class EntityData
{
    private UnityEvent<int, int> onTakeDamage;
    private UnityEvent onDeath;

    private int hp;
    private int damage;
    private float defense;

    private int score;

    public int HP => hp;
    public int Score => score;

    public bool IsDead => (hp <= 0);

    public EntityData() { }
    public EntityData(int hp, int damage, float defense)
    {
        this.hp = hp;
        this.damage = damage;
        this.defense = defense;
    }

    public void Setup(EntityData data)
    {
        Setup(data.HP, data.damage, data.defense);
    }

    public void Setup(int hp, int damage, float defense)
    {
        this.hp = hp;
        this.damage = damage;
        this.defense = defense;

        score = 0;
    }

    public void TakeDamage(int amount)
    {
        int prevHp = hp;

        //defense 추가할 꺼면 로직 수정
        hp -= amount;

        //2가지 이벤트, hpUI 수정, 데미지 출력
        onTakeDamage?.Invoke(prevHp, hp);

        if (IsDead)
        {
            onDeath?.Invoke();
        }
    }
};
