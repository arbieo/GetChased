using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowEnemy : PropelledEntity {

	[HideInInspector]
	public Entity target;

	public float distanceToSleep = 1000;

	public float throttleOnTurn = 1;
	public float maxThrottleAngle = 90;
	public float predictionStrength = 1;

	public float followStrength = 1;
	public float cohesionStrength = 0;
	public float avoidStrength = 0;

	public float cohesionMaxDistance = 40;
	public float avoidMaxDistance = 20;

	public override void FixedUpdate () 
	{
		base.FixedUpdate();
		if (((Vector2)Camera.main.transform.position - (Vector2)transform.position).magnitude > distanceToSleep)
		{
			return;
		}
		DoPropelledStep();
	}

	protected override void CalculateTargets()
	{
		Vector2 resultantVector = new Vector2(0,0);
		if (target != null)
		{
			Vector3 vectorToTarget = target.transform.position - transform.position;
			float projectionToTarget = Vector3.Dot(vectorToTarget, moveVector)/vectorToTarget.magnitude;
			float timeToTarget = projectionToTarget * speed / (vectorToTarget).magnitude;
			if(timeToTarget < 0)
			{
				timeToTarget = 0;
			}
			Vector3 predictedPosition = target.PredictPosition(timeToTarget*predictionStrength);

			Vector2 followVector = (predictedPosition - transform.position).normalized;
			resultantVector += followVector.normalized*followStrength;
		}

		Vector2 cohesionVector = new Vector2(0,0);
		Vector2 avoidVector = new Vector2(0,0);
		if (Mathf.Abs(cohesionStrength) > 0 || Mathf.Abs(avoidStrength) > 0){
			foreach (Entity entity in entities)
			{
				if(entity == this) continue;
				if(!(entity == target))
				{
					Vector2 entityVector = entity.transform.position - transform.position;
					float distance = entityVector.magnitude;
					if(distance < cohesionMaxDistance)
					{
						float weight = cohesionMaxDistance - distance;
						cohesionVector += entity.moveVector.normalized * weight;
					}
					if(distance < avoidMaxDistance)
					{
						float weight = (avoidMaxDistance - distance) * entity.collideDamage;
						avoidVector += -entityVector.normalized * weight;
					}
				}
			}
		}

		resultantVector += cohesionVector.normalized*cohesionStrength;
		resultantVector += avoidVector.normalized*avoidStrength;

		if(resultantVector == Vector2.zero)
		{
			targetSpeed = 0;
			targetVector = Vector2.zero;
		}
		else
		{
			targetVector = resultantVector.normalized;
			
			float angleToTarget = Mathf.DeltaAngle(Mathf.Atan2(moveVector.y, moveVector.x) * Mathf.Rad2Deg, Mathf.Atan2(targetVector.y, targetVector.x) * Mathf.Rad2Deg);
			float throttleAngle = Mathf.Min(maxThrottleAngle, Mathf.Abs(angleToTarget));
			targetSpeed = maxSpeed * (1 - throttleAngle/maxThrottleAngle + throttleOnTurn * throttleAngle/maxThrottleAngle);
		}
	}
}
