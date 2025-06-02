using UnityEngine;
using UnityEngine.Events;

public class EntityData
{
    private float maxHp;
    private float hp;
    private int damage;
    private float defense;

    private int score;

    public float MaxHp => maxHp;
    public float HP => hp;
    public int Score => score;

    public float HPPercent => hp / maxHp * 100;

    public EntityData(int hp, int damage, float defense)
    {
        Setup(hp, damage,defense);
    }

    public void Setup(EntityData data)
    {
        Setup(data.HP, data.damage, data.defense);
    }

    public void Setup(float hp, int damage, float defense)
    {
        this.maxHp = hp;
        this.hp = hp;
        this.damage = damage;
        this.defense = defense;

        score = 0;
    }

    public void TakeDamage(float amount)
    {
        hp -= amount;
    }

    public void AddScore(int amount)
    {
        score += amount;
    }
};
