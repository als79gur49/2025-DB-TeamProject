using UnityEngine;
using UnityEngine.Events;

public class EntityData
{
    private int hp;
    private int damage;
    private float defense;

    private int score;

    public int HP => hp;
    public int Score => score;


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
        hp -= amount;
    }
};
