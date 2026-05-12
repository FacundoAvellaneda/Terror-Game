using UnityEngine;

[CreateAssetMenu(fileName = "StatsEntitty", menuName = "Stats /StatsEntitty")]
public class StatsEntitty : ScriptableObject
{
    public enum Type { Player, Enemy }

    [Header("General Stats")]

    [SerializeField] private Type type;
    public Type _type => type;

    [SerializeField] private float maxHealth;
    public float MaxHealth => maxHealth;

    [SerializeField] private float speed;
    public float Speed => speed;

    [SerializeField] private float damage;
    public float Damage => damage;
}