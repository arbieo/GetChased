using UnityEngine;

public class CastSkill : Skill
{
    public float range;

    public override bool IsCast()
    {
        return true;
    }

    public override void Use(Player player)
    {
        GameController.instance.StartCasting(this);
    }

    public virtual void Cast(Player player, Vector2 location)
    {
        base.Use(player);
    }
}