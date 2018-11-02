using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ShootStraightSkill : Skill {

	public GameObject bulletPrefab;
    public float shotRate;
    public float duration;

	public override void Use(Player player)
	{
		ShootingEffect shootingEffect = new ShootingEffect(bulletPrefab, shotRate, duration);
		player.AddEffect(shootingEffect);
		base.Use(player);
	}
}
