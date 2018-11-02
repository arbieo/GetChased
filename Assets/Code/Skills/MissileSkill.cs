using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MissileSkill : Skill {

	public GameObject missilePrefab;

	public override void Use(Player player)
	{
		Entity target = null;
		foreach (Entity entity in Entity.entities)
		{
			if (entity.team == Entity.Team.ENEMY && (target == null || (entity.health > 1 && 
				(player.transform.position - entity.transform.position).magnitude < (player.transform.position - target.transform.position).magnitude)))
			{
				target = entity;
			}
		}

		GameObject missile1 = GameObject.Instantiate(missilePrefab, player.transform.position, Quaternion.identity);
		FollowEnemy missile1Entity = missile1.GetComponent<FollowEnemy>();
		missile1Entity.AvoidCollisions(player, 1);
		missile1Entity.moveVector = player.moveVector + new Vector2(-player.moveVector.y, player.moveVector.x);
		missile1Entity.DelayedKill(10);
		missile1Entity.speed = missile1Entity.maxSpeed;
		missile1Entity.target = target;

		GameObject missile2 = GameObject.Instantiate(missilePrefab, player.transform.position, Quaternion.identity);
		FollowEnemy missile2Entity = missile2.GetComponent<FollowEnemy>();
		missile2Entity.AvoidCollisions(player, 1);
		missile2Entity.moveVector = player.moveVector + new Vector2(player.moveVector.y, -player.moveVector.x);
		missile2Entity.DelayedKill(10);
		missile2Entity.speed = missile2Entity.maxSpeed;
		missile2Entity.target = target;

		missile1Entity.AvoidCollisions(missile2Entity,5);

		base.Use(player);
	}
}
