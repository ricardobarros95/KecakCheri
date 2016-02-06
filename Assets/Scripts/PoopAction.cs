using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoopAction
{
    public int damage;
    public int heal;
    public float globalSpeedFactor;

    Game game;
    string currentAction;
    public PoopAction(string actionString, Game game)
    {
        this.game = game;
        currentAction = actionString;
    }

	void Damage(Player target)
    {
        target.Damage(damage);
        game.speed -= globalSpeedFactor;
    }

    void Heal(Player target)
    {
        target.Damage(-heal);
    }

    void Dodge(Player target)
    {
        target.SetInvulnerable();
        game.speed += globalSpeedFactor;
    }

    public void Use(Player owner, Player target)
    {
        if (currentAction.Equals("Damage"))
        {
            Debug.Log("Attack!");
            Damage(target);
        }
        else if (currentAction.Equals("Heal"))
        {
            Debug.Log("Heal!");
            Heal(owner);
        }
        else
        {
            Debug.Log("Dodge!");
            Dodge(owner);
        }
    }

    public int GetActionId()
    {
        switch(currentAction)
        {
            case "Damage":
                return 0;
            case "Heal":
                return 1;
            case "Dodge":
                return 2;
        }
        return -1;
    }
}
