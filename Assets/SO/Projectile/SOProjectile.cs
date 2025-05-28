using UnityEngine;

[CreateAssetMenu(fileName = "SO", menuName = "Projectile", order = 0)]
public class SOProjectile : ScriptableObject
{
    public string projectileName;

    public float damage;
    public float speed;
    public float range;
    public float duration;
    public float attackRate;

    public float size;
}
