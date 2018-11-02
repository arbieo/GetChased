using UnityEngine;
using UnityEngine.UI;

public abstract class Skill
{
    public string name;
    public float cooldown;

    public float lastTimeUsed;
    public bool onCooldown = false;

    public virtual bool IsCast()
    {
        return false;
    }

    public virtual void Use(Player player)
    {
        lastTimeUsed = Time.time;
        onCooldown = true;
    }
}