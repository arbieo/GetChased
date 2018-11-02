using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class InvulnSkill : Skill {

	public float invulnTime;

	public override void Use(Player player)
	{
		player.BecomeEtheral(invulnTime);
		base.Use(player);
	}
}
