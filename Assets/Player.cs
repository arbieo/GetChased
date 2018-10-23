using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : PropelledEntity {

	public enum CONTROL_MODE {
		TURNING,
		FOLLOW,
		JOYSTICK,
	}

	public enum CONTROL_STATE {
		LOOP,
		FLYING,
		ROLLING,
		BOOSTING,
	}

	public CONTROL_MODE controlMode = CONTROL_MODE.TURNING;
	public CONTROL_STATE controlState = CONTROL_STATE.FLYING;

	public float loopDuration = 10;
	public float loopFullDuration;
	public float boostDuration = 2;
	public float rollDuration = 1.5f;

	public GameObject minePrefab;

	float originalMaxSpeed;

	float loopStartTime;
	float rollStartTime;
	float boostStartTime;
	Vector3 loopOrigin;
	Vector2 loopVector;
	float loopRadius;
	
	void Awake()
	{
		originalMaxSpeed = maxSpeed;
	}
	
	// Update is called once per frame
	public override void FixedUpdate () 
	{
		base.FixedUpdate();

		if(controlState == CONTROL_STATE.LOOP)
		{
			DoLoop();
			if(Time.time - loopStartTime >= loopFullDuration)
			{
				controlState = CONTROL_STATE.FLYING;
			}
		}
		else if(controlState == CONTROL_STATE.FLYING || controlState == CONTROL_STATE.BOOSTING || controlState == CONTROL_STATE.ROLLING) 
		{
			if(controlState == CONTROL_STATE.BOOSTING)
			{
				maxSpeed = originalMaxSpeed * 2;
				if(Time.time - boostStartTime >= boostDuration)
				{
					controlState = CONTROL_STATE.FLYING;
				}
			}
			else
			{
				maxSpeed = originalMaxSpeed;
			}
			DoPropelledStep();
			if (controlState == CONTROL_STATE.ROLLING)
			{
				float flipAngle = 360*(Time.time - rollStartTime)/rollDuration;
				display.transform.Rotate(new Vector3(flipAngle, 0,0 ));
				if(Time.time - rollStartTime >= rollDuration)
				{
					controlState = CONTROL_STATE.FLYING;
				}
			}
		}
	}

	public void DeployMines()
	{
		for (int i=0;i < 10; i++)
		{
			GameObject mine = GameObject.Instantiate(minePrefab, transform.position, Quaternion.identity);
			Mine mineEntity = mine.GetComponent<Mine>();
			mineEntity.moveVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * speed*1.5f;
		}
	}
	
	public void StartLoop()
	{
		if (controlState != CONTROL_STATE.FLYING)
		{
			return;
		}
		controlState = CONTROL_STATE.LOOP;
		loopOrigin = transform.position;
		float loopDistance = speed * loopDuration;
		loopRadius = loopDistance / (Mathf.PI * 2);
		loopVector = moveVector.normalized;
		loopStartTime = Time.time - Time.fixedDeltaTime;

		loopFullDuration = loopDuration * (1 + 1/Mathf.PI - 1f/4f);

		BecomeEtheral(loopFullDuration + 0.5f);
	}

	public void StartBoost()
	{
		if (controlState != CONTROL_STATE.FLYING)
		{
			return;
		}
		controlState = CONTROL_STATE.BOOSTING;
		boostStartTime = Time.time;
	}

	public void StartRoll()
	{
		if (controlState != CONTROL_STATE.FLYING)
		{
			return;
		}
		controlState = CONTROL_STATE.ROLLING;
		rollStartTime = Time.time;

		BecomeEtheral(rollDuration);
	}

	public void DoLoop()
	{
		float loopPercentage = (Time.time - loopStartTime)/loopDuration;
		float angle;
		float x;
		float y;
		float flipAngle = 0;
		if (loopPercentage < 0.625f)
		{
			float radPercentage = loopPercentage * Mathf.PI * 2;
			x = Mathf.Sin(radPercentage);
			angle = radPercentage;
			y = 1 - Mathf.Cos(radPercentage);
		}
		else if (loopPercentage < 0.625f + 1/Mathf.PI)
		{
			float stagePercentage = (loopPercentage - 0.625f)/(1/Mathf.PI);
			x = - Mathf.Sqrt(2)/2 - Mathf.Sqrt(2)*stagePercentage;
			y = 1 + Mathf.Sqrt(2)/2 - Mathf.Sqrt(2)*stagePercentage;
			angle = 0.625f * Mathf.PI * 2;
			flipAngle = Mathf.Min(stagePercentage * 180 * 2f, 180);
		}
		else
		{
			float stagePercentage = (loopPercentage - 0.625f - 1/Mathf.PI);
			float radPercentage = stagePercentage * 2 * Mathf.PI - 0.125f * 2 * Mathf.PI;
			x = -2*Mathf.Sqrt(2) - Mathf.Sin(radPercentage);
			y = 1 - Mathf.Cos(radPercentage);
			angle = 0.5f * Mathf.PI * 2 - radPercentage;
			flipAngle = 180;
		}

		transform.position = new Vector3(loopOrigin.x + loopVector.x*loopRadius*x, loopOrigin.y + loopVector.y*loopRadius*x, loopOrigin.z - loopRadius * y);
		if (loopPercentage < 0.25f)
		{
			moveVector = loopVector.normalized;
		}
		else
		{
			moveVector = -loopVector.normalized;
		}
		targetVector = moveVector;

		float moveAngle = Mathf.Atan2(loopVector.y, loopVector.x) * Mathf.Rad2Deg;
		Vector3 rMoveVector = new Vector3(-moveVector.y, moveVector.x, 0);

		Quaternion displayRotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, rMoveVector) * Quaternion.AngleAxis(moveAngle, Vector3.forward);
		display.transform.rotation = displayRotation;
		display.transform.Rotate(new Vector3(flipAngle, 0,0 ));
		float scale = 1 + y;
		transform.localScale = new Vector3(scale, scale, scale);
	}

	protected override void CalculateTargets()
	{
		switch (controlMode)
		{
			case CONTROL_MODE.TURNING:
				
			break;
			case CONTROL_MODE.FOLLOW:
			break;
			case CONTROL_MODE.JOYSTICK:
			break;
		}

		if (controlState == CONTROL_STATE.ROLLING)
		{
			targetVector = moveVector;
		}

		targetSpeed = maxSpeed;
	}

	public override void Kill()
	{
		base.Kill();
	}
}
