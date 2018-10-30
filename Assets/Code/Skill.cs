using UnityEngine.UI;

public class Skill
{
    public Type type;
    public string name;
    public float strength;
    public float cooldown;

    public SkillButton skillButton;
    public float lastTimeUsed;
    public bool onCooldown = false;

    public enum Type
	{
		BLNK,
		TIME_REVERSE,
		INVULN,
		TURRET,
	}

    public Skill(Type type, string name, float strength, float cooldown)
    {
        this.type = type;
        this.name = name;
        this.strength = strength;
        this.cooldown = cooldown;
    }
}