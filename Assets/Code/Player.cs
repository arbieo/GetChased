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

	public CONTROL_MODE controlMode = CONTROL_MODE.TURNING;

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

	public void StartBoost()
	{
		if (controlState != CONTROL_STATE.FLYING)
		{
			return;
		}
		controlState = CONTROL_STATE.BOOSTING;
		boostStartTime = Time.time;
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
