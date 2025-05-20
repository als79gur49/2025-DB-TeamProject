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
    public void TakeDamage(int amount)
    {
        int prevHp = hp;

        //defense �߰��� ���� ���� ����
        hp -= amount;

        //2���� �̺�Ʈ, hpUI ����, ������ ���
        onTakeDamage?.Invoke(prevHp, hp);

        if (IsDead)
        {
            onDeath?.Invoke();
        }
    }
};
