using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkSkill : CastSkill {

	public override void Cast(Player player, Vector2 location)
	{
		player.transform.position = location;
		base.Cast(player, location);
	}
}
