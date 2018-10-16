using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : PropelledEntity {

	public enum CONTROL_MODE {
		TURNING,
		FOLLOW,
		JOYSTICK,
	}

	public CONTROL_MODE controlMode = CONTROL_MODE.TURNING;

	public float loopDuration = 10;
	private float loopFullDuration;

	public GameObject minePrefab;

	float loopStartTime;
	Vector3 loopOrigin;
	Vector2 loopVector;
	float loopRadius;
	bool isLooping = false;
	
	// Update is called once per frame
	public override void FixedUpdate () 
	{
		base.FixedUpdate();

		if(Input.GetKeyDown(KeyCode.DownArrow) && !isLooping)
		{
			StartLoop();
		}

		if(Input.GetKeyDown(KeyCode.Space))
		{
			DeployMines();
		}

		if(isLooping)
		{
			DoLoop();
			if(Time.time - loopStartTime >= loopFullDuration)
			{
				isLooping = false;
			}
		}
		else {
			DoPropelledStep();
		}
	}

	public void DeployMines()
	{
		for (int i=0;i < 20; i++)
		{
			GameObject mine = GameObject.Instantiate(minePrefab, transform.position, Quaternion.identity);
			Mine mineEntity = mine.GetComponent<Mine>();
			mineEntity.moveVector = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * speed*2;
		}
	}
	
	public void StartLoop()
	{
		isLooping = true;
		loopOrigin = transform.position;
		float loopDistance = speed * loopDuration;
		loopRadius = loopDistance / (Mathf.PI * 2);
		loopVector = moveVector.normalized;
		loopStartTime = Time.time - Time.fixedDeltaTime;

		loopFullDuration = loopDuration * (1 + 1/Mathf.PI - 1f/4f);

		BecomeEtheral(loopFullDuration);
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
			angle = -0.125f * Mathf.PI * 2;
			flipAngle = Mathf.Min(stagePercentage * 180 * 2f, 180);
		}
		else
		{
			float stagePercentage = (loopPercentage - 0.625f - 1/Mathf.PI);
			float radPercentage = stagePercentage * 2 * Mathf.PI - 0.125f * 2 * Mathf.PI;
			x = -2*Mathf.Sqrt(2) - Mathf.Sin(radPercentage);
			y = 1 - Mathf.Cos(radPercentage);
			angle = radPercentage;
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

		float moveAngle = Mathf.Atan2(moveVector.y, moveVector.x) * Mathf.Rad2Deg;
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
				if (Input.GetKey(KeyCode.LeftArrow))
				{
					targetVector = new Vector2(-moveVector.y, moveVector.x);
				}
				else if (Input.GetKey(KeyCode.RightArrow))
				{
					targetVector = new Vector2(moveVector.y, -moveVector.x);
				}
				else
				{
					targetVector = moveVector;
				}
			break;
			case CONTROL_MODE.FOLLOW:
			break;
			case CONTROL_MODE.JOYSTICK:
			break;
		}

		targetSpeed = maxSpeed;
	}

	public override void Kill()
	{
		base.Kill();
		SceneManager.LoadScene("startScene");
	}
}
