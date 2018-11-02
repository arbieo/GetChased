using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ReverseTimeSkill : Skill {

	public float timeToReverse;

	public override void Use(Player player)
	{
		int timeStepsToReverse = (int)(timeToReverse/GameController.instance.originalDeltaTime);
		if (player.positionHistory.Count < timeStepsToReverse)
		{
			timeStepsToReverse = player.positionHistory.Count - 1;
		}
		player.transform.position = player.positionHistory[timeStepsToReverse];
		player.moveVector = player.moveVectorHistory[timeStepsToReverse];
		player.targetVector = player.moveVector;
		base.Use(player);
	}
}
