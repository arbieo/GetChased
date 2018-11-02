using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTurret : PropelledEntity, Enemy {

	public Laser[] lasers;

	public Entity target { get; set; }

	public float laserWarmupTime = 3;
	public float laserCooldownTime = 10;
	public float laserFireTime = 2;
	public float acceptableDeviation = 50;

	bool firing;
	float fireStartTime;

	public void Start()
	{
		fireStartTime = Random.Range(-laserCooldownTime, 0);
	}

	public override void FixedUpdate () 
	{
		base.FixedUpdate();


		if (firing)
		{
			float laserTime = Time.time - fireStartTime;
			if (laserTime < laserWarmupTime)
			{
				foreach (Laser laser in lasers)
				{
					laser.line.enabled = true;
					laser.line.startWidth = laserTime/laserWarmupTime;
					laser.line.endWidth = laserTime/laserWarmupTime;

					laser.laserCollider.enabled = false;
				}
			}
			else if (laserTime < laserWarmupTime + laserFireTime)
			{
				foreach (Laser laser in lasers)
				{
					laser.line.enabled = true;
					laser.line.startWidth = 5;
					laser.line.endWidth = 5;
					
					laser.laserCollider.enabled = true;
				}
			}
			else
			{
				firing = false;
				foreach (Laser laser in lasers)
				{
					laser.line.enabled = false;
					laser.laserCollider.enabled = false;
				}
			}
		}
		else if (Mathf.DeltaAngle(Mathf.Atan2(moveVector.y, moveVector.x), Mathf.Atan2(targetVector.y, targetVector.x)) < acceptableDeviation 
			&& Time.time - fireStartTime > laserCooldownTime)
		{
			speed = 0;
			turningSpeed = 0;

			firing = true;
			fireStartTime = Time.time;
		}
		else
		{
			foreach (Laser laser in lasers)
			{
				laser.line.enabled = false;
				laser.laserCollider.enabled = false;
			}
			DoPropelledStep();
		}
	}

	protected override void CalculateTargets()
	{
		targetVector = (target.transform.position - transform.position).normalized;
	}
}
