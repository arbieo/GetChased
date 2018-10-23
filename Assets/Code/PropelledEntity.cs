using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PropelledEntity : Entity {

	public float maxTurnSpeed = 90;
	public float timeToTurn = 0.2f;
	public float maxSpeed = 50;
	public float timeToSpeed = 1f;
	public float maxBankAngle = 25; 

	[HideInInspector]
	public float turningSpeed;
	[HideInInspector]
	public float speed;
	[HideInInspector]
	public float targetSpeed;

	protected abstract void CalculateTargets();
	public override Vector2 PredictPosition(float time)
	{
		return transform.position + (Vector3)moveVector * speed * time;
	}
	protected void CalculateSteeringAndSpeed()
	{
		float angleDifference = Mathf.DeltaAngle(Mathf.Atan2(moveVector.y, moveVector.x) * Mathf.Rad2Deg, Mathf.Atan2(targetVector.y, targetVector.x) * Mathf.Rad2Deg);
		float targetRotation;
		float turnImpulse = 1/timeToTurn * maxTurnSpeed;
		float throttleImpulse = 1/timeToSpeed * maxTurnSpeed;
		if(angleDifference > 0 && angleDifference > turningSpeed * (timeToTurn + Time.fixedDeltaTime) / 2)
		{
			targetRotation = 1;
		}
		else if (angleDifference < 0 && angleDifference < turningSpeed * (timeToTurn + Time.fixedDeltaTime) / 2)
		{
			targetRotation = -1;
		}
		else {
			targetRotation = 0;
		}
		turningSpeed = Mathf.MoveTowards(turningSpeed, maxTurnSpeed * targetRotation, turnImpulse*Time.fixedDeltaTime);

		speed = Mathf.Clamp(Mathf.MoveTowards(speed, targetSpeed, throttleImpulse*Time.fixedDeltaTime), -maxSpeed, maxSpeed);
	}

	protected void MoveObject()
	{
		moveVector = Quaternion.Euler(0, 0, turningSpeed * Time.fixedDeltaTime) * (Vector3)moveVector;

		Vector3 position = transform.position;
		position += (Vector3)(moveVector * speed * Time.fixedDeltaTime);
		transform.position = position;
	}

	protected void UpdateDisplay()
	{
		float moveAngle = Mathf.Atan2(moveVector.y, moveVector.x) * Mathf.Rad2Deg;

		Quaternion displayRotation = Quaternion.AngleAxis(moveAngle, Vector3.forward);
		display.transform.rotation = displayRotation;
	}

	protected void DoPropelledStep()
	{
		CalculateTargets();

		CalculateSteeringAndSpeed();

		MoveObject();

		UpdateDisplay();
	}
}
