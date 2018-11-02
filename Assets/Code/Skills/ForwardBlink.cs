using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ForwardBlink : Skill {

	public float distance;

	public override void Use(Player player)
	{
		player.transform.position = (Vector2)player.transform.position + player.moveVector.normalized * distance;
		base.Use(player);
	}
}
