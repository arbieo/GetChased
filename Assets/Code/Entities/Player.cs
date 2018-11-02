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

	public GameObject minePrefab;

	public List<Vector2> positionHistory = new List<Vector2>();
	public List<Vector2> moveVectorHistory = new List<Vector2>();
	private const float HISTORY_TO_STORE = 5;
	private float lastHistoryTick = 0;
	
	// Update is called once per frame
	public override void FixedUpdate () 
	{
		base.FixedUpdate();

		if (lastHistoryTick == 0)
		{
			lastHistoryTick = Time.time;
		}
		if (Time.time - lastHistoryTick > GameController.instance.originalDeltaTime)
		{
			lastHistoryTick += GameController.instance.originalDeltaTime;
			positionHistory.Insert(0, transform.position);
			moveVectorHistory.Insert(0, moveVector);
			if (positionHistory.Count > HISTORY_TO_STORE/GameController.instance.originalDeltaTime)
			{
				positionHistory.RemoveAt(positionHistory.Count-1);
			}
			if (moveVectorHistory.Count > HISTORY_TO_STORE/GameController.instance.originalDeltaTime)
			{
				moveVectorHistory.RemoveAt(moveVectorHistory.Count-1);
			}
		}
		
		/*if(HasEffect(Effect.Type.BOOST)) 
		{
			maxSpeed = originalMaxSpeed * 2;
		}*/
		DoPropelledStep();
		if (HasEffect(Effect.Type.ETHERAL))
		{
			Effect etheralEffect = effectsByType[Effect.Type.ETHERAL][0];
			float flipAngle = 360*(Time.time - etheralEffect.startTime)/etheralEffect.duration;
			display.transform.Rotate(new Vector3(flipAngle, 0,0 ));
		}
		else 
		{
			display.transform.Rotate(new Vector3(turningSpeed/maxTurnSpeed*maxBankAngle, 0,0 ));
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

	protected override void CalculateTargets()
	{
		targetSpeed = maxSpeed;
	}

	public override void Kill()
	{
		base.Kill();
	}
}
