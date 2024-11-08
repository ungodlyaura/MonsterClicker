using UnityEngine;

[System.Serializable]
public class Weapon
{
    public int rarity;
    public int damage;

    public Weapon(int rarity, int damage)
    {
        this.rarity = rarity;
        this.damage = damage;
    }
}
