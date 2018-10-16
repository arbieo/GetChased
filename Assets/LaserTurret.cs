using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTurret : PropelledEntity {

	public GameObject[] lasers;

	public Entity target;

	public float laserWarmupTime = 3;
	public float laserCooldownTime = 10;
	public float laserFireTime = 2;
	public float acceptableDeviation = 50;

	bool firing;
	bool fireStartTime;

	public override void FixedUpdate () 
	{
		base.FixedUpdate();
		if(target == null)
		{
			target = GameObject.Find("Player").GetComponent<Player>();
		}
		DoPropelledStep();
	}
	protected override void CalculateTargets()
	{
		if (target != null)
		{
			Vector3 vectorToTarget = target.transform.position - transform.position;
			float projectionToTarget = Vector3.Dot(vectorToTarget, moveVector)/vectorToTarget.magnitude;
			float timeToTarget = projectionToTarget * speed / (vectorToTarget).magnitude;
			if(timeToTarget < 0)
			{
				timeToTarget = 0;
			}
			Vector3 predictedPosition = target.PredictPosition(laserWarmupTime);

			Vector2 followVector = (predictedPosition - transform.position).normalized;
		}
	}
}
