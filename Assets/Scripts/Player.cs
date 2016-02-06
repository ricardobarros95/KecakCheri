using UnityEngine;
using System.Collections;

public class Player
{
    int health;
    bool invuln = false;

    public Player(int initHealth)
    {
        health = initHealth;
    }

    public void Damage(int amount)
    {
        if (invuln)
            invuln = false;
        else
            health -= amount;
    }

    public int GetHealth()
    {
        return health;
    }

    public void SetInvulnerable()
    {
        invuln = true;
    }

    public void SetHealth(int amount)
    {
        health = amount;
    }

    public bool IsInvulnerable()
    {
        return invuln;
    }
}
